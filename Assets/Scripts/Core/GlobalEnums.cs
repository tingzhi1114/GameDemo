/// <summary>
/// 全局枚举定义
/// </summary>

// 时段（一天分为6个时段）
public enum TimePeriodEnum
{
    DeadOfNight,    // 深夜
    EarlyMorning,   // 凌晨
    Morning,        // 上午
    Noon,           // 中午
    Afternoon,      // 下午
    Evening         // 晚上
}

// 地点类型
public enum LocationTypeEnum
{
    City,       // 城
    Village,    // 村
    Mountain,   // 山
    Forest,     // 林
    Lake,       // 湖
    Plain       // 原
}

// 属性类型
public enum AttributeTypeEnum
{
    Strength,   // 力量
    Agility,    // 敏捷
    Wit,        // 才智
    Charm,      // 魅力
    Physique,   // 体魄
    Luck        // 气运
}

// 物品类型（8大类）
public enum ItemTypeEnum
{
    Consumable,     // 消耗品
    Tool,           // 器具
    Clothing,       // 衣物
    Book,           // 书籍
    Collectible,    // 收集品
    Material,       // 材料
    TradeGood,      // 交易品
    Special         // 特殊
}

// 物品特性类型（暂空，后续按需追加枚举值）
public enum ItemPropertyType
{
}

// 物品子类型（按需扩展）
public enum ItemSubTypeEnum
{
    Food,       // 食物
    Drink,      // 饮品
    Medicine,   // 药品
    Tonic,      // 补品
    FarmTool,   // 农具
    FishTool,   // 渔具
    Cookware,   // 炊具
    HandTool,   // 工具
    Grain,      // 粮
    Cloth,      // 布
    Lumber,     // 木材
    Ore,        // 矿产
    Herb,       // 药材
    Tea,        // 茶
    Porcelain,  // 瓷器
    Jewel       // 珠宝
}

// 行动效果类型（行动可执行的各类原子效果）
public enum ActionEffectTypeEnum
{
    ModifyMoney,        // 修改金钱
    ModifyAttribute,    // 修改六维属性
    ModifyStatus,       // 修改状态（健康/精力/饱腹）
    GainItem,           // 获得物品
    LoseItem,           // 失去物品
    OpenPanel,          // 打开面板
    TriggerEvent,       // 触发事件
    AdvanceTime         // 推进时间
}

// 物品效果类型（使用物品时执行的各项效果）
public enum ItemEffectType
{
    ModifyMoney,        // 增减金钱
    ModifyHealth,       // 增减健康
    ModifyEnergy,       // 增减精力
    ModifyFullness,     // 增减饱腹
    ModifyStrength,     // 增减力量
    ModifyAgility,      // 增减敏捷
    ModifyWit,          // 增减才智
    ModifyCharm,        // 增减魅力
    ModifyPhysique,     // 增减体魄
    ModifyLuck          // 增减气运
}
