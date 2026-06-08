using System;
using System.Collections.Generic;

/// <summary>
/// JSON反序列化辅助类——JSON中一条场景条目
/// </summary>
[Serializable]
public class SceneDataJSON
{
    public int id;
    public int location_id;
    public string name;
    public int parent_scene_id;     // -1 表示顶级场景
    public int template_id;         // -1 表示无模板
    public List<int> children_ids;  // 可选，无则为null
}

/// <summary>
/// JSON反序列化辅助类——JSON根容器
/// </summary>
[Serializable]
public class SceneDataListJSON
{
    public List<SceneDataJSON> scenes;
}
