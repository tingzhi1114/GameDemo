/// <summary>
/// 修改金钱物品效果
/// </summary>
public static class ModifyMoneyEffect
{
    public static void Execute(CharacterData character, float value)
    {
        character.money += (int)value;
    }
}
