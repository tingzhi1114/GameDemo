using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包数据字典——管理所有角色的背包实例
/// 读写 CharacterData 的 inventory 时，应通过此字典访问
/// </summary>
public class InventoryDictionary : Singleton<InventoryDictionary>
{
    // 所有背包（key=角色ID）
    private Dictionary<int, Inventory> all_inventories;

    private InventoryDictionary()
    {
        this.all_inventories = new Dictionary<int, Inventory>();

        // 从 Resources/Data/Inventories.json 加载初始库存
        TextAsset json_text = Resources.Load<TextAsset>("Data/Inventories");
        if (json_text == null)
        {
            return;
        }

        InventoryDataListJSON database = JsonUtility.FromJson<InventoryDataListJSON>(json_text.text);
        if (database == null || database.inventories == null)
        {
            return;
        }

        for (int i = 0; i < database.inventories.Count; i++)
        {
            InventoryDataJSON entry = database.inventories[i];
            Inventory inv = GetOrCreate(entry.character_id);

            if (entry.items == null || entry.items.Count == 0)
            {
                continue;
            }

            for (int j = 0; j < entry.items.Count; j++)
            {
                inv.AddItem(entry.items[j].item_id, entry.items[j].count);
            }
        }

        Debug.Log("InventoryDictionary: 从 Inventories.json 加载了 " + database.inventories.Count + " 个角色的初始库存");
    }

    /// <summary>
    /// 获取或创建指定角色的背包
    /// </summary>
    public Inventory GetOrCreate(int character_id)
    {
        if (!this.all_inventories.ContainsKey(character_id))
        {
            this.all_inventories[character_id] = new Inventory();
        }
        return this.all_inventories[character_id];
    }

    /// <summary>
    /// 获取指定角色的背包，不存在返回 null
    /// </summary>
    public Inventory Get(int character_id)
    {
        if (this.all_inventories.ContainsKey(character_id))
        {
            return this.all_inventories[character_id];
        }
        return null;
    }

    /// <summary>
    /// 移除指定角色的背包（角色死亡时调用）
    /// </summary>
    public void Remove(int character_id)
    {
        this.all_inventories.Remove(character_id);
    }
}
