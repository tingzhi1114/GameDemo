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
    public List<int> action_ids;    // 可选，无则为null
    public List<string> trade_types; // 可选，允许交易的物品类型名列表
}

/// <summary>
/// JSON反序列化辅助类——JSON根容器
/// </summary>
[Serializable]
public class SceneTemplateDataListJSON
{
    public List<SceneTemplateDataJSON> templates;
}
