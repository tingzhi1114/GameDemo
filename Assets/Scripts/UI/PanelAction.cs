using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 行动面板——根据当前场景模板动态生成行动按钮
/// </summary>
public class PanelAction : MonoBehaviour
{
    /// <summary>
    /// 行动执行后触发，EventManager订阅此事件
    /// </summary>
    public static event System.Action OnActionExecuted;

    // 在Inspector中拖入 Button_Action 预制体
    public GameObject action_button_prefab;

    private Transform content;

    private void Awake()
    {
        content = this.transform.Find("Scroll_View/Viewport/Content");
    }

    private void Start()
    {
        Refresh();
    }

    /// <summary>
    /// 根据当前场景的模板刷新行动按钮列表
    /// </summary>
    public void Refresh()
    {
        // 清空现有按钮
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        CharacterData player = Player.Instance.GetCharacter();
        if (player == null)
        {
            return;
        }

        // 获取当前场景模板
        SceneData current_scene = SceneDictionary.Instance.Get(player.current_scene_id);
        if (current_scene == null)
        {
            return;
        }

        SceneTemplateData template = SceneTemplateDictionary.Instance.Get(current_scene.template_id);
        if (template == null || template.action_ids == null)
        {
            return;
        }

        // 为每个行动创建按钮
        for (int i = 0; i < template.action_ids.Count; i++)
        {
            int action_id = template.action_ids[i];
            ActionData action_data = ActionDictionary.Instance.Get(action_id);
            if (action_data == null)
            {
                continue;
            }

            // 实例化预制体
            GameObject button_obj = Instantiate(action_button_prefab, content);
            button_obj.name = "Button_" + action_data.name;

            // 设置按钮文字
            TextMeshProUGUI text_tmp = button_obj.transform.Find("Text_Action").GetComponent<TextMeshProUGUI>();
            text_tmp.text = action_data.name;

            // 绑定点击事件
            Button button = button_obj.GetComponent<Button>();
            int captured_id = action_id;
            button.onClick.AddListener(() =>
            {
                ActionEffect.Instance.Execute(captured_id, Player.Instance.character_id);
                // 行动执行完毕，通知EventManager刷新UI
                OnActionExecuted?.Invoke();
            });
        }
    }
}
