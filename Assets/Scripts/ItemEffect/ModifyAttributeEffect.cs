using System.Collections.Generic;

/// <summary>
/// 修改六维属性物品效果
/// </summary>
public static class ModifyAttributeEffect
{
    // 属性类型字典（ItemEffectType → AttributeTypeEnum）
    private static Dictionary<ItemEffectType, AttributeTypeEnum> attr_map = new Dictionary<ItemEffectType, AttributeTypeEnum>
    {
        { ItemEffectType.ModifyStrength, AttributeTypeEnum.Strength },
        { ItemEffectType.ModifyAgility, AttributeTypeEnum.Agility },
        { ItemEffectType.ModifyWit, AttributeTypeEnum.Wit },
        { ItemEffectType.ModifyCharm, AttributeTypeEnum.Charm },
        { ItemEffectType.ModifyPhysique, AttributeTypeEnum.Physique },
        { ItemEffectType.ModifyLuck, AttributeTypeEnum.Luck },
    };

    /// <summary>
    /// 执行修改属性效果
    /// </summary>
    public static void Execute(CharacterData character, ItemEffectType type, float value)
    {
        if (attr_map.ContainsKey(type))
        {
            character.ModifyAttribute(attr_map[type], value);
        }
    }
}
