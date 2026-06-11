/// <summary>
/// 全局配置——从 Config.json 加载，所有硬编码常量集中管理
/// </summary>
public class Config : Singleton<Config>
{
    // 全局卖出倍率（出售价 = 买入价 × 此值）
    public float sell_rate;

    private Config()
    {
        UnityEngine.TextAsset json_text = UnityEngine.Resources.Load<UnityEngine.TextAsset>("Data/Config");
        if (json_text != null)
        {
            ConfigJSON data = UnityEngine.JsonUtility.FromJson<ConfigJSON>(json_text.text);
            if (data != null)
            {
                this.sell_rate = data.sell_rate;
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
}
