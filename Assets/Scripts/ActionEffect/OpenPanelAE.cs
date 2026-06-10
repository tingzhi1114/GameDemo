using UnityEngine;

/// <summary>
/// 打开面板效果——按名称查找并激活指定的 UI 面板
/// 暂为框架预留，具体面板查找逻辑后续接入
/// </summary>
public static class OpenPanelAE
{
    /// <summary>
    /// 执行打开面板效果
    /// </summary>
    public static bool Execute(ActionEffectContext context)
    {
        // 按 panel_name 查找并激活面板
        // panel_name 对应场景中面板物体的名称
        if (!string.IsNullOrEmpty(context.panel_name))
        {
            GameObject panel = GameObject.Find(context.panel_name);
            if (panel != null)
            {
                panel.SetActive(true);
            }
        }

        // 输出日志（如果有）
        if (!string.IsNullOrEmpty(context.log_message))
        {
            Debug.Log(context.log_message);
        }

        return true;
    }
}
