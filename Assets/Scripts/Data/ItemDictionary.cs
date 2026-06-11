using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品数据字典——从 Items.json 加载所有物品定义
/// </summary>
public class ItemDictionary : Singleton<ItemDictionary>
{
    // 所有物品（key=物品ID）
    private Dictionary<int, ItemData> all_items;
    // 下一个可用的自增ID
    private int next_id;

    private ItemDictionary()
    {
        this.all_items = new Dictionary<int, ItemData>();

        // 从 Resources/Data/Items.json 加载物品数据
        TextAsset json_text = Resources.Load<TextAsset>("Data/Items");
        if (json_text == null)
        {
            Debug.LogError("Items.json 未找到，请确保 Assets/Resources/Data/Items.json 存在");
            return;
        }

        ItemDataListJSON database = JsonUtility.FromJson<ItemDataListJSON>(json_text.text);
        if (database == null || database.items == null)
        {
            Debug.LogError("Items.json 解析失败，请检查 JSON 格式");
            return;
        }

        // 遍历JSON条目，逐条解析为ItemData
        for (int i = 0; i < database.items.Count; i++)
        {
            ItemDataJSON entry = database.items[i];
            ItemData item = ParseEntry(entry);
            if (item != null)
            {
                this.all_items[item.id] = item;
            }
        }

        // 以JSON中最大ID为基础，后续新增物品的ID递增
        int max_id = 0;
        foreach (int key in this.all_items.Keys)
        {
            if (key > max_id)
            {
                max_id = key;
            }
        }
        this.next_id = max_id + 1;

        Debug.Log("ItemDictionary: 从 Items.json 加载了 " + this.all_items.Count + " 个物品");
    }

    /// <summary>
    /// 将一条 JSON 条目解析为 ItemData
    /// </summary>
    private ItemData ParseEntry(ItemDataJSON entry)
    {
        // 将字符串type解析为枚举，解析失败默认Special
        ItemTypeEnum item_type = ItemTypeEnum.Special;
        Enum.TryParse(entry.type, out item_type);

        // 将字符串sub_type解析为枚举，解析失败默认Food
        ItemSubTypeEnum item_sub_type = (ItemSubTypeEnum)0;
        if (!string.IsNullOrEmpty(entry.sub_type))
        {
            Enum.TryParse(entry.sub_type, out item_sub_type);
        }

        // 将 properties 列表转为字典
        Dictionary<ItemPropertyType, float> prop_dict = null;
        if (entry.properties != null && entry.properties.Count > 0)
        {
            prop_dict = new Dictionary<ItemPropertyType, float>();
            for (int i = 0; i < entry.properties.Count; i++)
            {
                ItemPropertyType prop_type = (ItemPropertyType)0;
                Enum.TryParse(entry.properties[i].type, out prop_type);
                prop_dict[prop_type] = entry.properties[i].value;
            }
        }

        // 将 effects 列表转为字典
        Dictionary<ItemEffectType, float> effect_dict = null;
        if (entry.effects != null && entry.effects.Count > 0)
        {
            effect_dict = new Dictionary<ItemEffectType, float>();
            for (int i = 0; i < entry.effects.Count; i++)
            {
                ItemEffectType effect_type = ItemEffectType.ModifyMoney;
                Enum.TryParse(entry.effects[i].type, out effect_type);
                effect_dict[effect_type] = entry.effects[i].value;
            }
        }

        return new ItemData(
            id: entry.id,
            name: entry.name,
            description: entry.description,
            type: item_type,
            sub_type: item_sub_type,
            grade: entry.grade,
            base_value: entry.base_value,
            max_stack: entry.max_stack,
            properties: prop_dict,
            effects: effect_dict,
            can_use: entry.can_use,
            can_discard: entry.can_discard,
            can_trade: entry.can_trade,
            elasticity: entry.elasticity
        );
    }

    /// <summary>
    /// 添加一个物品到字典中，自动分配自增ID
    /// </summary>
    public int Add(ItemData item)
    {
        int new_id = this.next_id;
        this.next_id++;
        item.id = new_id;
        this.all_items[new_id] = item;
        return new_id;
    }

    /// <summary>
    /// 根据ID获取物品，找不到返回null
    /// </summary>
    public ItemData Get(int id)
    {
        if (this.all_items.ContainsKey(id))
        {
            return this.all_items[id];
        }
        return null;
    }
}
