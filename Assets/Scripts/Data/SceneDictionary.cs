using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景数据字典——从 Scenes.json 加载所有场景
/// </summary>
public class SceneDictionary : Singleton<SceneDictionary>
{
    // 所有场景（key=场景ID）
    private Dictionary<int, SceneData> all_scenes;
    // 下一个可用的自增ID
    private int next_id;

    private SceneDictionary()
    {
        this.all_scenes = new Dictionary<int, SceneData>();

        // 从 Resources/Data/Scenes.json 加载场景数据
        TextAsset json_text = Resources.Load<TextAsset>("Data/Scenes");
        if (json_text == null)
        {
            Debug.LogError("Scenes.json 未找到，请确保 Assets/Resources/Data/Scenes.json 存在");
            return;
        }

        SceneDataListJSON database = JsonUtility.FromJson<SceneDataListJSON>(json_text.text);
        if (database == null || database.scenes == null)
        {
            Debug.LogError("Scenes.json 解析失败，请检查 JSON 格式");
            return;
        }

        // 遍历JSON条目，逐条解析为SceneData
        for (int i = 0; i < database.scenes.Count; i++)
        {
            SceneDataJSON entry = database.scenes[i];

            // 解析商品数据，同时得到库存字典和期望库存字典
            Dictionary<int, Dictionary<ItemData, int>> inv = null;
            Dictionary<int, int> target = null;
            ParseGoods(entry.goods, out inv, out target);

            SceneData scene = new SceneData(
                id: entry.id,
                location_id: entry.location_id,
                name: entry.name,
                parent_scene_id: entry.parent_scene_id,
                template_id: entry.template_id,
                children_ids: entry.children_ids,
                inventory: inv,
                target_stock: target
            );
            this.all_scenes[scene.id] = scene;
        }

        // 以JSON中最大ID为基础，后续新增场景的ID递增
        int max_id = 0;
        foreach (int key in this.all_scenes.Keys)
        {
            if (key > max_id)
            {
                max_id = key;
            }
        }
        this.next_id = max_id + 1;

        Debug.Log("SceneDictionary: 从 Scenes.json 加载了 " + this.all_scenes.Count + " 个场景");
    }

    // 将 JSON 中的 goods 列表解析为商店库存字典 + 期望库存字典
    private void ParseGoods(List<SceneGoodsJSON> goods, out Dictionary<int, Dictionary<ItemData, int>> inventory, out Dictionary<int, int> targetStock)
    {
        inventory = null;
        targetStock = null;

        if (goods == null || goods.Count == 0)
        {
            return;
        }

        inventory = new Dictionary<int, Dictionary<ItemData, int>>();
        targetStock = new Dictionary<int, int>();

        for (int i = 0; i < goods.Count; i++)
        {
            int item_id = goods[i].item_id;
            int target_qty = goods[i].target;

            ItemData template = ItemDictionary.Instance.Get(item_id);
            if (template == null)
            {
                continue;
            }

            // 初始化库存到目标数量
            Dictionary<ItemData, int> inner = new Dictionary<ItemData, int>();
            inner[template] = target_qty;
            inventory[item_id] = inner;

            // 记录期望库存
            targetStock[item_id] = target_qty;
        }
    }

    /// <summary>
    /// 根据ID获取场景，找不到返回null
    /// </summary>
    public SceneData Get(int id)
    {
        if (this.all_scenes.ContainsKey(id))
        {
            return this.all_scenes[id];
        }
        return null;
    }

    /// <summary>
    /// 添加一个场景到字典中，自动分配自增ID
    /// </summary>
    public int Add(SceneData scene)
    {
        int new_id = this.next_id;
        this.next_id++;
        scene.id = new_id;
        this.all_scenes[new_id] = scene;
        return new_id;
    }

    /// <summary>
    /// 移除指定ID的场景
    /// </summary>
    public void Remove(int id)
    {
        // 从所有场景的子场景列表中移除对它的引用
        foreach (SceneData existing in this.all_scenes.Values)
        {
            if (existing.children_ids != null)
            {
                existing.children_ids.Remove(id);
            }
        }
        this.all_scenes.Remove(id);
    }
}
