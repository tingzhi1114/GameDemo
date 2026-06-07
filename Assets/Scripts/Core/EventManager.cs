using UnityEngine;

/// <summary>
/// 中央事件管理器——订阅所有全局事件，统一刷新各UI面板
/// 被动触发的脚本（各Panel）对事件毫无感知，由EventManager集中调度
/// </summary>
public static class EventManager
{
    // 静态构造器——订阅所有事件
    static EventManager()
    {
        PanelAction.OnActionExecuted += OnActionExecuted;
        PanelMove.OnSceneChanged += OnSceneChanged;
        PanelMap.OnLocationChanged += OnLocationChanged;
    }

    private static void OnActionExecuted()
    {
        RefreshAllPanels();
    }

    private static void OnSceneChanged()
    {
        RefreshAllPanels();
    }

    private static void OnLocationChanged()
    {
        RefreshAllPanels();
    }

    // 遍历查找并刷新所有面板
    private static void RefreshAllPanels()
    {
        PanelPlayer panel_player = Object.FindObjectOfType<PanelPlayer>();
        if (panel_player != null) panel_player.Refresh();

        PanelTitle panel_title = Object.FindObjectOfType<PanelTitle>();
        if (panel_title != null) panel_title.Refresh();

        PanelAction panel_action = Object.FindObjectOfType<PanelAction>();
        if (panel_action != null) panel_action.Refresh();

        PanelCharacter panel_character = Object.FindObjectOfType<PanelCharacter>();
        if (panel_character != null) panel_character.Refresh();

        PanelMove panel_move = Object.FindObjectOfType<PanelMove>();
        if (panel_move != null) panel_move.Refresh();
    }
}
