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
