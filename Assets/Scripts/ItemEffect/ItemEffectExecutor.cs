/// <summary>
/// 物品效果执行器——根据效果类型分派到对应的效果类执行
/// </summary>
public static class ItemEffectExecutor
{
    /// <summary>
    /// 执行一个物品效果
    /// </summary>
    public static void Execute(CharacterData character, ItemEffectType type, float value)
    {
        if (type == ItemEffectType.ModifyMoney)
        {
            ModifyMoneyEffect.Execute(character, value);
        }
        else if (type == ItemEffectType.ModifyHealth)
        {
            ModifyHealthEffect.Execute(character, value);
        }
        else if (type == ItemEffectType.ModifyEnergy)
        {
            ModifyEnergyEffect.Execute(character, value);
        }
        else if (type == ItemEffectType.ModifyFullness)
        {
            ModifyFullnessEffect.Execute(character, value);
        }
        else
        {
            // 六维属性
            ModifyAttributeEffect.Execute(character, type, value);
        }
    }
}
