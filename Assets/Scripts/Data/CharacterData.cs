using System.Collections.Generic;

/// <summary>
/// 角色数据模型——存储一个角色的核心信息
/// </summary>
public class CharacterData
{
    // 角色唯一ID
    public int id;
    // 姓名
    public string name;
    // 性别（0 = 女, 1 = 男）
    public int gender;
    // 年龄
    public int age;
    // 当前所在地点ID（-1表示不在任何地点）
    public int current_location_id;
    // 当前场景ID（-1表示在大地图）
    public int current_scene_id;

    // 六维属性字典
    public Dictionary<AttributeTypeEnum, float> attributes;

    // 健康值及上限
    public float health;
    public float max_health;
    // 精力值及上限
    public float energy;
    public float max_energy;
    // 饱腹值及上限
    public float fullness;
    public float max_fullness;
    // 金钱
    public int money;
    // 背包——外层key=物品ID, 内层key=物品实例副本, value=数量
    // 可堆叠物品：inner dict 只有1条（{ 模板实例: 数量 }）
    // 不可堆叠物品：inner dict 每条都是独立副本（{ 副本A: 1, 副本B: 1 }）
    public Dictionary<int, Dictionary<ItemData, int>> inventory;

    public CharacterData(int id, string name, int gender, int age, Dictionary<AttributeTypeEnum, float> attributes, int current_location_id, int current_scene_id = -1)
    {
        this.id = id;
        this.name = name;
        this.gender = gender;
        this.age = age;
        this.attributes = attributes;
        this.current_location_id = current_location_id;
        this.current_scene_id = current_scene_id;

        // 默认状态值均为100
        this.health = 100f;
        this.max_health = 100f;
        this.energy = 100f;
        this.max_energy = 100f;
        this.fullness = 100f;
        this.max_fullness = 100f;
        // 初始金钱
        this.money = 100;
        // 初始化背包
        this.inventory = new Dictionary<int, Dictionary<ItemData, int>>();
    }

    /// <summary>
    /// 进入指定场景——自动从旧场景移除并加入新场景
    /// </summary>
    public void EnterScene(int scene_id)
    {
        // 从旧场景移除
        SceneData old_scene = SceneDictionary.Instance.Get(this.current_scene_id);
        if (old_scene != null)
        {
            old_scene.character_ids.Remove(this.id);
        }

        // 更新当前场景ID
        this.current_scene_id = scene_id;

        // 加入新场景
        SceneData new_scene = SceneDictionary.Instance.Get(scene_id);
        if (new_scene != null)
        {
            new_scene.character_ids.Add(this.id);
        }
    }

    /// <summary>
    /// 修改某项属性值（加上delta，负数为减少）
    /// </summary>
    public void ModifyAttribute(AttributeTypeEnum type, float delta)
    {
        if (this.attributes.ContainsKey(type))
        {
            this.attributes[type] += delta;
        }
    }

    /// <summary>
    /// 根据饱腹值占上限的比值返回对应的状态文字
    /// </summary>
    public string GetFullnessStatus()
    {
        float ratio = this.fullness / this.max_fullness;

        if (ratio <= 0.1f)
        {
            return "快饿死了";
        }
        else if (ratio <= 0.25f)
        {
            return "饥饿";
        }
        else if (ratio <= 0.45f)
        {
            return "有点饿了";
        }
        else if (ratio <= 0.65f)
        {
            return "良好";
        }
        else if (ratio <= 0.8f)
        {
            return "有点饱了";
        }
        else if (ratio <= 0.95f)
        {
            return "饱了";
        }
        else
        {
            return "撑了";
        }
    }

    /// <summary>
    /// 向背包中添加指定数量的物品
    /// </summary>
    public void AddItem(int item_id, int count)
    {
        if (count <= 0)
        {
            return;
        }

        ItemData template = ItemDictionary.Instance.Get(item_id);
        if (template == null)
        {
            return;
        }

        // 获取或创建该物品ID的inner dict
        Dictionary<ItemData, int> slot;
        if (this.inventory.ContainsKey(item_id))
        {
            slot = this.inventory[item_id];
        }
        else
        {
            slot = new Dictionary<ItemData, int>();
            this.inventory[item_id] = slot;
        }

        if (template.max_stack > 1)
        {
            // 可堆叠：找已有条目合并，没有则新建
            bool found = false;
            foreach (ItemData key in slot.Keys)
            {
                slot[key] = slot[key] + count;
                found = true;
                break;
            }
            if (!found)
            {
                slot[template] = count;
            }
        }
        else
        {
            // 不可堆叠：每个副本独立实例
            for (int i = 0; i < count; i++)
            {
                ItemData copy = template.Clone();
                slot[copy] = 1;
            }
        }
    }

    /// <summary>
    /// 从背包中移除指定数量的物品
    /// </summary>
    public void RemoveItem(int item_id, int count)
    {
        if (count <= 0)
        {
            return;
        }

        if (!this.inventory.ContainsKey(item_id))
        {
            return;
        }

        Dictionary<ItemData, int> slot = this.inventory[item_id];
        int remaining = count;

        // 遍历inner dict，优先扣可堆叠项的count
        List<ItemData> to_remove = new List<ItemData>();
        foreach (ItemData key in slot.Keys)
        {
            int current = slot[key];
            if (current <= remaining)
            {
                // 当前项不够扣或刚好够，整条移除
                to_remove.Add(key);
                remaining = remaining - current;
            }
            else
            {
                // 当前项足够，只减数量
                slot[key] = current - remaining;
                remaining = 0;
                break;
            }

            if (remaining <= 0)
            {
                break;
            }
        }

        // 移除标记的条目
        for (int i = 0; i < to_remove.Count; i++)
        {
            slot.Remove(to_remove[i]);
        }

        // 如果inner dict空了，移除外层key
        if (slot.Count == 0)
        {
            this.inventory.Remove(item_id);
        }
    }
}
