using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 批量处理弹窗——用于背包（使用/丢弃）和交易（购买/出售）的批量数量调整
/// </summary>
public class PanelBatchProcess : MonoBehaviour
{
    // 缓存组件
    private TextMeshProUGUI text_title;
    private Scrollbar scrollbar;
    private TextMeshProUGUI text_num;
    private TextMeshProUGUI text_another;
    private Button button_confirm;
    private Button button_close;
    private Button button_add;
    private Button button_reduce;

    // 当前最大数量和确认回调
    private int max_count;
    private int current_count;
    private System.Action<int> on_confirm;
    // 动态信息构建器：接收当前数量，返回要显示在 Text_Another 中的文本
    private System.Func<int, string> another_builder;
    // 懒加载标记
    private bool initialized;

    private void Awake()
    {
        Initialize();
    }

    // 查找组件（对象非激活时 Awake 不会被调用，所以 Show 中也会触发）
    private void Initialize()
    {
        if (initialized) return;
        initialized = true;

        // 递归查找子组件（兼容多层嵌套结构）
        text_title = FindDeepChild<TextMeshProUGUI>(this.transform, "Text_Title");
        scrollbar = FindDeepChild<Scrollbar>(this.transform, "Scrollbar_BatchProcess");
        text_num = FindDeepChild<TextMeshProUGUI>(this.transform, "Text_Num");
        text_another = FindDeepChild<TextMeshProUGUI>(this.transform, "Text_Another");
        button_confirm = FindDeepChild<Button>(this.transform, "Button_Confirm");
        button_close = FindDeepChild<Button>(this.transform, "Button_Close");
        button_add = FindDeepChild<Button>(this.transform, "Button_Add");
        button_reduce = FindDeepChild<Button>(this.transform, "Button_Reduce");

        if (button_close != null)
        {
            button_close.onClick.AddListener(() => { this.gameObject.SetActive(false); });
        }
        if (button_confirm != null)
        {
            button_confirm.onClick.AddListener(OnConfirmClicked);
        }
        if (button_add != null)
        {
            button_add.onClick.AddListener(OnAddClicked);
        }
        if (button_reduce != null)
        {
            button_reduce.onClick.AddListener(OnReduceClicked);
        }
        if (scrollbar != null)
        {
            scrollbar.onValueChanged.AddListener(OnScrollChanged);
        }
    }

    // 在指定父节点下递归查找指定名称的组件
    private T FindDeepChild<T>(Transform parent, string name) where T : Component
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                T component = child.GetComponent<T>();
                if (component != null) return component;
            }
            T result = FindDeepChild<T>(child, name);
            if (result != null) return result;
        }
        return null;
    }

    /// <summary>
    /// 打开批量面板
    /// </summary>
    /// <param name="title">窗口标题</param>
    /// <param name="maxCount">最大可选数量</param>
    /// <param name="onConfirm">确认回调，参数为选中的数量</param>
    /// <param name="anotherBuilder">可选，接收当前数量返回要在 Text_Another 中显示的文字（如总价、累计效果）</param>
    public void Show(string title, int maxCount, System.Action<int> onConfirm, System.Func<int, string> anotherBuilder = null)
    {
        // 确保组件已绑定（对象非激活时 Awake 不会自动运行）
        Initialize();

        this.max_count = maxCount;
        this.on_confirm = onConfirm;
        this.another_builder = anotherBuilder;

        // 默认选中1个（滑块最左）
        this.current_count = 1;
        if (scrollbar != null) scrollbar.value = 0f;
        if (text_title != null) text_title.text = title;
        UpdateNumText();
        UpdateAnotherText();

        this.gameObject.SetActive(true);
    }

    // 滑块值变化
    private void OnScrollChanged(float value)
    {
        int count = Mathf.RoundToInt(1f + value * (max_count - 1));
        if (count < 1) count = 1;
        if (count > max_count) count = max_count;
        this.current_count = count;
        UpdateNumText();
        UpdateAnotherText();
    }

    // 更新数量显示
    private void UpdateNumText()
    {
        if (text_num != null)
        {
            text_num.text = current_count + "/" + max_count;
        }
    }

    // 更新另一个信息显示（累计效果/总价）
    private void UpdateAnotherText()
    {
        if (text_another != null)
        {
            if (another_builder != null)
            {
                text_another.text = another_builder(current_count);
            }
            else
            {
                text_another.text = "";
            }
        }
    }

    // 点击确认按钮
    private void OnConfirmClicked()
    {
        if (on_confirm != null && current_count > 0)
        {
            on_confirm(current_count);
        }
        this.gameObject.SetActive(false);
    }

    // 点击 +1 按钮
    private void OnAddClicked()
    {
        if (current_count < max_count)
        {
            current_count++;
            if (scrollbar != null)
            {
                scrollbar.value = (float)(current_count - 1) / (max_count - 1);
            }
            UpdateNumText();
            UpdateAnotherText();
        }
    }

    // 点击 -1 按钮
    private void OnReduceClicked()
    {
        if (current_count > 1)
        {
            current_count--;
            if (scrollbar != null)
            {
                scrollbar.value = (float)(current_count - 1) / (max_count - 1);
            }
            UpdateNumText();
            UpdateAnotherText();
        }
    }
}
