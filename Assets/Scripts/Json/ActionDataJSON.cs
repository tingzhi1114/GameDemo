using System;
using System.Collections.Generic;

/// <summary>
/// JSON反序列化辅助类——JSON中一条行动条目
/// </summary>
[Serializable]
public class ActionDataJSON
{
    public int id;
    public string name;
}

/// <summary>
/// JSON反序列化辅助类——JSON根容器
/// </summary>
[Serializable]
public class ActionDataListJSON
{
    public List<ActionDataJSON> actions;
}
