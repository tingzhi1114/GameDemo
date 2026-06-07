using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 大地图面板——显示所有地点及道路连接
/// 默认未激活，离开顶级场景时打开
/// </summary>
public class PanelMap : MonoBehaviour
{
    /// <summary>
    /// 地点切换后触发（从大地图切换到其他城市/乡村等），EventManager订阅此事件
    /// </summary>
    public static event System.Action OnLocationChanged;

    private Button button_close;

    private void Awake()
    {
        // 绑定关闭按钮
        button_close = this.transform.Find("Button_Close")?.GetComponent<Button>();
        if (button_close != null)
        {
            button_close.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });
        }
    }

    private void OnEnable()
    {
        Refresh();
        BindLocationButtons();
    }

    /// <summary>
    /// 绑定各地点的点击事件——进入对应地点的顶级场景
    /// </summary>
    private void BindLocationButtons()
    {
        Transform image_map = this.transform.Find("Image_Map");
        if (image_map == null)
        {
            return;
        }

        CharacterData player = Player.Instance.Get();
        if (player == null)
        {
            return;
        }

        // 遍历所有子物体，找到地点按钮
        for (int i = 0; i < image_map.childCount; i++)
        {
            Transform child = image_map.GetChild(i);
            if (!child.name.StartsWith("Button_Location_"))
            {
                continue;
            }

            TextMeshProUGUI text_tmp = child.Find("Text")?.GetComponent<TextMeshProUGUI>();
            if (text_tmp == null)
            {
                continue;
            }

            Button button = child.GetComponent<Button>();
            if (button == null)
            {
                continue;
            }

            // 清空旧监听，绑定新监听
            button.onClick.RemoveAllListeners();
            string location_name = text_tmp.text;
            button.onClick.AddListener(() =>
            {
                OnLocationClicked(location_name);
            });
        }
    }

    // 点击某个地点按钮
    private void OnLocationClicked(string location_name)
    {
        // 根据名称查找地点
        LocationData target_location = null;
        foreach (LocationData loc in LocationDictionary.Instance.GetAll())
        {
            if (loc.name == location_name)
            {
                target_location = loc;
                break;
            }
        }

        if (target_location == null)
        {
            return;
        }

        CharacterData player = Player.Instance.Get();
        if (player == null)
        {
            return;
        }

        // 如果点击的是当前所在地点，不做任何事（不推进时间、不切换）
        if (target_location.id == player.current_location_id)
        {
            return;
        }

        // 计算路径长度并推进时间（connections的数值直接代表时段数）
        LocationData current_location = LocationDictionary.Instance.Get(player.current_location_id);
        int distance = -1;
        if (current_location != null)
        {
            distance = current_location.GetConnectionLength(target_location.id);
        }
        if (distance > 0)
        {
            TimeManager.Instance.AdvanceTime(distance);
        }

        // 切换到目标地点
        player.current_location_id = target_location.id;
        player.current_scene_id = target_location.top_scene_id;

        // 关闭大地图
        this.gameObject.SetActive(false);

        // 触发地点切换事件（EventManager订阅后刷新UI）
        OnLocationChanged?.Invoke();
    }

    /// <summary>
    /// 刷新大地图显示
    /// </summary>
    public void Refresh()
    {
        Transform image_map = this.transform.Find("Image_Map");
        if (image_map == null)
        {
            return;
        }

        CharacterData player = Player.Instance.Get();
        if (player == null)
        {
            return;
        }

        // 遍历所有地点按钮，高亮当前所在地
        for (int i = 0; i < image_map.childCount; i++)
        {
            Transform child = image_map.GetChild(i);
            if (!child.name.StartsWith("Button_Location_"))
            {
                continue;
            }

            TextMeshProUGUI text_tmp = child.Find("Text")?.GetComponent<TextMeshProUGUI>();
            if (text_tmp == null)
            {
                continue;
            }

            // 根据地点名称判断是否为当前所在地
            LocationData location = LocationDictionary.Instance.Get(player.current_location_id);
            if (location != null && text_tmp.text == location.name)
            {
                // 当前所在地：蓝色
                text_tmp.color = Color.blue;
            }
            else
            {
                // 非当前所在地：恢复黑色
                text_tmp.color = Color.black;
            }
        }
    }
}
