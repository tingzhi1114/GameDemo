using System.Collections.Generic;

/// <summary>
/// 场景模板数据——定义某类场景的通用配置
/// </summary>
public class SceneTemplateData
{
    // 模板ID
    public int id;
    // 模板名称（如"客栈"、"市集"）
    public string name;
    // 该模板场景可执行的操作ID列表
    public List<int> action_ids;

    public SceneTemplateData(int id, string name, List<int> action_ids = null)
    {
        this.id = id;
        this.name = name;
        this.action_ids = action_ids;
    }
}
