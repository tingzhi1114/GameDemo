using System.Collections.Generic;

/// <summary>
/// 场景数据模型——地点内部的子区域
/// parent_scene_id = -1 表示顶级场景
/// </summary>
public class SceneData
{
    // 场景ID
    public int id;
    // 所属大地图节点ID
    public int location_id;
    // 上级场景ID（-1表示顶级场景）
    public int parent_scene_id;
    // 子场景ID列表（双向链表，方便查询）
    public List<int> children_ids;
    // 场景模板ID（指向SceneTemplateDictionary）
    public int template_id;
    // 场景名称（如"秦淮河畔"）
    public string name;
    // 当前在此场景的角色ID列表（运行时动态数据，不由数据字典初始化）
    public List<int> character_ids;
    // 商店库存（key=物品ID, value=inner dict，和角色背包结构一致）
    public Dictionary<int, Dictionary<ItemData, int>> inventory;
    // 期望库存（key=物品ID, value=目标数量，补货逻辑参考此值）
    public Dictionary<int, int> target_stock;

    public SceneData(int id, int location_id, string name, int parent_scene_id = -1, int template_id = -1, List<int> children_ids = null, Dictionary<int, Dictionary<ItemData, int>> inventory = null, Dictionary<int, int> target_stock = null)
    {
        this.id = id;
        this.location_id = location_id;
        this.name = name;
        this.parent_scene_id = parent_scene_id;
        this.template_id = template_id;
        this.children_ids = children_ids;
        this.character_ids = new List<int>();
        this.inventory = inventory;
        this.target_stock = target_stock;
    }

    /// <summary>
    /// 获取商店中某物品的库存数量
    /// </summary>
    public int GetStock(int item_id)
    {
        if (this.inventory == null || !this.inventory.ContainsKey(item_id))
        {
            return 0;
        }

        int total = 0;
        foreach (int cnt in this.inventory[item_id].Values)
        {
            total += cnt;
        }
        return total;
    }

    /// <summary>
    /// 向商店库存中添加指定数量的物品
    /// </summary>
    public void AddStock(int item_id, int count)
    {
        if (count <= 0)
        {
            return;
        }

        if (this.inventory == null)
        {
            this.inventory = new Dictionary<int, Dictionary<ItemData, int>>();
        }

        ItemData template = ItemDictionary.Instance.Get(item_id);
        if (template == null)
        {
            return;
        }

        if (this.inventory.ContainsKey(item_id))
        {
            // 存在时往第一项叠加数量
            foreach (ItemData key in this.inventory[item_id].Keys)
            {
                this.inventory[item_id][key] += count;
                return;
            }
        }

        // 新物品条目
        Dictionary<ItemData, int> inner = new Dictionary<ItemData, int>();
        inner[template] = count;
        this.inventory[item_id] = inner;
    }

    /// <summary>
    /// 从商店库存中扣减指定数量的物品
    /// </summary>
    public void RemoveStock(int item_id, int count)
    {
        if (this.inventory == null || !this.inventory.ContainsKey(item_id) || count <= 0)
        {
            return;
        }

        Dictionary<ItemData, int> inner = this.inventory[item_id];
        int remaining = count;

        List<ItemData> to_remove = new List<ItemData>();
        foreach (ItemData key in inner.Keys)
        {
            int current = inner[key];
            if (current <= remaining)
            {
                to_remove.Add(key);
                remaining -= current;
            }
            else
            {
                inner[key] = current - remaining;
                remaining = 0;
                break;
            }
            if (remaining <= 0)
            {
                break;
            }
        }

        for (int i = 0; i < to_remove.Count; i++)
        {
            inner.Remove(to_remove[i]);
        }

        if (inner.Count == 0)
        {
            this.inventory.Remove(item_id);
        }
    }
}
