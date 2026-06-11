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

    // 是否已初始化，防止重复订阅
    private static bool has_initialized;

    /// <summary>
    /// 初始化——订阅全局事件，缓存面板引用
    /// </summary>
    public static void Init()
    {
        if (has_initialized)
        {
            return;
        }
        has_initialized = true;

        // 订阅事件
        PanelAction.OnActionExecuted += OnActionExecuted;
        PanelMove.OnSceneChanged += OnSceneChanged;
        PanelMap.OnLocationChanged += OnLocationChanged;
        PanelInventory.OnItemUsed += OnItemUsed;
        PanelTrade.OnTradeExecuted += OnTradeExecuted;
        TimeManager.Instance.OnTimeAdvanced += OnTimeAdvanced;

        // 缓存面板引用
        CachePanels();
    }

    /// <summary>
    /// 取消所有事件订阅，清理状态（场景切换或重启时调用）
    /// </summary>
    public static void Shutdown()
    {
        if (!has_initialized)
        {
            return;
        }

        PanelAction.OnActionExecuted -= OnActionExecuted;
        PanelMove.OnSceneChanged -= OnSceneChanged;
        PanelMap.OnLocationChanged -= OnLocationChanged;
        PanelInventory.OnItemUsed -= OnItemUsed;
        PanelTrade.OnTradeExecuted -= OnTradeExecuted;
        TimeManager.Instance.OnTimeAdvanced -= OnTimeAdvanced;

        panel_player = null;
        panel_title = null;
        panel_action = null;
        panel_character = null;
        panel_move = null;

        has_initialized = false;
    }

    // 查找并缓存所有面板
    private static void CachePanels()
    {
        panel_player = Object.FindObjectOfType<PanelPlayer>(true);
        panel_title = Object.FindObjectOfType<PanelTitle>(true);
        panel_action = Object.FindObjectOfType<PanelAction>(true);
        panel_character = Object.FindObjectOfType<PanelCharacter>(true);
        panel_move = Object.FindObjectOfType<PanelMove>(true);
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

    private static void OnTradeExecuted()
    {
        RefreshAllPanels();
    }

    private static void OnTimeAdvanced()
    {
        // 世界演化：角色状态衰减
        WorldSimulator.OnTimeAdvanced();

        // 刷新所有面板
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
