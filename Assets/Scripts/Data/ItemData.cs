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
        Dictionary<ItemEffectType, float> effects
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
    }
}
