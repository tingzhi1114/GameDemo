/// <summary>
/// 启动管线——在游戏启动时按依赖顺序显式初始化所有单例
/// 各 XXXDictionary 的构造器之间有隐式依赖（部分字典在构造中访问 ItemDictionary），
/// 显式初始化可以保证顺序可控、依赖明确、出问题第一时间暴露。
/// </summary>
public static class Bootstrap
{
    /// <summary>
    /// 按依赖顺序初始化所有数据字典
    /// </summary>
    public static void Initialize()
    {
        // 第0层：零依赖
        Config.EnsureCreated();
        ItemDictionary.EnsureCreated();

        // 第1层：依赖 ItemDictionary
        ActionDictionary.EnsureCreated();
        SceneTemplateDictionary.EnsureCreated();
        LocationDictionary.EnsureCreated();
        InventoryDictionary.EnsureCreated();

        // 第2层：依赖 ItemDictionary
        SceneDictionary.EnsureCreated();

        // 第3层：零依赖（运行时角色数据）
        CharacterDictionary.EnsureCreated();
    }
}
