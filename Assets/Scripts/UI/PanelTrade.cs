using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 交易面板——支持买入/卖出两种模式
/// 买入模式显示指定商品列表，卖出模式显示角色背包中可交易的物品
/// 价格以 "价格(价值)" 格式显示，价格根据与价值的比值着色
/// </summary>
public class PanelTrade : MonoBehaviour
{
    /// <summary>
    /// 购买完成后触发，EventManager订阅此事件
    /// </summary>
    public static event System.Action OnBuyExecuted;
    /// <summary>
    /// 出售完成后触发，EventManager订阅此事件
    /// </summary>
    public static event System.Action OnSellExecuted;

    // 在Inspector中拖入 Button_Item_Trade 预制体
    public GameObject button_item_trade_prefab;

    // 缓存组件
    private Transform content;
    private GameObject detail_panel;
    private TextMeshProUGUI detail_name;
    private TextMeshProUGUI detail_desc;
    private TextMeshProUGUI detail_effect;
    private TextMeshProUGUI detail_status;
    private Button button_buy_action;
    private Button button_sell_action;
    private Button button_process_mode;
    private PanelBatchProcess batch_process;
    private TextMeshProUGUI text_money;
    private TextMeshProUGUI text_title;
    private TextMeshProUGUI text_list_title;

    // 当前模式（true=买入, false=卖出）
    private bool is_buy_mode;
    // 当前筛选类型（-1=全部）
    private int current_filter = -1;

    // 买入模式数据：商品ID列表 + 价格字典（item_id → 买入价）
    private List<int> buy_item_ids;
    private Dictionary<int, int> buy_prices;

    // 当前交易的场景引用（用于读取/扣减商店库存）
    private SceneData current_trade_scene;

    // 卖出模式数据：卖出倍率 + 允许售卖的物品子类型列表（由SceneTemplate提供）
    private float sell_rate;
    private List<ItemSubTypeEnum> sell_allowed_types;

    // 选中状态（不依赖自定义类，直接用ItemData + 标量字段）
    private int selected_item_id = -1;
    private int selected_price;
    private int selected_count;  // 卖出模式用：选中物品的持有数
    private Button selected_button;
    private Image selected_image;
    private Color original_image_color;

    // 筛选按钮选中状态
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

    // 子类型中文名（与 ItemSubTypeEnum 顺序一致）
    private static readonly string[] sub_type_names = {
        "食物", "饮品", "药品", "补品",
        "农具", "渔具", "炊具", "工具",
        "粮", "布", "木材", "矿产",
        "药材", "茶", "瓷器", "珠宝"
    };

    // 效果中文名（与 ItemEffectType 顺序一致）
    private static readonly string[] effect_names = {
        "金钱", "健康", "精力", "饱腹",
        "力量", "敏捷", "才智", "魅力", "体魄", "气运"
    };

    private void Awake()
    {
        Transform panel = this.transform.Find("Image_Trade");

        // 标题
        text_title = panel.Find("Image_Title/Text_Title").GetComponent<TextMeshProUGUI>();
        text_list_title = panel.Find("Image_ItemListTitle/Text_ItemListTitle").GetComponent<TextMeshProUGUI>();

        // 关闭按钮
        Button button_close = panel.Find("Button_Close").GetComponent<Button>();
        button_close.onClick.AddListener(() => { this.gameObject.SetActive(false); });

        // 物品列表容器
        content = panel.Find("Image_ItemList/Scroll_View/Viewport/Content");

        // 绑定类型筛选按钮
        BindFilterButtons(panel);

        // 详情面板
        detail_panel = panel.Find("Image_ItemDetail").gameObject;
        detail_name = detail_panel.transform.Find("Text_Name").GetComponent<TextMeshProUGUI>();
        detail_desc = detail_panel.transform.Find("Text_Desc").GetComponent<TextMeshProUGUI>();
        detail_effect = detail_panel.transform.Find("Text_Effect").GetComponent<TextMeshProUGUI>();
        detail_status = detail_panel.transform.Find("Text_Status").GetComponent<TextMeshProUGUI>();
        button_buy_action = detail_panel.transform.Find("Button_Buy").GetComponent<Button>();
        button_sell_action = detail_panel.transform.Find("Button_Sell").GetComponent<Button>();

        // 绑定买入/卖出操作按钮
        button_buy_action.onClick.AddListener(OnBuyClicked);
        button_sell_action.onClick.AddListener(OnSellClicked);

        // 批量模式切换按钮（递归查找任意深度的Button_ProcessMode）
        button_process_mode = FindDeepChild<Button>(this.transform, "Button_ProcessMode");
        if (button_process_mode != null)
        {
            button_process_mode.onClick.AddListener(ToggleProcessMode);
        }

        // 批量处理弹窗
        batch_process = FindOrCreateBatchProcess();

        // 金钱显示
        Transform money_trans = panel.Find("Image_Money");
        if (money_trans != null)
        {
            text_money = money_trans.Find("Text_Money")?.GetComponent<TextMeshProUGUI>();
        }

        // 初始隐藏详情
        detail_panel.SetActive(false);
    }

