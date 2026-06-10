using UnityEngine;

/// <summary>
/// 修改金钱效果——增加或减少角色的金钱
/// </summary>
public static class ModifyMoneyAE
{
    /// <summary>
    /// 执行修改金钱效果
    /// </summary>
    public static bool Execute(CharacterData character, ActionEffectContext context)
    {
        // 如果钱不够扣，提示并返回失败
        if (context.money_change < 0 && character.money < -context.money_change)
        {
            Debug.Log(character.name + "囊中羞涩，钱不够。");
            return false;
        }

        character.money += context.money_change;

        // 输出日志（如果有）
        if (!string.IsNullOrEmpty(context.log_message))
        {
            Debug.Log(context.log_message);
        }

        return true;
    }
}
