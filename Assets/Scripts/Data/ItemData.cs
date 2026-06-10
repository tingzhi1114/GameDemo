using System.Collections.Generic;

/// <summary>
/// 物品数据模型——存储一个物品的核心信息
/// </summary>
public class ItemData
{
    // 物品唯一ID
    public int id;
    // 物品名称
    public string name;
    // 物品描述
    public string description;
    // 物品类型
    public ItemTypeEnum type;
    // 品级（1=仙一品 ~ 9=劣九品）
    public int grade;
    // 基础价格（买入/卖出的基准价）
    public int base_value;
    // 最大堆叠数量（1=不可堆叠）
    public int max_stack;
    // 物品特性字典（如攻击力、耐久度、知识量等，暂空）
    public Dictionary<ItemPropertyType, float> properties;
    // 物品效果字典（使用物品时触发的效果列表）
    public Dictionary<ItemEffectType, float> effects;
    // 是否可使用
    public bool can_use;
    // 是否可丢弃
    public bool can_discard;
    // 是否可交易
    public bool can_trade;

    /// <summary>
    /// 构造一个物品
    /// </summary>
    public ItemData(
        int id,
        string name,
        string description,
        ItemTypeEnum type,
        int grade,
        int base_value,
        int max_stack,
        Dictionary<ItemPropertyType, float> properties,
        Dictionary<ItemEffectType, float> effects,
        bool can_use = false,
        bool can_discard = false,
        bool can_trade = false
    )
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.type = type;
        this.grade = grade;
        this.base_value = base_value;
        this.max_stack = max_stack;
        this.properties = properties;
        this.effects = effects;
        this.can_use = can_use;
        this.can_discard = can_discard;
        this.can_trade = can_trade;
    }

    /// <summary>
    /// 深拷贝当前物品——properties和effects字典各为独立副本
    /// </summary>
    public ItemData Clone()
    {
        // 复制 properties 字典
        Dictionary<ItemPropertyType, float> prop_copy = null;
        if (this.properties != null)
        {
            prop_copy = new Dictionary<ItemPropertyType, float>();
            foreach (KeyValuePair<ItemPropertyType, float> kv in this.properties)
            {
                prop_copy[kv.Key] = kv.Value;
            }
        }

        // 复制 effects 字典
        Dictionary<ItemEffectType, float> effect_copy = null;
        if (this.effects != null)
        {
            effect_copy = new Dictionary<ItemEffectType, float>();
            foreach (KeyValuePair<ItemEffectType, float> kv in this.effects)
            {
                effect_copy[kv.Key] = kv.Value;
            }
        }

        return new ItemData(
            id: this.id,
            name: this.name,
            description: this.description,
            type: this.type,
            grade: this.grade,
            base_value: this.base_value,
            max_stack: this.max_stack,
            properties: prop_copy,
            effects: effect_copy,
            can_use: this.can_use,
            can_discard: this.can_discard,
            can_trade: this.can_trade
        );
    }
}