    // 刷新金钱显示
    private void RefreshMoney()
    {
        if (text_money == null) return;
        CharacterData player = Player.Instance.GetCharacter();
        if (player != null)
        {
            text_money.text = "金钱：" + player.money;
        }
    }

    private void OnEnable()
    {
        // 清除选中状态，以干净状态展示
        DeselectItem();

        // 更新批量模式按钮文字
        UpdateProcessModeButton();

        // 刷新金钱显示
        RefreshMoney();

        // 每次激活时刷新显示
        if (is_buy_mode && buy_item_ids != null)
        {
            Refresh();
        }
        else if (!is_buy_mode)
        {
            Refresh();
        }
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

    // ==================== 价格重新计算 & 批量累进总价 ====================

    // 获取当前场景中选中物品的价格计算参数
    private void GetCurrentItemPriceParams(out ItemData template, out int stock, out int target,
        out float prod_coeff, out float z1, out float z2)
    {
        template = ItemDictionary.Instance.Get(selected_item_id);
        stock = 0;
        target = 0;
        prod_coeff = 0f;
        z1 = 0f;
        z2 = 0f;

        if (template == null || current_trade_scene == null) return;

        stock = current_trade_scene.GetStock(selected_item_id);
        if (current_trade_scene.target_stock != null && current_trade_scene.target_stock.ContainsKey(selected_item_id))
            target = current_trade_scene.target_stock[selected_item_id];

        LocationData location = LocationDictionary.Instance.Get(current_trade_scene.location_id);
        if (location != null)
        {
            if (location.productivity != null && location.productivity.ContainsKey(selected_item_id))
                prod_coeff = location.productivity[selected_item_id];
            z1 = LocationDictionary.Instance.GetZ1(location.id, selected_item_id);
            z2 = LocationDictionary.Instance.GetZ2(location.id, selected_item_id);
        }
    }

    // 重新计算选中物品的当前买入价
    private void RecalculateBuyPrice()
    {
        ItemData template;
        int stock, target;
        float prod_coeff, z1, z2;
        GetCurrentItemPriceParams(out template, out stock, out target, out prod_coeff, out z1, out z2);
        if (template != null)
        {
            selected_price = template.GetPrice(stock, target, prod_coeff, z1, z2);
        }
    }

    // 重新计算选中物品的当前卖出价
    private void RecalculateSellPrice()
    {
        ItemData template;
        int stock, target;
        float prod_coeff, z1, z2;
        GetCurrentItemPriceParams(out template, out stock, out target, out prod_coeff, out z1, out z2);
        if (template != null)
        {
            int buy_price = template.GetPrice(stock, target, prod_coeff, z1, z2);
            selected_price = ClampPrice(Mathf.RoundToInt(buy_price * sell_rate), template.base_value);
        }
    }

    // 计算批量买入的累进总价（每次购买后库存减少，价格变化）
    // 返回富文本带颜色（总价根据平均单价 vs 标准价着色）
    private string CalcBatchBuyTotal(int count)
    {
        ItemData template;
        int stock, target;
        float prod_coeff, z1, z2;
        GetCurrentItemPriceParams(out template, out stock, out target, out prod_coeff, out z1, out z2);
        if (template == null || count <= 0) return "总价：0";

        int total = 0;
        for (int i = 0; i < count; i++)
        {
            int p = template.GetPrice(stock - i, target, prod_coeff, z1, z2);
            if (p <= 0) break;
            total += p;
        }

        // 按平均单价着色
        int avg = total / count;
        int base_val = template.base_value;
        Color c = GetPriceColor(avg, base_val);
        string hex = ColorToHex(c);
        return "总价：<color=#" + hex + ">" + total + "</color>";
    }

    // 计算批量出售的累进总价（每次出售库存增加，卖出价变化）
    // 返回富文本带颜色（总价根据平均单价 vs 标准价着色）
    private string CalcBatchSellTotal(int count)
    {
        ItemData template;
        int stock, target;
        float prod_coeff, z1, z2;
        GetCurrentItemPriceParams(out template, out stock, out target, out prod_coeff, out z1, out z2);
        if (template == null || count <= 0) return "总价：0";

        int total = 0;
        for (int i = 0; i < count; i++)
        {
            int buy_price = template.GetPrice(stock + i, target, prod_coeff, z1, z2);
            int sell_price = ClampPrice(Mathf.RoundToInt(buy_price * sell_rate), template.base_value);
            total += sell_price;
        }

        // 按平均单价着色
        int avg = total / count;
        int base_val = template.base_value;
        Color c = GetPriceColor(avg, base_val);
        string hex = ColorToHex(c);
        return "总价：<color=#" + hex + ">" + total + "</color>";
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
    /// 打开买入面板——传入场景ID，从场景库存 + 地点价格乘数获取商品和价格
    /// </summary>
    public void ShowBuyPanel(int scene_id)
    {
        SceneData scene = SceneDictionary.Instance.Get(scene_id);
        if (scene == null || scene.inventory == null || scene.inventory.Count == 0)
        {
            return;
        }

        // 获取场景所属地点的生产力系数和地缘参数
        LocationData location = LocationDictionary.Instance.Get(scene.location_id);
        Dictionary<int, float> productivity = null;
        if (location != null)
        {
            productivity = location.productivity;
        }

        // 构建买入商品ID列表和价格字典
        this.buy_item_ids = new List<int>();
        this.buy_prices = new Dictionary<int, int>();
        this.current_trade_scene = scene;

        foreach (KeyValuePair<int, Dictionary<ItemData, int>> kv in scene.inventory)
        {
            int item_id = kv.Key;
            ItemData template = ItemDictionary.Instance.Get(item_id);
            if (template == null)
            {
                continue;
            }

            // 获取当前库存和期望库存
            int stock = scene.GetStock(item_id);
            int target = 0;
            if (scene.target_stock != null && scene.target_stock.ContainsKey(item_id))
            {
                target = scene.target_stock[item_id];
            }
            // 获取生产力系数（默认0）
            float prod_coeff = 0f;
            if (productivity != null && productivity.ContainsKey(item_id))
            {
                prod_coeff = productivity[item_id];
            }

            // 获取 Z1、Z2（默认0）
            float z1 = 0f, z2 = 0f;
            if (location != null)
            {
                z1 = LocationDictionary.Instance.GetZ1(location.id, item_id);
                z2 = LocationDictionary.Instance.GetZ2(location.id, item_id);
            }

            // 计算动态价格
            int price = template.GetPrice(stock, target, prod_coeff, z1, z2);

            buy_item_ids.Add(item_id);
            buy_prices[item_id] = price;
        }

        this.is_buy_mode = true;
        this.gameObject.SetActive(true);
        DeselectItem();
        Refresh();
    }

    /// <summary>
    /// 打开卖出面板——传入场景ID和卖出倍率，从场景模板获取允许售卖类型
    /// </summary>
    public void ShowSellPanel(int scene_id, float sellRate)
    {
        this.sell_rate = sellRate;

        // 从场景模板获取允许售卖的类型
        SceneData scene = SceneDictionary.Instance.Get(scene_id);
        this.current_trade_scene = scene;
        if (scene != null)
        {
            SceneTemplateData template = SceneTemplateDictionary.Instance.Get(scene.template_id);
            if (template != null)
            {
                this.sell_allowed_types = template.trade_types;
            }
            else
            {
                this.sell_allowed_types = null;
            }
        }
        else
        {
            this.sell_allowed_types = null;
        }

        this.is_buy_mode = false;
        this.gameObject.SetActive(true);
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

        // 取消选中的物品
        DeselectItem();

        // 根据模式更新标题和按钮可见性
        if (is_buy_mode)
        {
            text_title.text = "购买";
            button_buy_action.gameObject.SetActive(true);
            button_sell_action.gameObject.SetActive(false);
        }
        else
        {
            text_title.text = "出售";
            button_buy_action.gameObject.SetActive(false);
            button_sell_action.gameObject.SetActive(true);
        }

        if (is_buy_mode)
        {
            RefreshBuyList();
        }
        else
        {
            RefreshSellList();
        }
    }

    // 刷新买入列表
    private void RefreshBuyList()
    {
        if (buy_item_ids == null || buy_prices == null)
        {
            return;
        }

        for (int i = 0; i < buy_item_ids.Count; i++)
        {
            int item_id = buy_item_ids[i];
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

            int price = buy_prices[item_id];

            // 从场景库存中获取数量
            int stock = current_trade_scene.GetStock(item_id);

            // 库存为0的商品不显示
            if (stock <= 0)
            {
                continue;
            }

            CreateItemButton(template, price, stock);
        }
    }

    // 刷新卖出列表
    private void RefreshSellList()
    {
        CharacterData player = Player.Instance.GetCharacter();
        if (player == null)
        {
            return;
        }

        // 遍历背包（item_id → inner dict）
        foreach (KeyValuePair<int, Dictionary<ItemData, int>> outer_kv in InventoryDictionary.Instance.GetOrCreate(player.id).items)
        {
            int item_id = outer_kv.Key;
            int total_count = 0;
            foreach (int cnt in outer_kv.Value.Values)
            {
                total_count += cnt;
            }

            ItemData template = ItemDictionary.Instance.Get(item_id);
            if (template == null || !template.can_trade)
            {
                continue;
            }

            // 类型筛选：必须符合场景模板允许售卖的物品子类型
            if (sell_allowed_types != null && sell_allowed_types.Count > 0)
            {
                bool type_allowed = false;
                for (int t = 0; t < sell_allowed_types.Count; t++)
                {
                    if (sell_allowed_types[t] == template.sub_type)
                    {
                        type_allowed = true;
                        break;
                    }
                }
                if (!type_allowed)
                {
                    continue;
                }
            }

            // 类型筛选按钮过滤
            if (current_filter >= 0 && (int)template.type != current_filter)
            {
                continue;
            }

            // 根据当前场景计算卖出价 = 商店购入价 × 卖出倍率
            int price = CalculateSellPrice(item_id, template, total_count);

            CreateItemButton(template, price, total_count);
        }
    }

    // 根据当前场景计算某物品的卖出价
    private int CalculateSellPrice(int item_id, ItemData template, int player_count)
    {
        if (current_trade_scene == null)
        {
            // 没有场景上下文时按基准价算
            int fallback = Mathf.RoundToInt(template.base_value * sell_rate);
            return ClampPrice(fallback, template.base_value);
        }

        // 获取场景所属地点的参数
        LocationData location = LocationDictionary.Instance.Get(current_trade_scene.location_id);

        // 从场景库存中获取当前库存和期望库存
        int stock = current_trade_scene.GetStock(item_id);
        int target = 0;
        if (current_trade_scene.target_stock != null && current_trade_scene.target_stock.ContainsKey(item_id))
        {
            target = current_trade_scene.target_stock[item_id];
        }

        // 生产力系数
        float prod_coeff = 0f;
        if (location != null && location.productivity != null && location.productivity.ContainsKey(item_id))
        {
            prod_coeff = location.productivity[item_id];
        }

        // Z1、Z2
        float z1 = 0f, z2 = 0f;
        if (location != null)
        {
            z1 = LocationDictionary.Instance.GetZ1(location.id, item_id);
            z2 = LocationDictionary.Instance.GetZ2(location.id, item_id);
        }

        // 先算商店买入价，再打折
        int buy_price = template.GetPrice(stock, target, prod_coeff, z1, z2);
        int sell_price = Mathf.RoundToInt(buy_price * sell_rate);
        return ClampPrice(sell_price, template.base_value);
    }

    // 创建一个物品按钮（买入/卖出模式共用）
    private void CreateItemButton(ItemData template, int price, int count)
    {
        // 实例化预制体
        GameObject btn_obj = Instantiate(button_item_trade_prefab, content);
        btn_obj.name = "Button_Trade_" + template.name;

        // 设置各文本
        TextMeshProUGUI text_name = btn_obj.transform.Find("Text_Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI text_grade = btn_obj.transform.Find("Text_Grade").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI text_type = btn_obj.transform.Find("Text_Type").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI text_num = btn_obj.transform.Find("Text_Num").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI text_value = btn_obj.transform.Find("Text_Value").GetComponent<TextMeshProUGUI>();

        text_name.text = template.name;

        // 品级：无品级（0）显示"-"
        if (template.grade >= 1)
        {
            text_grade.text = GetGradeName(template.grade);
        }
        else
        {
            text_grade.text = "-";
        }

        text_type.text = GetSubTypeName(template.sub_type) + "(" + GetTypeName(template.type) + ")";

        // 数量：买入模式显示商店库存，卖出模式显示角色持有数
        if (count > 0)
        {
            text_num.text = "×" + count;
        }
        else
        {
            text_num.text = "";
        }

        // 价格(价值) 格式——价格着彩色，括号和价值保持默认色
        int base_value = template.base_value;
        Color price_color = GetPriceColor(price, base_value);
        string price_hex = ColorToHex(price_color);
        text_value.text = "<color=#" + price_hex + ">" + price + "</color>(" + base_value + ")";

        // 绑定点击事件
        Button btn = btn_obj.GetComponent<Button>();
        Image img = btn_obj.GetComponent<Image>();
        int captured_id = template.id;
        int captured_price = price;
        int captured_count = count;

        btn.onClick.AddListener(() =>
        {
            OnItemClicked(captured_id, captured_price, captured_count, btn, img);
        });
    }

    // 绑定类型筛选按钮的点击事件
    private void BindFilterButtons(Transform panel)
    {
        Transform filter = panel.Find("Image_TypeFilter");
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

    // 点击类型筛选按钮
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

    // 选中一个物品
    private void OnItemClicked(int item_id, int price, int count, Button button, Image image)
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
        selected_price = price;
        selected_count = count;
        selected_button = button;
        selected_image = image;

        // 保存按钮图片原始颜色，用于取消时恢复
        original_image_color = image.color;

        // 禁用按钮过渡，直接设颜色为按压色，确保选中状态不受交互影响
        button.transition = Selectable.Transition.None;
        image.color = button.colors.pressedColor;

        // 显示详情
        ShowDetail(item_id);
    }

    // 取消选中
    private void DeselectItem()
    {
        if (selected_image != null)
        {
            // 恢复按钮原始过渡和颜色
            selected_button.transition = Selectable.Transition.ColorTint;
            selected_image.color = original_image_color;
        }

        selected_item_id = -1;
        selected_price = 0;
        selected_count = 0;
        selected_button = null;
        selected_image = null;

        detail_panel.SetActive(false);
    }

    // 显示物品详情
    private void ShowDetail(int item_id)
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
                effect_text += GetEffectName(kv.Key) + " " + sign + kv.Value + "  ";
            }
            detail_effect.text = "效果：" + effect_text;
        }
        else
        {
            detail_effect.text = "效果：无效果";
        }

        // 状态行——显示价格(价值)信息，价格着彩色
        int base_value = template.base_value;
        Color price_color = GetPriceColor(selected_price, base_value);
        string price_hex = ColorToHex(price_color);
        string price_colored = "<color=#" + price_hex + ">" + selected_price + "</color>";

        if (is_buy_mode)
        {
            detail_status.text = "买入价：" + price_colored + "(" + base_value + ")";
        }
        else
        {
            detail_status.text = "卖出价：" + price_colored + "(" + base_value + ")    持有：" + selected_count;
        }

        detail_panel.SetActive(true);
    }

