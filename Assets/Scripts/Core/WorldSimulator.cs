/// <summary>
/// 世界演化器——每推进一个时段时，对所有角色执行状态衰减
/// 由 EventManager 在 TimeManager.OnTimeAdvanced 触发时调用
/// </summary>
public static class WorldSimulator
{
    /// <summary>
    /// 每时段推进时调用，衰减所有角色的状态
    /// </summary>
    public static void OnTimeAdvanced()
    {
        foreach (CharacterData character in CharacterDictionary.Instance.GetAll())
        {
            // 精力每时段减少3
            character.energy -= 3f;
            if (character.energy < 0f)
            {
                character.energy = 0f;
            }

            // 饱腹每时段减少2
            character.fullness -= 2f;
            if (character.fullness < 0f)
            {
                character.fullness = 0f;
            }

            // 饱腹为0时，每时段健康减少2
            if (character.fullness <= 0f)
            {
                character.health -= 2f;
                if (character.health < 0f)
                {
                    character.health = 0f;
                }
            }
        }
    }
}
