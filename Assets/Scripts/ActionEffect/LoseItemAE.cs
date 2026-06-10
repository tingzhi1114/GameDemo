using UnityEngine;

/// <summary>
/// 失去物品效果——从角色背包中移除指定数量和ID的物品
/// </summary>
public static class LoseItemAE
{
    /// <summary>
    /// 执行失去物品效果
    /// </summary>
    public static bool Execute(CharacterData character, ActionEffectContext context)
    {
        character.RemoveItem(context.item_id, context.item_count);

        // 输出日志（如果有）
        if (!string.IsNullOrEmpty(context.log_message))
        {
            Debug.Log(context.log_message);
        }

        return true;
    }
}
