using System.Collections.Generic;
using UnityEngine;

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
    // 物品子类型
    public ItemSubTypeEnum sub_type;
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
    // 弹性系数（价格受库存影响的敏感度，值越大价格波动越剧烈）
    public float elasticity;

    /// <summary>
    /// 构造一个物品
    /// </summary>
    public ItemData(
        int id,
        string name,
        string description,
        ItemTypeEnum type,
        ItemSubTypeEnum sub_type,
        int grade,
        int base_value,
        int max_stack,
        Dictionary<ItemPropertyType, float> properties,
        Dictionary<ItemEffectType, float> effects,
        bool can_use = false,
        bool can_discard = false,
        bool can_trade = false,
        float elasticity = 0f
    )
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.type = type;
        this.sub_type = sub_type;
        this.grade = grade;
        this.base_value = base_value;
        this.max_stack = max_stack;
        this.properties = properties;
        this.effects = effects;
        this.can_use = can_use;
        this.can_discard = can_discard;
        this.can_trade = can_trade;
        this.elasticity = elasticity;
    }

    /// <summary>
    /// 根据物品子类型获取对应的大类
    /// </summary>
    public static ItemTypeEnum GetMainType(ItemSubTypeEnum subType)
    {
        // 消耗品子类
        if (subType == ItemSubTypeEnum.Food
            || subType == ItemSubTypeEnum.Drink
            || subType == ItemSubTypeEnum.Medicine
            || subType == ItemSubTypeEnum.Tonic)
        {
            return ItemTypeEnum.Consumable;
        }

        // 器具子类
        if (subType == ItemSubTypeEnum.FarmTool
            || subType == ItemSubTypeEnum.FishTool
            || subType == ItemSubTypeEnum.Cookware
            || subType == ItemSubTypeEnum.HandTool)
        {
            return ItemTypeEnum.Tool;
        }

        // 交易品子类
        if (subType == ItemSubTypeEnum.Grain
            || subType == ItemSubTypeEnum.Cloth
            || subType == ItemSubTypeEnum.Lumber
            || subType == ItemSubTypeEnum.Ore
            || subType == ItemSubTypeEnum.Herb
            || subType == ItemSubTypeEnum.Tea
            || subType == ItemSubTypeEnum.Porcelain
            || subType == ItemSubTypeEnum.Jewel)
        {
            return ItemTypeEnum.TradeGood;
        }

        return ItemTypeEnum.Special;
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
            sub_type: this.sub_type,
            grade: this.grade,
            base_value: this.base_value,
            max_stack: this.max_stack,
            properties: prop_copy,
            effects: effect_copy,
            can_use: this.can_use,
            can_discard: this.can_discard,
            can_trade: this.can_trade,
            elasticity: this.elasticity
        );
    }

    /// <summary>
    /// 根据当前库存、期望库存、生产力、Z1（邻居）、Z2（两步邻居）计算动态价格
    /// 公式：B = 1 - (p-0.5)×0.4 - (Z1×0.3+Z2×0.1)×0.4
    ///       p_mult = B × exp(γ × (1-r) × e)，clamp [0.2, 3.0]
    ///       Buy = Base × p_mult
    /// </summary>
    public int GetPrice(int stock, int target, float productivity, float z1 = 0f, float z2 = 0f)
    {
        if (target <= 0)
        {
            return PanelTrade.ClampPrice(this.base_value, this.base_value);
        }

        // ratio = 库存/期望库存，限定[0.3, 3.0]
        float ratio = (float)stock / (float)target;
        if (ratio < 0.3f) ratio = 0.3f;
        if (ratio > 3.0f) ratio = 3.0f;

        // 弹性系数受 Config 上下限限制
        float e = this.elasticity;
        if (e < Config.Instance.elasticity_min) e = Config.Instance.elasticity_min;
        if (e > Config.Instance.elasticity_max) e = Config.Instance.elasticity_max;

        Config cfg = Config.Instance;

        // 静态地缘基准系数 B = 1 - (p-0.5)×0.4 - (Z1×0.3+Z2×0.1)×0.4
        float B = 1f - (productivity - 0.5f) * 0.4f - (z1 * 0.3f + z2 * 0.1f) * 0.4f;

        // 最终价格系数 p_mult = B × exp(γ × (1-r) × e)，clamp [0.2, 3.0]
        float exponent = cfg.gamma * (1f - ratio) * e;
        float p_mult = B * Mathf.Exp(exponent);
        if (p_mult < 0.2f) p_mult = 0.2f;
        if (p_mult > cfg.price_cap_ratio) p_mult = cfg.price_cap_ratio;

        int price = Mathf.RoundToInt(this.base_value * p_mult);
        return PanelTrade.ClampPrice(price, this.base_value);
    }
}
