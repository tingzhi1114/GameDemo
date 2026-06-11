using System.Collections.Generic;

/// <summary>
/// 背包——管理一个角色持有的物品
/// 外层 key=物品ID, 内层 key=ItemData实例（可堆叠物品为模板本身，不可堆叠物品为独立副本）, value=数量
/// </summary>
public class Inventory
{
    public Dictionary<int, Dictionary<ItemData, int>> items;

    public Inventory()
    {
        this.items = new Dictionary<int, Dictionary<ItemData, int>>();
    }

    /// <summary>
    /// 添加指定数量的物品到背包中
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
        if (this.items.ContainsKey(item_id))
        {
            slot = this.items[item_id];
        }
        else
        {
            slot = new Dictionary<ItemData, int>();
            this.items[item_id] = slot;
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
                slot[template.Clone()] = count;
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

        if (!this.items.ContainsKey(item_id))
        {
            return;
        }

        Dictionary<ItemData, int> slot = this.items[item_id];
        int remaining = count;

        List<ItemData> to_remove = new List<ItemData>();
        foreach (ItemData key in slot.Keys)
        {
            int current = slot[key];
            if (current <= remaining)
            {
                to_remove.Add(key);
                remaining -= current;
            }
            else
            {
                slot[key] = current - remaining;
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
            slot.Remove(to_remove[i]);
        }

        if (slot.Count == 0)
        {
            this.items.Remove(item_id);
        }
    }

    /// <summary>
    /// 获取某物品的持有数量
    /// </summary>
    public int GetCount(int item_id)
    {
        if (!this.items.ContainsKey(item_id))
        {
            return 0;
        }

        int total = 0;
        foreach (int cnt in this.items[item_id].Values)
        {
            total += cnt;
        }
        return total;
    }
}
