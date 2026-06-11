using System.Collections.Generic;

/// <summary>
/// 行动效果参数——存储单个行动效果所需的全部参数
/// 不同 ActionEffectTypeEnum 使用不同的字段组合
/// </summary>
public class ActionEffectContext
{
    // ModifyMoney 参数：金钱变化量（正数=增加，负数=减少）
    public int money_change;

    // ModifyAttribute 参数：六维属性变化字典（key=属性类型, value=变化量）
    public Dictionary<AttributeTypeEnum, float> attribute_changes;

    // ModifyStatus 参数：三种状态的变化量
    public float health_change;
    public float energy_change;
    public float fullness_change;

    // GainItem / LoseItem 参数：物品ID和数量
    public int item_id;
    public int item_count;

    // OpenPanel 参数：面板名称（用于 Find 查找）
    public string panel_name;

    // TriggerEvent 参数：事件ID
    public int event_id;

    // AdvanceTime 参数：推进的时段数（正数=向后推进）
    public int advance_slots;

    // OpenPanel 交易相关参数：true=买入模式, false=卖出模式
    public bool trade_is_buy;

    // 执行成功后输出的日志文字（可选，为空则不输出）
    public string log_message;

    public ActionEffectContext()
    {
    }
}
