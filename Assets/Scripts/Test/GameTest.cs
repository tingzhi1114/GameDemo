using UnityEngine;

/// <summary>
/// 游戏启动器——初始化所有数据并设置玩家初始位置
/// 后续会被正式的GameManager替代
/// </summary>
public class GameTest : MonoBehaviour
{
    private void Awake()
    {
        // 设置玩家ID
        Player.Instance.character_id = 1;

        Debug.Log("GameTest: 游戏初始化完成");
    }

    private void Start()
    {
        // 玩家进入锦安城顶级场景
        CharacterData player = Player.Instance.Get();
        if (player != null)
        {
            player.current_location_id = 1;
            player.current_scene_id = 1;
        }

        // 首次刷新所有面板
        PanelTitle panel_title = FindObjectOfType<PanelTitle>();
        if (panel_title != null)
        {
            panel_title.Refresh();
        }

        PanelPlayer panel_player = FindObjectOfType<PanelPlayer>();
        if (panel_player != null)
        {
            panel_player.Refresh();
        }

        PanelAction panel_action = FindObjectOfType<PanelAction>();
        if (panel_action != null)
        {
            panel_action.Refresh();
        }

        PanelMove panel_move = FindObjectOfType<PanelMove>();
        if (panel_move != null)
        {
            panel_move.Refresh();
        }

        PanelCharacter panel_character = FindObjectOfType<PanelCharacter>();
        if (panel_character != null)
        {
            panel_character.Refresh();
        }

        Debug.Log("玩家已进入锦安城");
    }
}
