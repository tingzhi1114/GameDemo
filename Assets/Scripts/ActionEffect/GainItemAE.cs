using UnityEngine;

/// <summary>
/// 获得物品效果——向角色背包中添加指定数量和ID的物品
/// </summary>
public static class GainItemAE
{
    /// <summary>
    /// 执行获得物品效果
    /// </summary>
    public static bool Execute(CharacterData character, ActionEffectContext context)
    {
        InventoryDictionary.Instance.GetOrCreate(character.id).AddItem(context.item_id, context.item_count);

        // 输出日志（如果有）
        if (!string.IsNullOrEmpty(context.log_message))
        {
            Debug.Log(context.log_message);
        }

        return true;
    }
}
