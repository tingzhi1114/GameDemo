/// <summary>
/// 全局配置——从 Config.json 加载，所有可调参数集中管理
/// </summary>
public class Config : Singleton<Config>
{
    // ====== 交易 ======
    public float sell_rate;                         // 全局卖出倍率

    // ====== 角色状态衰减 ======
    public float energy_decay_per_slot;             // 每时段精力衰减
    public float fullness_decay_per_slot;           // 每时段饱腹衰减
    public float health_decay_when_starving;        // 饱腹为0时每时段健康衰减

    // ====== 商店补货 ======
    public int restock_interval_slots;              // 补货间隔（时段数，15=半个月）
    public float restock_percent;                   // 每次补货/扣货比例（如0.25=25%）

    // ====== 价格 ======
    public float price_floor_ratio;                 // 最低价格 = 基准 × 此值（如0.1=10%）
    public float price_cap_ratio;                   // 最高价格 = 基准 × 此值（如5.0=500%）
    public float price_color_deep_green;            // 低于此比值显示深绿
    public float price_color_light_green;           // 低于此比值显示浅绿
    public float price_color_orange;                // 高于此比值显示橙色
    public float price_color_red;                   // 高于此比值显示红色
    public float elasticity_min;                    // 弹性系数下限
    public float elasticity_max;                    // 弹性系数上限
    public float gamma;                             // 指数平滑常数（默认0.4）

    private Config()
    {
        UnityEngine.TextAsset json_text = UnityEngine.Resources.Load<UnityEngine.TextAsset>("Data/Config");
        if (json_text != null)
        {
            ConfigJSON data = UnityEngine.JsonUtility.FromJson<ConfigJSON>(json_text.text);
            if (data != null)
            {
                this.sell_rate = data.sell_rate;
                this.energy_decay_per_slot = data.energy_decay_per_slot;
                this.fullness_decay_per_slot = data.fullness_decay_per_slot;
                this.health_decay_when_starving = data.health_decay_when_starving;
                this.restock_interval_slots = data.restock_interval_slots;
                this.restock_percent = data.restock_percent;
                this.price_floor_ratio = data.price_floor_ratio;
                this.price_cap_ratio = data.price_cap_ratio;
                this.price_color_deep_green = data.price_color_deep_green;
                this.price_color_light_green = data.price_color_light_green;
                this.price_color_orange = data.price_color_orange;
                this.price_color_red = data.price_color_red;
                this.elasticity_min = data.elasticity_min;
                this.elasticity_max = data.elasticity_max;
                this.gamma = data.gamma;
            }
        }
    }
}

/// <summary>
/// JSON反序列化辅助类——Config.json 根容器
/// </summary>
[System.Serializable]
public class ConfigJSON
{
    public float sell_rate;
    public float energy_decay_per_slot;
    public float fullness_decay_per_slot;
    public float health_decay_when_starving;
    public int restock_interval_slots;
    public float restock_percent;
    public float price_floor_ratio;
    public float price_cap_ratio;
    public float price_color_deep_green;
    public float price_color_light_green;
    public float price_color_orange;
    public float price_color_red;
    public float elasticity_min;
    public float elasticity_max;
    public float gamma;
}
