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
    private static readonly string[] GRADE_NAMES = {
        "仙一品", "神二品", "灵三品", "绝四品", "卓五品",
        "精六品", "良七品", "凡八品", "劣九品"
    };

    // 类型中文名（与 ItemTypeEnum 顺序一致）
    private static readonly string[] TYPE_NAMES = {
        "消耗品", "器具", "衣物", "书籍",
        "收集品", "材料", "交易品", "特殊"
    };

    // 效果中文名（与 ItemEffectType 顺序一致）
    private static readonly string[] EFFECT_NAMES = {
        "金钱", "健康", "精力", "饱腹",
        "力量", "敏捷", "才智", "魅力", "体魄", "气运"
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

        // 初始隐藏详情
        detail_panel.SetActive(false);
    }

    private void OnEnable()
    {
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
        foreach (KeyValuePair<int, Dictionary<ItemData, int>> outer_kv in player.inventory)
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

            Dictionary<ItemData, int> inner = outer_kv.Value;

            foreach (KeyValuePair<ItemData, int> inner_kv in inner)
            {
                ItemData instance = inner_kv.Key;
                int count = inner_kv.Value;

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
                text_type.text = GetTypeName(template.type);

                if (template.max_stack > 1)
                {
                    // 可堆叠：显示数量
                    text_num.text = "×" + count;
                    text_value.text = (template.base_value * count).ToString();
                }
                else
                {
                    // 不可堆叠：不显示数量
                    text_num.text = "";
                    text_value.text = template.base_value.ToString();
                }

                // 绑定点击事件
                Button btn = btn_obj.GetComponent<Button>();
                Image img = btn_obj.GetComponent<Image>();
                int captured_id = item_id;
                ItemData captured_instance = instance;

                btn.onClick.AddListener(() =>
                {
                    OnItemClicked(captured_id, captured_instance, btn, img);
                });
            }
        }
    }

    /// <summary>
    /// 选中一个物品
    /// </summary>
    private void OnItemClicked(int item_id, ItemData instance, Button button, Image image)
    {
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

        // 名称 + 品级
        detail_name.text = template.name + " · " + GetGradeName(template.grade);

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
        if (player != null && player.inventory.ContainsKey(item_id))
        {
            int total = 0;
            foreach (int cnt in player.inventory[item_id].Values)
            {
                total = total + cnt;
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
    /// 点击使用按钮——执行效果并移除一个
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

        // 获取模板
        ItemData template = ItemDictionary.Instance.Get(selected_item_id);
        if (template == null || template.effects == null)
        {
            return;
        }

        // 执行所有效果
        foreach (KeyValuePair<ItemEffectType, float> kv in template.effects)
        {
            ApplyEffect(player, kv.Key, kv.Value);
        }

        // 移除一个
        player.RemoveItem(selected_item_id, 1);

        // 通知其他面板刷新
        if (OnItemUsed != null)
        {
            OnItemUsed();
        }

        // 检查是否还有剩余
        if (player.inventory.ContainsKey(selected_item_id) && player.inventory[selected_item_id].Count > 0)
        {
            // 有剩余：更新选中按钮的数量文本 + 刷新详情，但不重建列表
            UpdateButtonCount(selected_button, selected_item_id);
            ShowDetail(selected_item_id, selected_item_instance);
        }
        else
        {
            // 无剩余：取消选中并刷新
            DeselectItem();
            Refresh();
        }
    }

    /// <summary>
    /// 应用单个物品效果到角色
    /// </summary>
    private void ApplyEffect(CharacterData character, ItemEffectType type, float value)
    {
        if (type == ItemEffectType.ModifyMoney)
        {
            character.money = character.money + (int)value;
        }
        else if (type == ItemEffectType.ModifyHealth)
        {
            character.health = character.health + value;
            if (character.health > character.max_health)
            {
                character.health = character.max_health;
            }
            if (character.health < 0f)
            {
                character.health = 0f;
            }
        }
        else if (type == ItemEffectType.ModifyEnergy)
        {
            character.energy = character.energy + value;
            if (character.energy > character.max_energy)
            {
                character.energy = character.max_energy;
            }
            if (character.energy < 0f)
            {
                character.energy = 0f;
            }
        }
        else if (type == ItemEffectType.ModifyFullness)
        {
            character.fullness = character.fullness + value;
            if (character.fullness > character.max_fullness)
            {
                character.fullness = character.max_fullness;
            }
            if (character.fullness < 0f)
            {
                character.fullness = 0f;
            }
        }
        else if (type == ItemEffectType.ModifyStrength)
        {
            character.ModifyAttribute(AttributeTypeEnum.Strength, value);
        }
        else if (type == ItemEffectType.ModifyAgility)
        {
            character.ModifyAttribute(AttributeTypeEnum.Agility, value);
        }
        else if (type == ItemEffectType.ModifyWit)
        {
            character.ModifyAttribute(AttributeTypeEnum.Wit, value);
        }
        else if (type == ItemEffectType.ModifyCharm)
        {
            character.ModifyAttribute(AttributeTypeEnum.Charm, value);
        }
        else if (type == ItemEffectType.ModifyPhysique)
        {
            character.ModifyAttribute(AttributeTypeEnum.Physique, value);
        }
        else if (type == ItemEffectType.ModifyLuck)
        {
            character.ModifyAttribute(AttributeTypeEnum.Luck, value);
        }
    }

    /// <summary>
    /// 点击丢弃按钮
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

        player.RemoveItem(selected_item_id, 1);

        // 检查是否还有剩余
        if (player.inventory.ContainsKey(selected_item_id) && player.inventory[selected_item_id].Count > 0)
        {
            // 有剩余：更新选中按钮的数量文本 + 刷新详情，但不重建列表
            UpdateButtonCount(selected_button, selected_item_id);
            ShowDetail(selected_item_id, selected_item_instance);
        }
        else
        {
            // 无剩余：取消选中并刷新
            DeselectItem();
            Refresh();
        }
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
        if (player == null || !player.inventory.ContainsKey(item_id))
        {
            return;
        }

        TextMeshProUGUI text_num = button.transform.Find("Text_Num").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI text_value = button.transform.Find("Text_Value").GetComponent<TextMeshProUGUI>();

        if (template.max_stack > 1)
        {
            // 统计该物品的总数
            int total = 0;
            foreach (int cnt in player.inventory[item_id].Values)
            {
                total = total + cnt;
            }
            text_num.text = "×" + total;
            text_value.text = (template.base_value * total).ToString();
        }
        // 不可堆叠物品不存在"有剩余"的情况（每个副本 count=1，被移除就没了）
    }

    /// <summary>
    /// 品级数字转中文（1→仙一品 ... 9→劣九品）
    /// </summary>
    private string GetGradeName(int grade)
    {
        if (grade >= 1 && grade <= 9)
        {
            return GRADE_NAMES[grade - 1];
        }
        return "";
    }

    /// <summary>
    /// 类型枚举转中文
    /// </summary>
    private string GetTypeName(ItemTypeEnum type)
    {
        int index = (int)type;
        if (index >= 0 && index < TYPE_NAMES.Length)
        {
            return TYPE_NAMES[index];
        }
        return "";
    }

    /// <summary>
    /// 效果枚举转中文
    /// </summary>
    private string GetEffectName(ItemEffectType type)
    {
        int index = (int)type;
        if (index >= 0 && index < EFFECT_NAMES.Length)
        {
            return EFFECT_NAMES[index];
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
