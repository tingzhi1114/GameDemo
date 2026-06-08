using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行动字典——从 Actions.json 加载所有行动定义
/// </summary>
public class ActionDictionary : Singleton<ActionDictionary>
{
    // 所有行动（key=行动ID）
    private Dictionary<int, ActionData> all_actions;

    private ActionDictionary()
    {
        this.all_actions = new Dictionary<int, ActionData>();

        // 从 Resources/Data/Actions.json 加载行动数据
        TextAsset json_text = Resources.Load<TextAsset>("Data/Actions");
        if (json_text == null)
        {
            Debug.LogError("Actions.json 未找到，请确保 Assets/Resources/Data/Actions.json 存在");
            return;
        }

        ActionDataListJSON database = JsonUtility.FromJson<ActionDataListJSON>(json_text.text);
        if (database == null || database.actions == null)
        {
            Debug.LogError("Actions.json 解析失败，请检查 JSON 格式");
            return;
        }

        // 遍历JSON条目，逐条解析为ActionData
        for (int i = 0; i < database.actions.Count; i++)
        {
            ActionDataJSON entry = database.actions[i];
            ActionData action = new ActionData(
                id: entry.id,
                name: entry.name
            );
            this.all_actions[action.id] = action;
        }

        Debug.Log("ActionDictionary: 从 Actions.json 加载了 " + this.all_actions.Count + " 个行动");
    }

    /// <summary>
    /// 根据ID获取行动，找不到返回null
    /// </summary>
    public ActionData Get(int id)
    {
        if (this.all_actions.ContainsKey(id))
        {
            return this.all_actions[id];
        }
        return null;
    }
}
