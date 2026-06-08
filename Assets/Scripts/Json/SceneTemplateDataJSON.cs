using System;
using System.Collections.Generic;

/// <summary>
/// JSON反序列化辅助类——JSON中一条场景模板条目
/// </summary>
[Serializable]
public class SceneTemplateDataJSON
{
    public int id;
    public string name;
    public List<int> action_ids; // 可选，无则为null
}

/// <summary>
/// JSON反序列化辅助类——JSON根容器
/// </summary>
[Serializable]
public class SceneTemplateDataListJSON
{
    public List<SceneTemplateDataJSON> templates;
}
