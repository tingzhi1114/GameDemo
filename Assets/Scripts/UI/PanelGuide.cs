using UnityEngine;
using TMPro;

/// <summary>
/// 任务指引面板——固定显示占位文字，后续任务系统再联动
/// </summary>
public class PanelGuide : MonoBehaviour
{
    private TextMeshProUGUI text_guide;

    private void Awake()
    {
        Transform button_guide = this.transform.Find("Button_Guide");
        if (button_guide != null)
        {
            text_guide = button_guide.Find("Text_Guide").GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
    }

    /// <summary>
    /// 刷新指引（后续任务系统接入后再实现）
    /// </summary>
    public void Refresh()
    {
    }

    private void OnDestroy()
    {
    }
}