    // 点击买入按钮（批量模式弹出弹窗）
    private void OnBuyClicked()
    {
        if (selected_item_id < 0)
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
            int max_count = current_trade_scene != null ? current_trade_scene.GetStock(selected_item_id) : 0;
            if (max_count <= 0) return;

            // 检查能买多少（看钱）
            int max_by_money = player.money / selected_price;
            if (max_by_money < max_count) max_count = max_by_money;
            if (max_count <= 0)
            {
                Debug.Log(player.name + "囊中羞涩，买不起。");
                return;
            }
            if (max_count <= 1)
            {
                // 只能买1个时直接执行
                ExecuteBuyOnce(player);
                OnBuyExecuted?.Invoke();
                RefreshMoney();
                HandleBuyComplete();
                return;
            }

            // 总价由 anotherBuilder 动态更新（累进计算，每次购买后库存减少，价格变化）
            if (batch_process != null)
            {
                batch_process.Show("购买", max_count, (count) =>
                {
                    CharacterData latest_player = Player.Instance.GetCharacter();
                    if (latest_player == null) return;

                    int total_cost = selected_price * count;
                    if (latest_player.money < total_cost)
                    {
                        Debug.Log("钱不够，只能买一部分。");
                        int can_afford = latest_player.money / selected_price;
                        if (can_afford <= 0) return;
                        count = can_afford;
                    }

                    if (current_trade_scene != null)
                    {
                        int actual_stock = current_trade_scene.GetStock(selected_item_id);
                        if (count > actual_stock) count = actual_stock;
                    }
                    if (count <= 0) return;

                    for (int i = 0; i < count; i++)
                    {
                        ExecuteBuyOnce(latest_player);
                    }
                    OnBuyExecuted?.Invoke();
                    RefreshMoney();
                    HandleBuyComplete();
                },
                CalcBatchBuyTotal
                );
            }
            return;
        }

