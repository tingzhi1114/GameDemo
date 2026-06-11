using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 背包面板——显示角色背包物品，支持类型筛选、使用、丢弃
/// </summary>
public class PanelInventory : MonoBehaviour
{
    /// <summary>
    /// 使用物品后触发，EventManager订阅此事件
    /// </summary>
    public static event System.Action OnItemUsed;
    // 在Inspector中拖入 Button_Item 预制体
    public GameObject button_item_prefab;

    // 缓存组件
    private Transform content;
    private GameObject detail_panel;
    private TextMeshProUGUI detail_name;
    private TextMeshProUGUI detail_desc;
    private TextMeshProUGUI detail_effect;
    private TextMeshProUGUI detail_status;
    private Button button_use;
    private Button button_discard;
    private Button button_process_mode;
    private PanelBatchProcess batch_process;    // 弹窗引用

    // 当前筛选类型（-1=全部）
    private int current_filter = -1;

    // 当前选中的物品信息
    private int selected_item_id = -1;
    private ItemData selected_item_instance;
    private Button selected_button;
    private Image selected_image;
    // 存储选中前按钮图片的原始颜色，用于取消时恢复
    private Color original_image_color;

    // 当前选中的筛选按钮及其原始颜色
    private Button selected_filter_button;
    private Image selected_filter_image;
    private Color original_filter_color;

    // 品级中文名（index 0=仙一品 ... index 8=劣九品）
    private static readonly string[] grade_names = {
        "仙一品", "神二品", "灵三品", "绝四品", "卓五品",
        "精六品", "良七品", "凡八品", "劣九品"
    };

    // 类型中文名（与 ItemTypeEnum 顺序一致）
    private static readonly string[] type_names = {
        "消耗品", "器具", "衣物", "书籍",
        "收集品", "材料", "交易品", "特殊"
    };

    // 效果中文名（与 ItemEffectType 顺序一致）
    private static readonly string[] effect_names = {
        "金钱", "健康", "精力", "饱腹",
        "力量", "敏捷", "才智", "魅力", "体魄", "气运"
    };

    // 子类型中文名（与 ItemSubTypeEnum 顺序一致）
    private static readonly string[] sub_type_names = {
        "食物", "饮品", "药品", "补品",
        "农具", "渔具", "炊具", "工具",
        "粮", "布", "木材", "矿产",
        "药材", "茶", "瓷器", "珠宝"
    };

    private void Awake()
    {
        // 缓存所有组件
        Transform inventory = this.transform.Find("Image_Inventory");

        // 关闭按钮
        Button button_close = inventory.Find("Button_Close").GetComponent<Button>();
        button_close.onClick.AddListener(() => { this.gameObject.SetActive(false); });

        // 物品列表Content
        content = inventory.Find("Image_ItemList/Scroll_View/Viewport/Content");

        // 绑定类型筛选按钮
        BindFilterButtons(inventory);

        // 详情面板
        detail_panel = inventory.Find("Image_ItemDetail").gameObject;
        detail_name = detail_panel.transform.Find("Text_Name").GetComponent<TextMeshProUGUI>();
        detail_desc = detail_panel.transform.Find("Text_Desc").GetComponent<TextMeshProUGUI>();
        detail_effect = detail_panel.transform.Find("Text_Effect").GetComponent<TextMeshProUGUI>();
        detail_status = detail_panel.transform.Find("Text_Status").GetComponent<TextMeshProUGUI>();
        button_use = detail_panel.transform.Find("Button_Use").GetComponent<Button>();
        button_discard = detail_panel.transform.Find("Button_Discard").GetComponent<Button>();

        // 绑定使用/丢弃
        button_use.onClick.AddListener(OnUseClicked);
        button_discard.onClick.AddListener(OnDiscardClicked);

        // 批量模式切换按钮（递归查找任意深度的Button_ProcessMode）
        button_process_mode = FindDeepChild<Button>(this.transform, "Button_ProcessMode");
        if (button_process_mode != null)
        {
            button_process_mode.onClick.AddListener(ToggleProcessMode);
        }

        // 批量处理弹窗
        batch_process = FindOrCreateBatchProcess();

        // 初始隐藏详情
        detail_panel.SetActive(false);
    }

