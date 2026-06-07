using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 移动面板——显示当前场景的子场景列表，支持场景切换
/// </summary>
public class PanelMove : MonoBehaviour
{
    // 场景切换时触发，供其他面板刷新
    public static event System.Action OnSceneChanged;

    // 在Inspector中拖入 Button_Move 预制体
    public GameObject move_button_prefab;

    private Transform content;
    private Button button_leave;

    private void Awake()
    {
        // 订阅行动执行事件，自动刷新
        ActionEffect.OnActionExecuted += Refresh;

        content = this.transform.Find("Scroll_View/Viewport/Content");

        // 绑定离开按钮
        button_leave = this.transform.Find("Button_Leave")?.GetComponent<Button>();
        if (button_leave != null)
        {
            button_leave.onClick.AddListener(OnLeaveClicked);
        }
    }

    private void Start()
    {
        Refresh();
    }

    /// <summary>
    /// 根据当前场景的子场景列表刷新移动按钮
    /// </summary>
    public void Refresh()
    {
        // 清空现有按钮
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        CharacterData player = Player.Instance.Get();
        if (player == null)
        {
            return;
        }

        SceneData current_scene = SceneDictionary.Instance.Get(player.current_scene_id);
        if (current_scene == null)
        {
            return;
        }

        // 为每个子场景创建移动按钮
        if (current_scene.children_ids != null)
        {
            for (int i = 0; i < current_scene.children_ids.Count; i++)
            {
                int child_scene_id = current_scene.children_ids[i];
                SceneData child_scene = SceneDictionary.Instance.Get(child_scene_id);
                if (child_scene == null)
                {
                    continue;
                }

                // 实例化预制体
                GameObject button_obj = Instantiate(move_button_prefab, content);
                button_obj.name = "MoveTo_" + child_scene.name;

                // 设置文字
                TextMeshProUGUI text_tmp = button_obj.transform.Find("Image_MoveTitle/Text_MoveTitle").GetComponent<TextMeshProUGUI>();
                text_tmp.text = child_scene.name;

                // 绑定点击事件
                Button button = button_obj.GetComponent<Button>();
                int captured_id = child_scene_id;
                button.onClick.AddListener(() =>
                {
                    EnterScene(captured_id);
                });
            }
        }
    }

    // 进入指定场景
    private void EnterScene(int scene_id)
    {
        CharacterData player = Player.Instance.Get();
        if (player == null)
        {
            return;
        }

        player.current_scene_id = scene_id;

        // 进入新场景，时间推进1个时段
        TimeManager.Instance.AdvanceTime(1);

        // 通知所有面板场景已切换
        OnSceneChanged?.Invoke();
        // 自己刷新
        Refresh();
    }

    // 点击离开按钮
    private void OnLeaveClicked()
    {
        CharacterData player = Player.Instance.Get();
        if (player == null)
        {
            return;
        }

        SceneData current_scene = SceneDictionary.Instance.Get(player.current_scene_id);
        if (current_scene == null)
        {
            return;
        }

        if (current_scene.parent_scene_id >= 0)
        {
            // 有上级场景：返回上级
            player.current_scene_id = current_scene.parent_scene_id;
        }
        else
        {
            // 顶级场景：返回大地图（暂未实现，占位）
            Debug.Log("返回大地图（功能待实现）");
            return;
        }

        // 通知所有面板场景已切换
        OnSceneChanged?.Invoke();
        // 自己刷新
        Refresh();
    }

    private void OnDestroy()
    {
        ActionEffect.OnActionExecuted -= Refresh;
    }
}
