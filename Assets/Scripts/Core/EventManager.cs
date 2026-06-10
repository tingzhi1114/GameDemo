using UnityEngine;

/// <summary>
/// 中央事件管理器——订阅所有全局事件，统一刷新各UI面板
/// 缓存各Panel引用，避免每次刷新都查找
/// </summary>
public static class EventManager
{
    // 缓存的Panel引用
    private static PanelPlayer panel_player;
    private static PanelTitle panel_title;
    private static PanelAction panel_action;
    private static PanelCharacter panel_character;
    private static PanelMove panel_move;

    /// <summary>
    /// 初始化——订阅全局事件，缓存面板引用
    /// </summary>
    public static void Init()
    {
        // 订阅事件
        PanelAction.OnActionExecuted += OnActionExecuted;
        PanelMove.OnSceneChanged += OnSceneChanged;
        PanelMap.OnLocationChanged += OnLocationChanged;
        PanelInventory.OnItemUsed += OnItemUsed;

        // 缓存面板引用
        CachePanels();
    }

    // 查找并缓存所有面板
    private static void CachePanels()
    {
        panel_player = Object.FindObjectOfType<PanelPlayer>();
        panel_title = Object.FindObjectOfType<PanelTitle>();
        panel_action = Object.FindObjectOfType<PanelAction>();
        panel_character = Object.FindObjectOfType<PanelCharacter>();
        panel_move = Object.FindObjectOfType<PanelMove>();
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

    private static void OnItemUsed()
    {
        RefreshAllPanels();
    }

    // 刷新所有已缓存的Panel
    private static void RefreshAllPanels()
    {
        if (panel_player != null) panel_player.Refresh();
        if (panel_title != null) panel_title.Refresh();
        if (panel_action != null) panel_action.Refresh();
        if (panel_character != null) panel_character.Refresh();
        if (panel_move != null) panel_move.Refresh();
    }
}
