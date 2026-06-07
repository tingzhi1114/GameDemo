using System.Collections.Generic;

/// <summary>
/// 场景模板字典——存放所有场景模板定义
/// </summary>
public class SceneTemplateDictionary : Singleton<SceneTemplateDictionary>
{
    // 所有模板（key=模板ID）
    private Dictionary<int, SceneTemplateData> all_templates;

    private SceneTemplateDictionary()
    {
        this.all_templates = new Dictionary<int, SceneTemplateData>()
        {
            {
                1,
                new SceneTemplateData(
                    id: 1,
                    name: "市集",
                    action_ids: new List<int>() { 1, 2 }
                )
            },
            {
                2,
                new SceneTemplateData(
                    id: 2,
                    name: "客栈",
                    action_ids: new List<int>() { 3, 4, 5 }
                )
            },
            {
                3,
                new SceneTemplateData(
                    id: 3,
                    name: "寺庙",
                    action_ids: new List<int>() { 6, 7 }
                )
            },
            {
                4,
                new SceneTemplateData(
                    id: 4,
                    name: "城镇",
                    action_ids: new List<int>() { 8 }
                )
            }
        };
    }

    /// <summary>
    /// 添加一个模板到字典中
    /// </summary>
    public void Add(SceneTemplateData template)
    {
        this.all_templates[template.id] = template;
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

    /// <summary>
    /// 移除指定ID的模板
    /// </summary>
    public void Remove(int id)
    {
        if (this.all_templates.ContainsKey(id))
        {
            this.all_templates.Remove(id);
        }
    }

    /// <summary>
    /// 获取所有模板（返回副本）
    /// </summary>
    public List<SceneTemplateData> GetAll()
    {
        return new List<SceneTemplateData>(this.all_templates.Values);
    }
}
