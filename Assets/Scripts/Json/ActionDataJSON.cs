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
    // 可选的效果列表
    public List<ActionEffectJSON> effects;
}

/// <summary>
/// JSON反序列化辅助类——JSON中一条行动效果
/// </summary>
[Serializable]
public class ActionEffectJSON
{
    public string type;
    public int money_change;
    public float health_change;
    public float energy_change;
    public float fullness_change;
    public int item_id;
    public int item_count;
    public string panel_name;
    public int event_id;
    public int advance_slots;
    public bool trade_is_buy;
    public string log_message;
    public List<AttributeChangeJSON> attribute_changes;
}

/// <summary>
/// JSON反序列化辅助类——JSON中一条属性变化条目
/// </summary>
[Serializable]
public class AttributeChangeJSON
{
    public string type;
    public float value;
}

/// <summary>
/// JSON反序列化辅助类——JSON根容器
/// </summary>
[Serializable]
public class ActionDataListJSON
{
    public List<ActionDataJSON> actions;
}
