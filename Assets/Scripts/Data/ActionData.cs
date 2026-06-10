using System.Collections.Generic;

/// <summary>
/// 行动数据——定义玩家在场景中可执行的一个操作
/// </summary>
public class ActionData
{
    // 行动ID
    public int id;
    // 行动名称（显示在按钮上的文字，如"交谈"、"购物"）
    public string name;
    // 效果类型列表——依次执行，下标与 effect_contexts 一一对应
    public List<ActionEffectTypeEnum> effect_types;
    // 效果参数列表——每个效果对应的参数
    public List<ActionEffectContext> effect_contexts;

    public ActionData(
        int id,
        string name,
        List<ActionEffectTypeEnum> effect_types = null,
        List<ActionEffectContext> effect_contexts = null
    )
    {
        this.id = id;
        this.name = name;
        this.effect_types = effect_types;
        this.effect_contexts = effect_contexts;
    }
}
