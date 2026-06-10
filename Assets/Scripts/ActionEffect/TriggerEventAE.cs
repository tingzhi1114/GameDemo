using UnityEngine;

/// <summary>
/// 触发事件效果——按事件 ID 触发对应的游戏事件
/// 暂为框架预留，事件系统接入后实现具体逻辑
/// </summary>
public static class TriggerEventAE
{
    /// <summary>
    /// 执行触发事件效果
    /// </summary>
    public static bool Execute(ActionEffectContext context)
    {
        // 触发指定事件ID，后续事件系统接入后实现
        if (context.event_id > 0)
        {
            Debug.Log("触发事件：" + context.event_id + "（事件系统未接入）");
        }

        // 输出日志（如果有）
        if (!string.IsNullOrEmpty(context.log_message))
        {
            Debug.Log(context.log_message);
        }

        return true;
    }
}