    // 查找场景中已存在的批量处理弹窗（非激活状态）
    private PanelBatchProcess FindOrCreateBatchProcess()
    {
        PanelBatchProcess bp = Object.FindObjectOfType<PanelBatchProcess>(true);
        if (bp != null)
        {
            bp.gameObject.SetActive(false);
        }
        return bp;
    }

    // 切换批量/单个模式
    private void ToggleProcessMode()
    {
        Player.Instance.is_batch_mode = !Player.Instance.is_batch_mode;
        UpdateProcessModeButton();
    }

    // 更新批量模式按钮文字
    private void UpdateProcessModeButton()
    {
        if (button_process_mode != null)
        {
            TextMeshProUGUI btn_text = button_process_mode.GetComponentInChildren<TextMeshProUGUI>();
            if (btn_text != null)
            {
                btn_text.text = Player.Instance.is_batch_mode ? "批量模式" : "单个模式";
            }
        }
    }

    private void OnEnable()
    {
        UpdateProcessModeButton();
        Refresh();
    }

    /// <summary>
    /// 绑定类型筛选按钮的点击事件
    /// </summary>
    private void BindFilterButtons(Transform inventory)
    {
        Transform filter = inventory.Find("Image_TypeFilter");
        for (int i = 0; i < filter.childCount; i++)
        {
            Transform child = filter.GetChild(i);
            Button btn = child.GetComponent<Button>();
            if (btn == null)
            {
                continue;
            }

            // 默认选中"全部"按钮
            if (child.name == "Button_Type_All")
            {
                Image img = child.GetComponent<Image>();
                selected_filter_button = btn;
                selected_filter_image = img;
                original_filter_color = img.color;
                btn.transition = Selectable.Transition.None;
                img.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 灰色
            }

            string name = child.name;
            btn.onClick.AddListener(() =>
            {
                OnFilterClicked(name, btn, child.GetComponent<Image>());
            });
        }
    }

