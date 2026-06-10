using System.Collections.Generic;

/// <summary>
/// 修改六维属性效果——增减角色的力量/敏捷/才智等属性
/// </summary>
public static class ModifyAttributeAE
{
    /// <summary>
    /// 执行修改属性效果
    /// </summary>
    public static bool Execute(CharacterData character, ActionEffectContext context)
    {
        if (context.attribute_changes == null)
        {
            return true;
        }

        // 遍历属性变化字典，逐一应用
        foreach (KeyValuePair<AttributeTypeEnum, float> kv in context.attribute_changes)
        {
            character.ModifyAttribute(kv.Key, kv.Value);
        }

        // 输出日志（如果有）
        if (!string.IsNullOrEmpty(context.log_message))
        {
            UnityEngine.Debug.Log(context.log_message);
        }

        return true;
    }
}
