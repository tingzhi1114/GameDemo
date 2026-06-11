using System;
using System.Collections.Generic;

/// <summary>
/// JSON反序列化辅助类——JSON中一条效果条目
/// </summary>
[Serializable]
public class ItemEffectJSON
{
    public string type; // 字符串，解析时转为 ItemEffectType
    public float value;
}

/// <summary>
/// JSON反序列化辅助类——JSON中一条特性条目
/// </summary>
[Serializable]
public class ItemPropertyJSON
{
    public string type; // 字符串，解析时转为 ItemPropertyType
    public float value;
}

/// <summary>
/// JSON反序列化辅助类——JSON中一条物品条目
/// </summary>
[Serializable]
public class ItemDataJSON
{
    public int id;
    public string name;
    public string description;
    public string type;                     // 字符串，解析时转为 ItemTypeEnum
    public string sub_type;                 // 字符串，解析时转为 ItemSubTypeEnum
    public int grade;
    public int base_value;
    public int max_stack;
    public List<ItemPropertyJSON> properties; // 可选，无则为null
    public List<ItemEffectJSON> effects;      // 可选，无则为null
    public bool can_use;
    public bool can_discard;
    public bool can_trade;
}

/// <summary>
/// JSON反序列化辅助类——JSON根容器
/// </summary>
[Serializable]
public class ItemDataListJSON
{
    public List<ItemDataJSON> items;
}
