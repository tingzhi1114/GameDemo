/// <summary>
/// 修改健康物品效果
/// </summary>
public static class ModifyHealthEffect
{
    public static void Execute(CharacterData character, float value)
    {
        character.health += value;
        if (character.health > character.max_health)
        {
            character.health = character.max_health;
        }
        if (character.health < 0f)
        {
            character.health = 0f;
        }
    }
}