        // 单个模式
        if (player.money < selected_price)
        {
            Debug.Log(player.name + "囊中羞涩，买不起。");
            return;
        }

        ExecuteBuyOnce(player);
        OnBuyExecuted?.Invoke();
        RefreshMoney();
        HandleBuyComplete();
    }

    // 执行一次购买
    private void ExecuteBuyOnce(CharacterData player)
    {
        player.money -= selected_price;
        InventoryDictionary.Instance.GetOrCreate(player.id).AddItem(selected_item_id, 1);
        if (current_trade_scene != null)
        {
            current_trade_scene.RemoveStock(selected_item_id, 1);
        }
    }

    // 购买后的通用处理
    private void HandleBuyComplete()
    {
        bool stock_gone = current_trade_scene == null || current_trade_scene.GetStock(selected_item_id) <= 0;
        if (stock_gone)
        {
            DeselectItem();
            Refresh();
        }
        else
        {
            RecalculateBuyPrice();
            UpdateItemButton(selected_button, selected_item_id, selected_price);
            ShowDetail(selected_item_id);
        }
    }

    // 点击卖出按钮（批量模式弹出弹窗）
    private void OnSellClicked()
    {
        if (selected_item_id < 0)
        {
            return;
        }

        CharacterData player = Player.Instance.GetCharacter();
        if (player == null)
        {
            return;
        }

        ItemData template = ItemDictionary.Instance.Get(selected_item_id);
        if (template == null)
        {
            return;
        }

        // 批量模式 → 弹窗
        if (Player.Instance.is_batch_mode)
        {
            int max_count = InventoryDictionary.Instance.GetOrCreate(player.id).GetCount(selected_item_id);
            if (max_count <= 0) return;
            if (max_count <= 1)
            {
                ExecuteSellOnce(player);
                OnSellExecuted?.Invoke();
                RefreshMoney();
                HandleSellComplete();
                return;
            }

            if (batch_process != null)
            {
                batch_process.Show("出售", max_count, (count) =>
                {
                    CharacterData latest_player = Player.Instance.GetCharacter();
                    if (latest_player == null) return;

                    int actual_count = InventoryDictionary.Instance.GetOrCreate(latest_player.id).GetCount(selected_item_id);
                    if (count > actual_count) count = actual_count;
                    if (count <= 0) return;

                    for (int i = 0; i < count; i++)
                    {
                        ExecuteSellOnce(latest_player);
                    }
                    OnSellExecuted?.Invoke();
                    RefreshMoney();
                    HandleSellComplete();
                },
                CalcBatchSellTotal
                );
            }
            return;
        }

        // 单个模式
        ExecuteSellOnce(player);
        OnSellExecuted?.Invoke();
        RefreshMoney();
        HandleSellComplete();
    }

    // 执行一次出售
    private void ExecuteSellOnce(CharacterData player)
    {
        InventoryDictionary.Instance.GetOrCreate(player.id).RemoveItem(selected_item_id, 1);
        player.money += selected_price;
    }

    // 出售后的通用处理
    private void HandleSellComplete()
    {
        bool still_exists = InventoryDictionary.Instance.GetOrCreate(Player.Instance.GetCharacter().id).GetCount(selected_item_id) > 0;
        if (still_exists)
        {
            RecalculateSellPrice();
            UpdateItemButton(selected_button, selected_item_id, selected_price);
            ShowDetail(selected_item_id);
        }
        else
        {
            DeselectItem();
            Refresh();
        }
    }

    // 更新单个物品按钮的显示文本（买入/卖出后调用，不重建列表）
    private void UpdateItemButton(Button button, int item_id, int price)
    {
        TextMeshProUGUI text_num = button.transform.Find("Text_Num")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI text_value = button.transform.Find("Text_Value")?.GetComponent<TextMeshProUGUI>();
        if (text_num == null || text_value == null)
        {
            return;
        }

        // 重新统计数量
        int count = 0;
        CharacterData player = Player.Instance.GetCharacter();
        if (is_buy_mode && current_trade_scene != null)
        {
            count = current_trade_scene.GetStock(item_id);
        }
        else if (!is_buy_mode && player != null)
        {
            count = InventoryDictionary.Instance.GetOrCreate(player.id).GetCount(item_id);
        }

        // 更新数量文本
        if (count > 0)
        {
            text_num.text = "×" + count;
        }
        else
        {
            text_num.text = "";
        }

        // 更新价格文本
        ItemData template = ItemDictionary.Instance.Get(item_id);
        if (template != null)
        {
            int base_value = template.base_value;
            Color price_color = GetPriceColor(price, base_value);
            string price_hex = ColorToHex(price_color);
            text_value.text = "<color=#" + price_hex + ">" + price + "</color>(" + base_value + ")";
        }
    }

    // 将价格限制在 [基准 × 下限, 基准 × 上限] 范围内
    public static int ClampPrice(int price, int base_value)
    {
        Config config = Config.Instance;
        int floor = Mathf.RoundToInt(base_value * config.price_floor_ratio);
        int cap = Mathf.RoundToInt(base_value * config.price_cap_ratio);
        if (price < floor) price = floor;
        if (price > cap) price = cap;
        return price;
    }

    // 根据价格与价值的比值获取着色颜色，且应用价格上下限
    public static Color GetPriceColor(int price, int base_value)
    {
        if (base_value <= 0)
        {
            return Color.white;
        }

        float ratio = (float)price / (float)base_value;
        Config config = Config.Instance;

        if (ratio <= config.price_color_deep_green)
        {
            return new Color(0f, 0.5f, 0f);
        }
        else if (ratio <= config.price_color_light_green)
        {
            return new Color(0.4f, 0.8f, 0.2f);
        }
        else if (ratio < config.price_color_orange)
        {
            return Color.white;
        }
        else if (ratio < config.price_color_red)
        {
            return new Color(1f, 0.6f, 0f);
        }
        else
        {
            return Color.red;
        }
    }

    // Color 转 Hex 字符串（用于 Rich Text 着色标签）
    public static string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255f);
        int g = Mathf.RoundToInt(color.g * 255f);
        int b = Mathf.RoundToInt(color.b * 255f);
        return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
    }

    // 品级数字转中文（1→仙一品 ... 9→劣九品）
    private string GetGradeName(int grade)
    {
        if (grade >= 1 && grade <= 9)
        {
            return grade_names[grade - 1];
        }
        return "";
    }

    // 类型枚举转中文
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

    // 效果枚举转中文
    private string GetEffectName(ItemEffectType type)
    {
        int index = (int)type;
        if (index >= 0 && index < effect_names.Length)
        {
            return effect_names[index];
        }
        return "";
    }

    // 构建限制后缀——如"(不可使用，不可丢弃)"
    private string BuildRestrictionSuffix(ItemData template)
    {
        string suffix = "";
        if (!template.can_use)
        {
            suffix += "不可使用";
        }
        if (!template.can_discard)
        {
            if (suffix.Length > 0) suffix += "，";
            suffix += "不可丢弃";
        }
        if (!template.can_trade)
        {
            if (suffix.Length > 0) suffix += "，";
            suffix += "不可交易";
        }
        if (suffix.Length > 0)
        {
            return "(" + suffix + ")";
        }
        return "";
    }
}
