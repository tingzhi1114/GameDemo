using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 场景人物面板——显示当前场景中的所有NPC
/// </summary>
public class PanelCharacter : MonoBehaviour
{
    // 在Inspector中拖入 Button_Character 预制体
    public GameObject character_button_prefab;

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
    /// 根据当前场景刷新人物列表
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

        int current_scene_id = player.current_scene_id;

        // 通过场景的character_ids直接获取当前场景的角色
        SceneData current_scene = SceneDictionary.Instance.Get(current_scene_id);
        if (current_scene == null || current_scene.character_ids == null)
        {
            return;
        }

        foreach (int char_id in current_scene.character_ids)
        {
            // 跳过玩家自己
            if (char_id == player.id)
            {
                continue;
            }

            CharacterData character = CharacterDictionary.Instance.Get(char_id);
            if (character == null)
            {
                continue;
            }

            // 实例化预制体
            GameObject button_obj = Instantiate(character_button_prefab, content);
            button_obj.name = "Char_" + character.name;

            // 设置NPC名字
            TextMeshProUGUI text_name = button_obj.transform.Find("Text_Name").GetComponent<TextMeshProUGUI>();
            text_name.text = character.name + "（" + character.age + "岁）";

            // 设置关系（暂无社交系统，占位）
            TextMeshProUGUI text_relationship = button_obj.transform.Find("Text_Relationship").GetComponent<TextMeshProUGUI>();
            text_relationship.text = "普通";
        }
    }
}
