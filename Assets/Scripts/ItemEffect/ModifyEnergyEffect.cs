/// <summary>
/// 修改精力物品效果
/// </summary>
public static class ModifyEnergyEffect
{
    public static void Execute(CharacterData character, float value)
    {
        character.energy += value;
        if (character.energy > character.max_energy)
        {
            character.energy = character.max_energy;
        }
        if (character.energy < 0f)
        {
            character.energy = 0f;
        }
    }
}
