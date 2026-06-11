using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景模板字典——从 SceneTemplates.json 加载所有场景模板定义
/// </summary>
public class SceneTemplateDictionary : Singleton<SceneTemplateDictionary>
{
    // 所有模板（key=模板ID）
    private Dictionary<int, SceneTemplateData> all_templates;

    private SceneTemplateDictionary()
    {
        this.all_templates = new Dictionary<int, SceneTemplateData>();

        // 从 Resources/Data/SceneTemplates.json 加载模板数据
        TextAsset json_text = Resources.Load<TextAsset>("Data/SceneTemplates");
        if (json_text == null)
        {
            Debug.LogError("SceneTemplates.json 未找到，请确保 Assets/Resources/Data/SceneTemplates.json 存在");
            return;
        }

        SceneTemplateDataListJSON database = JsonUtility.FromJson<SceneTemplateDataListJSON>(json_text.text);
        if (database == null || database.templates == null)
        {
            Debug.LogError("SceneTemplates.json 解析失败，请检查 JSON 格式");
            return;
        }

        // 遍历JSON条目，逐条解析为SceneTemplateData
        for (int i = 0; i < database.templates.Count; i++)
        {
            SceneTemplateDataJSON entry = database.templates[i];
            SceneTemplateData template = new SceneTemplateData(
                id: entry.id,
                name: entry.name,
                action_ids: entry.action_ids,
                trade_types: ParseTradeTypes(entry.trade_types)
            );
            this.all_templates[template.id] = template;
        }

        Debug.Log("SceneTemplateDictionary: 从 SceneTemplates.json 加载了 " + this.all_templates.Count + " 个模板");
    }

    // 将 JSON 中的 trade_types 字符串列表解析为 ItemTypeEnum 列表
    private List<ItemSubTypeEnum> ParseTradeTypes(List<string> typeNames)
    {
        if (typeNames == null || typeNames.Count == 0)
        {
            return null;
        }

        List<ItemSubTypeEnum> types = new List<ItemSubTypeEnum>();
        for (int i = 0; i < typeNames.Count; i++)
        {
            ItemSubTypeEnum parsed = (ItemSubTypeEnum)0;
            System.Enum.TryParse(typeNames[i], out parsed);
            types.Add(parsed);
        }
        return types;
    }

    /// <summary>
    /// 根据ID获取模板，找不到返回null
    /// </summary>
    public SceneTemplateData Get(int id)
    {
        if (this.all_templates.ContainsKey(id))
        {
            return this.all_templates[id];
        }
        return null;
    }
}
