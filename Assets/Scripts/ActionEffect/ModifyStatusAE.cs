using UnityEngine;

/// <summary>
/// 修改状态效果——增减角色的健康/精力/饱腹值
/// </summary>
public static class ModifyStatusAE
{
    /// <summary>
    /// 执行修改状态效果
    /// </summary>
    public static bool Execute(CharacterData character, ActionEffectContext context)
    {
        // 健康值变化
        if (context.health_change != 0f)
        {
            character.health += context.health_change;
            if (character.health > character.max_health)
            {
                character.health = character.max_health;
            }
            if (character.health < 0f)
            {
                character.health = 0f;
            }
        }

        // 精力值变化
        if (context.energy_change != 0f)
        {
            character.energy += context.energy_change;
            if (character.energy > character.max_energy)
            {
                character.energy = character.max_energy;
            }
            if (character.energy < 0f)
            {
                character.energy = 0f;
            }
        }

        // 饱腹值变化
        if (context.fullness_change != 0f)
        {
            character.fullness += context.fullness_change;
            if (character.fullness > character.max_fullness)
            {
                character.fullness = character.max_fullness;
            }
            if (character.fullness < 0f)
            {
                character.fullness = 0f;
            }
        }

        // 输出日志（如果有）
        if (!string.IsNullOrEmpty(context.log_message))
        {
            Debug.Log(context.log_message);
        }

        return true;
    }
}
