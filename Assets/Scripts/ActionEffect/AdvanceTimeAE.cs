using UnityEngine;

/// <summary>
/// 推进时间效果——将游戏时间推进指定数量的时段
/// </summary>
public static class AdvanceTimeAE
{
    /// <summary>
    /// 执行推进时间效果
    /// </summary>
    public static bool Execute(ActionEffectContext context)
    {
        if (context.advance_slots > 0)
        {
            TimeManager.Instance.AdvanceTime(context.advance_slots);
        }

        // 输出日志（如果有）
        if (!string.IsNullOrEmpty(context.log_message))
        {
            Debug.Log(context.log_message);
        }

        return true;
    }
}
