using UnityEngine;

/// <summary>
/// 打开面板效果——按名称查找并激活指定的 UI 面板
/// 当 panel_name 为 "Panel_Trade" 时，自动调起交易面板的买入/卖出模式
/// </summary>
public static class OpenPanelAE
{
    /// <summary>
    /// 执行打开面板效果
    /// </summary>
    public static bool Execute(ActionEffectContext context, CharacterData character = null)
    {
        if (string.IsNullOrEmpty(context.panel_name))
        {
            // 输出日志（如果有）
            if (!string.IsNullOrEmpty(context.log_message))
            {
                Debug.Log(context.log_message);
            }
            return true;
        }

        // 特殊处理：打开交易面板
        if (context.panel_name == "Panel_Trade")
        {
            PanelTrade panel = Object.FindObjectOfType<PanelTrade>(true);
            if (panel == null)
            {
                Debug.LogError("Panel_Trade 未找到，请检查场景中是否存在该面板");
                return false;
            }

            if (character == null)
            {
                Debug.LogError("打开交易面板需要传入角色数据");
                return false;
            }

            if (context.trade_is_buy)
            {
                panel.ShowBuyPanel(character.current_scene_id);
            }
            else
            {
                panel.ShowSellPanel(character.current_scene_id, Config.Instance.sell_rate);
            }

            return true;
        }

        // 普通面板：按名称查找并激活
        GameObject target = GameObject.Find(context.panel_name);
        if (target != null)
        {
            target.SetActive(true);
        }

        // 输出日志（如果有）
        if (!string.IsNullOrEmpty(context.log_message))
        {
            Debug.Log(context.log_message);
        }

        return true;
    }
}
