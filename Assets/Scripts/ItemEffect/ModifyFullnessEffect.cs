/// <summary>
/// 修改饱腹物品效果
/// </summary>
public static class ModifyFullnessEffect
{
    public static void Execute(CharacterData character, float value)
    {
        character.fullness += value;
        if (character.fullness > character.max_fullness)
        {
            character.fullness = character.max_fullness;
        }
        if (character.fullness < 0f)
        {
            character.fullness = 0f;
        }
    }
}
