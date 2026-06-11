using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界演化器——每推进一个时段时，对所有角色执行状态衰减并触发商店补货
/// 由 EventManager 在 TimeManager.OnTimeAdvanced 触发时调用
/// </summary>
public class WorldSimulator : Singleton<WorldSimulator>
{
    // 补货计数器，每时段+1，达到阈值后补货
    private int restock_counter;

    private WorldSimulator()
    {
        this.restock_counter = 0;
    }

    /// <summary>
    /// 每时段推进时调用
    /// </summary>
    public void OnTimeAdvanced()
    {
        ApplyDecayToAll();
        TickRestock();
    }

    // 对所有角色执行状态衰减
    private void ApplyDecayToAll()
    {
        Config config = Config.Instance;

        foreach (CharacterData character in CharacterDictionary.Instance.GetAll())
        {
            character.energy -= config.energy_decay_per_slot;
            if (character.energy < 0f)
            {
                character.energy = 0f;
            }

            character.fullness -= config.fullness_decay_per_slot;
            if (character.fullness < 0f)
            {
                character.fullness = 0f;
            }

            if (character.fullness <= 0f)
            {
                character.health -= config.health_decay_when_starving;
                if (character.health < 0f)
                {
                    character.health = 0f;
                }
            }
        }
    }

    // 补货计数器：每时段+1，满 interval 时对所有场景补货
    private void TickRestock()
    {
        Config config = Config.Instance;
        this.restock_counter++;

        if (this.restock_counter >= config.restock_interval_slots)
        {
            this.restock_counter = 0;
            RestockAllScenes();
        }
    }

    // 对所有有 target_stock 的场景执行补货/扣货
    private void RestockAllScenes()
    {
        Config config = Config.Instance;

        foreach (SceneData scene in SceneDictionary.Instance.GetAll())
        {
            if (scene.target_stock == null || scene.target_stock.Count == 0)
            {
                continue;
            }

            foreach (KeyValuePair<int, int> kv in scene.target_stock)
            {
                int item_id = kv.Key;
                int target = kv.Value;
                int current = scene.GetStock(item_id);
                int diff = target - current;

                // 没偏差 → 跳过
                if (diff == 0)
                {
                    continue;
                }

                // 获取该物品的弹性系数
                float elasticity = 0f;
                ItemData template = ItemDictionary.Instance.Get(item_id);
                if (template != null)
                {
                    elasticity = template.elasticity;
                }

                if (diff > 0)
                {
                    // 补货量受弹性系数影响：弹性0=25%，弹性1=50%，线性增长
                    float restock_rate = config.restock_percent + (0.50f - 0.25f) * Mathf.Min(elasticity, 1.0f);
                    int adjust = Mathf.Max(1, Mathf.RoundToInt(diff * restock_rate));

                    // 补货，但不超出 target
                    int actual_add = Mathf.Min(adjust, diff);
                    if (actual_add > 0)
                    {
                        scene.AddStock(item_id, actual_add);
                    }
                }
                else
                {
                    // 扣货量固定为 25%，不高于弹性系数影响
                    int adjust = Mathf.Max(1, Mathf.RoundToInt(Mathf.Abs(diff) * config.restock_percent));

                    // 扣货，但不低于 target
                    int actual_remove = Mathf.Min(adjust, current - target);
                    if (actual_remove > 0)
                    {
                        scene.RemoveStock(item_id, actual_remove);
                    }
                }
            }
        }
    }
}
