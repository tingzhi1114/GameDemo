using System.Collections.Generic;

/// <summary>
/// 场景数据模型——地点内部的子区域
/// parent_scene_id = -1 表示顶级场景
/// </summary>
public class SceneData
{
    // 场景ID
    public int id;
    // 所属大地图节点ID
    public int location_id;
    // 上级场景ID（-1表示顶级场景）
    public int parent_scene_id;
    // 子场景ID列表（双向链表，方便查询）
    public List<int> children_ids;
    // 场景模板ID（指向SceneTemplateDictionary）
    public int template_id;
    // 场景名称（如"秦淮河畔"）
    public string name;

    public SceneData(int id, int location_id, string name, int parent_scene_id = -1, int template_id = -1, List<int> children_ids = null)
    {
        this.id = id;
        this.location_id = location_id;
        this.name = name;
        this.parent_scene_id = parent_scene_id;
        this.template_id = template_id;
        this.children_ids = children_ids;
    }
}