    /// <summary>
    /// 点击类型筛选按钮
    /// </summary>
    private void OnFilterClicked(string button_name, Button button, Image image)
    {
        // 取消上一个筛选项的选中状态
        if (selected_filter_image != null)
        {
            selected_filter_button.transition = Selectable.Transition.ColorTint;
            selected_filter_image.color = original_filter_color;
        }

        // 设置当前筛选按钮为选中状态
        selected_filter_button = button;
        selected_filter_image = image;
        original_filter_color = image.color;
        button.transition = Selectable.Transition.None;
        image.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 灰色

        if (button_name == "Button_Type_All")
        {
            current_filter = -1;
        }
        else
        {
            // 从按钮名解析类型，如 "Button_Type_Consumable" → Consumable
            string type_name = button_name.Replace("Button_Type_", "");
            ItemTypeEnum parsed;
            if (System.Enum.TryParse(type_name, out parsed))
            {
                current_filter = (int)parsed;
            }
            else
            {
                current_filter = -1;
            }
        }

        // 取消当前物品选中，刷新列表
        DeselectItem();
        Refresh();
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
    /// 刷新物品列表
    /// </summary>
    public void Refresh()
    {
        // 清空现有按钮
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        CharacterData player = Player.Instance.GetCharacter();
        if (player == null)
        {
            return;
        }

        // 遍历背包外层字典（item_id → inner dict）
        foreach (KeyValuePair<int, Dictionary<ItemData, int>> outer_kv in InventoryDictionary.Instance.GetOrCreate(player.id).items)
        {
            int item_id = outer_kv.Key;
            ItemData template = ItemDictionary.Instance.Get(item_id);
            if (template == null)
            {
                continue;
            }

            // 类型筛选
            if (current_filter >= 0 && (int)template.type != current_filter)
            {
                continue;
            }

            // 统计该物品的总数
            int count = 0;
            foreach (int cnt in outer_kv.Value.Values)
            {
                count += cnt;
            }

            // 实例化预制体
            GameObject btn_obj = Instantiate(button_item_prefab, content);
            btn_obj.name = "Button_Item_" + template.name;

            // 设置各文本
            TextMeshProUGUI text_name = btn_obj.transform.Find("Text_Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI text_grade = btn_obj.transform.Find("Text_Grade").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI text_type = btn_obj.transform.Find("Text_Type").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI text_num = btn_obj.transform.Find("Text_Num").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI text_value = btn_obj.transform.Find("Text_Value").GetComponent<TextMeshProUGUI>();

            text_name.text = template.name;
            text_grade.text = GetGradeName(template.grade);
            text_type.text = GetSubTypeName(template.sub_type) + "(" + GetTypeName(template.type) + ")";

            if (template.max_stack > 1)
            {
                // 可堆叠：显示数量和总价值
                text_num.text = "×" + count;
                text_value.text = (template.base_value * count).ToString();
            }
            else
            {
                // 不可堆叠：显示单个价值
                text_num.text = "";
                text_value.text = template.base_value.ToString();
            }

            // 绑定点击事件
            Button btn = btn_obj.GetComponent<Button>();
            Image img = btn_obj.GetComponent<Image>();
            int captured_id = item_id;
            ItemData captured_instance = template;

            btn.onClick.AddListener(() =>
            {
                OnItemClicked(captured_id, captured_instance, btn, img);
            });
        }
    }

    /// <summary>
    /// 选中一个物品
    /// </summary>
    private void OnItemClicked(int item_id, ItemData instance, Button button, Image image)
    {
        // 再次点击已选中的物品 → 取消选中
        if (selected_item_id == item_id)
        {
            DeselectItem();
            return;
        }

        // 取消上一个选中状态
        DeselectItem();

        // 设置当前选中
        selected_item_id = item_id;
        selected_item_instance = instance;
        selected_button = button;
        selected_image = image;

        // 保存按钮图片原始颜色，用于取消时恢复
        original_image_color = image.color;

        // 禁用按钮过渡，直接设颜色为按压色，确保选中状态不受交互影响
        button.transition = Selectable.Transition.None;
        image.color = button.colors.pressedColor;

        // 显示详情
        ShowDetail(item_id, instance);
    }

    /// <summary>
    /// 取消选中
    /// </summary>
    private void DeselectItem()
    {
        if (selected_image != null)
        {
            // 恢复按钮原始过渡和颜色
            selected_button.transition = Selectable.Transition.ColorTint;
            selected_image.color = original_image_color;
        }

        selected_item_id = -1;
        selected_item_instance = null;
        selected_button = null;
        selected_image = null;

        detail_panel.SetActive(false);
    }

    /// <summary>
    /// 显示物品详情
    /// </summary>
    private void ShowDetail(int item_id, ItemData instance)
    {
        ItemData template = ItemDictionary.Instance.Get(item_id);
        if (template == null)
        {
            return;
        }

        // 名称 + 品级（无品级不显示" · "）
        string grade_str = GetGradeName(template.grade);
        if (!string.IsNullOrEmpty(grade_str))
        {
            detail_name.text = template.name + " · " + grade_str;
        }
        else
        {
            detail_name.text = template.name;
        }

        // 描述 + 限制后缀
        detail_desc.text = "描述：" + template.description + BuildRestrictionSuffix(template);

        // 效果
        if (template.effects != null && template.effects.Count > 0)
        {
            string effect_text = "";
            foreach (KeyValuePair<ItemEffectType, float> kv in template.effects)
            {
                string sign = "";
                if (kv.Value >= 0f)
                {
                    sign = "+";
                }
                effect_text = effect_text + GetEffectName(kv.Key) + " " + sign + kv.Value + "  ";
            }
            detail_effect.text = "效果：" + effect_text;
        }
        else
        {
            detail_effect.text = "效果：无效果";
        }

        // 状态（数量 + 个体特性）
        string status_text = "";

        // 个体特性
        if (instance.properties != null && instance.properties.Count > 0)
        {
            foreach (KeyValuePair<ItemPropertyType, float> kv in instance.properties)
            {
                status_text = status_text + kv.Key + ":" + kv.Value + "  ";
            }
        }

        // 统计背包中该物品总数
        CharacterData player = Player.Instance.GetCharacter();
        if (player != null && InventoryDictionary.Instance.GetOrCreate(player.id).items.ContainsKey(item_id))
        {
            int total = 0;
            foreach (int cnt in InventoryDictionary.Instance.GetOrCreate(player.id).items[item_id].Values)
            {
                total += cnt;
            }
            status_text = "持有：" + total + "    " + status_text;
        }

        detail_status.text = "状态：" + status_text;

        // 控制使用/丢弃按钮——不可用时设为灰色不可交互
        button_use.interactable = template.can_use;
        button_discard.interactable = template.can_discard;

        detail_panel.SetActive(true);
    }

    /// <summary>
    /// 点击使用按钮——执行效果并移除一个（批量模式弹出弹窗）
    /// </summary>
    private void OnUseClicked()
    {
        if (selected_item_instance == null)
        {
            return;
        }

        CharacterData player = Player.Instance.GetCharacter();
        if (player == null)
        {
            return;
        }

        ItemData template = ItemDictionary.Instance.Get(selected_item_id);
        if (template == null || template.effects == null)
        {
            return;
        }

        // 批量模式 → 弹窗
        if (Player.Instance.is_batch_mode)
        {
            int max_count = InventoryDictionary.Instance.GetOrCreate(player.id).GetCount(selected_item_id);
            if (max_count <= 1)
            {
                // 只有1个时直接执行，不弹窗
                ExecuteUseOnce(player, template);
                return;
            }

            // 效果累计由 anotherBuilder 动态更新
            if (batch_process != null)
            {
                batch_process.Show("使用", max_count, (count) =>
                {
                    CharacterData latest_player = Player.Instance.GetCharacter();
                    if (latest_player == null) return;

                    for (int i = 0; i < count; i++)
                    {
                        ExecuteUseOnce(latest_player, template);
                    }

                    if (OnItemUsed != null) OnItemUsed();

                    if (InventoryDictionary.Instance.GetOrCreate(latest_player.id).GetCount(selected_item_id) > 0)
                    {
                        UpdateButtonCount(selected_button, selected_item_id);
                        ShowDetail(selected_item_id, selected_item_instance);
                    }
                    else
                    {
                        DeselectItem();
                        Refresh();
                    }
                },
                (c) => BuildEffectAccumulation(template.effects, c)
                );
            }
            return;
        }

        // 单个模式
        ExecuteUseOnce(player, template);
        if (OnItemUsed != null) OnItemUsed();

        if (InventoryDictionary.Instance.GetOrCreate(player.id).GetCount(selected_item_id) > 0)
        {
            UpdateButtonCount(selected_button, selected_item_id);
            ShowDetail(selected_item_id, selected_item_instance);
        }
        else
        {
            DeselectItem();
            Refresh();
        }
    }

    // 执行一次使用效果并移除一个物品
    private void ExecuteUseOnce(CharacterData player, ItemData template)
    {
        foreach (KeyValuePair<ItemEffectType, float> kv in template.effects)
        {
            ItemEffectExecutor.Execute(player, kv.Key, kv.Value);
        }
        InventoryDictionary.Instance.GetOrCreate(player.id).RemoveItem(template.id, 1);
    }

    // 构建效果累计文本（如"饱腹+50  精力+20"）
    private string BuildEffectAccumulation(Dictionary<ItemEffectType, float> effects, int multiplier)
    {
        if (effects == null || effects.Count == 0)
        {
            return "";
        }

        string text = "";
        foreach (KeyValuePair<ItemEffectType, float> kv in effects)
        {
            float total = kv.Value * (multiplier > 0 ? multiplier : 1);
            string sign = total >= 0f ? "+" : "";
            text += GetEffectName(kv.Key) + " " + sign + total + "  ";
        }
        return text.Trim();
    }

    /// <summary>
    /// 点击丢弃按钮——移除一个（批量模式弹出弹窗）
    /// </summary>
    private void OnDiscardClicked()
    {
        if (selected_item_instance == null)
        {
            return;
        }

        CharacterData player = Player.Instance.GetCharacter();
        if (player == null)
        {
            return;
        }

        // 批量模式 → 弹窗  
        if (Player.Instance.is_batch_mode)
        {
            int max_count = InventoryDictionary.Instance.GetOrCreate(player.id).GetCount(selected_item_id);
            if (max_count <= 1)
            {
                ExecuteDiscardOnce(player);
                // 检查剩余
                if (InventoryDictionary.Instance.GetOrCreate(player.id).GetCount(selected_item_id) > 0)
                {
                    UpdateButtonCount(selected_button, selected_item_id);
                    ShowDetail(selected_item_id, selected_item_instance);
                }
                else
                {
                    DeselectItem();
                    Refresh();
                }
                return;
            }

            if (batch_process != null)
            {
                batch_process.Show("丢弃", max_count, (count) =>
                {
                    CharacterData latest_player = Player.Instance.GetCharacter();
                    if (latest_player == null) return;

                    for (int i = 0; i < count; i++)
                    {
                        ExecuteDiscardOnce(latest_player);
                    }

                    if (InventoryDictionary.Instance.GetOrCreate(latest_player.id).GetCount(selected_item_id) > 0)
                    {
                        UpdateButtonCount(selected_button, selected_item_id);
                        ShowDetail(selected_item_id, selected_item_instance);
                    }
                    else
                    {
                        DeselectItem();
                        Refresh();
                    }
                });
            }
            return;
        }

        // 单个模式
        ExecuteDiscardOnce(player);
        if (InventoryDictionary.Instance.GetOrCreate(player.id).GetCount(selected_item_id) > 0)
        {
            UpdateButtonCount(selected_button, selected_item_id);
            ShowDetail(selected_item_id, selected_item_instance);
        }
        else
        {
            DeselectItem();
            Refresh();
        }
    }

    // 执行一次丢弃
    private void ExecuteDiscardOnce(CharacterData player)
    {
        InventoryDictionary.Instance.GetOrCreate(player.id).RemoveItem(selected_item_id, 1);
    }

    /// <summary>
    /// 更新指定按钮上的数量/价值文本
    /// </summary>
    private void UpdateButtonCount(Button button, int item_id)
    {
        ItemData template = ItemDictionary.Instance.Get(item_id);
        if (template == null)
        {
            return;
        }

        CharacterData player = Player.Instance.GetCharacter();
        if (player == null || !InventoryDictionary.Instance.GetOrCreate(player.id).items.ContainsKey(item_id))
        {
            return;
        }

        TextMeshProUGUI text_num = button.transform.Find("Text_Num").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI text_value = button.transform.Find("Text_Value").GetComponent<TextMeshProUGUI>();

        int total = 0;
        foreach (int cnt in InventoryDictionary.Instance.GetOrCreate(player.id).items[item_id].Values)
        {
            total += cnt;
        }
        text_num.text = "×" + total;
        text_value.text = (template.base_value * total).ToString();
    }

    /// <summary>
    /// 品级数字转中文（1→仙一品 ... 9→劣九品）
    /// </summary>
    private string GetGradeName(int grade)
    {
        if (grade >= 1 && grade <= 9)
        {
            return grade_names[grade - 1];
        }
        return "";
    }

    /// <summary>
    /// 类型枚举转中文
    /// </summary>
    private string GetTypeName(ItemTypeEnum type)
    {
        int index = (int)type;
        if (index >= 0 && index < type_names.Length)
        {
            return type_names[index];
        }
        return "";
    }

    // 子类型枚举转中文
    private string GetSubTypeName(ItemSubTypeEnum subType)
    {
        int index = (int)subType;
        if (index >= 0 && index < sub_type_names.Length)
        {
            return sub_type_names[index];
        }
        return "";
    }

    /// <summary>
    /// 效果枚举转中文
    /// </summary>
    private string GetEffectName(ItemEffectType type)
    {
        int index = (int)type;
        if (index >= 0 && index < effect_names.Length)
        {
            return effect_names[index];
        }
        return "";
    }

    /// <summary>
    /// 构建限制后缀——如"(不可使用，不可丢弃)"
    /// </summary>
    private string BuildRestrictionSuffix(ItemData template)
    {
        string suffix = "";
        if (!template.can_use)
        {
            suffix = suffix + "不可使用";
        }
        if (!template.can_discard)
        {
            if (suffix.Length > 0) suffix = suffix + "，";
            suffix = suffix + "不可丢弃";
        }
        if (!template.can_trade)
        {
            if (suffix.Length > 0) suffix = suffix + "，";
            suffix = suffix + "不可交易";
        }
        if (suffix.Length > 0)
        {
            return "(" + suffix + ")";
        }
        return "";
    }
}
