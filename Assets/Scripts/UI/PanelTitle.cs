using UnityEngine;
using TMPro;

/// <summary>
/// 顶部标题面板——显示时间与当前位置
/// </summary>
public class PanelTitle : MonoBehaviour
{
    private TextMeshProUGUI text_time;
    private TextMeshProUGUI text_position;

    private void Awake()
    {
        // 通过Transform查找子物体
        Transform panel = this.transform;
        text_time = panel.Find("Image_Time/Text_Time").GetComponent<TextMeshProUGUI>();
        text_position = panel.Find("Image_Position/Text_Position").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Refresh();
    }

    /// <summary>
    /// 刷新时间与地点显示
    /// </summary>
    public void Refresh()
    {
        // 更新时间
        text_time.text = TimeManager.Instance.GetDateString();

        // 更新位置
        CharacterData player = Player.Instance.GetCharacter();
        if (player != null)
        {
            if (player.current_scene_id >= 0)
            {
                SceneData scene = SceneDictionary.Instance.Get(player.current_scene_id);
                if (scene != null)
                {
                    if (scene.parent_scene_id >= 0)
                    {
                        // 子场景显示 "顶级场景·子场景"
                        SceneData parent_scene = SceneDictionary.Instance.Get(scene.parent_scene_id);
                        if (parent_scene != null)
                        {
                            text_position.text = parent_scene.name + "·" + scene.name;
                        }
                        else
                        {
                            text_position.text = scene.name;
                        }
                    }
                    else
                    {
                        // 顶级场景直接显示名称
                        text_position.text = scene.name;
                    }
                }
            }
            else
            {
                // 在大地图显示地点名
                LocationData location = LocationDictionary.Instance.Get(player.current_location_id);
                if (location != null)
                {
                    text_position.text = location.name;
                }
            }
        }
    }
}
