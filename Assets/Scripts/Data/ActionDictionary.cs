using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行动字典——从 Actions.json 加载所有行动定义
/// </summary>
public class ActionDictionary : Singleton<ActionDictionary>
{
    // 所有行动（key=行动ID）
    private Dictionary<int, ActionData> all_actions;

    private ActionDictionary()
    {
        this.all_actions = new Dictionary<int, ActionData>();

        // 从 Resources/Data/Actions.json 加载行动数据
        TextAsset json_text = Resources.Load<TextAsset>("Data/Actions");
        if (json_text == null)
        {
            Debug.LogError("Actions.json 未找到，请确保 Assets/Resources/Data/Actions.json 存在");
            return;
        }

        ActionDataListJSON database = JsonUtility.FromJson<ActionDataListJSON>(json_text.text);
        if (database == null || database.actions == null)
        {
            Debug.LogError("Actions.json 解析失败，请检查 JSON 格式");
            return;
        }

        // 遍历JSON条目，逐条解析为ActionData
        for (int i = 0; i < database.actions.Count; i++)
        {
            ActionDataJSON entry = database.actions[i];
            ActionData action = new ActionData(
                id: entry.id,
                name: entry.name,
                effect_types: ParseEffectTypes(entry.effects),
                effect_contexts: ParseEffectContexts(entry.effects)
            );
            this.all_actions[action.id] = action;
        }

        Debug.Log("ActionDictionary: 从 Actions.json 加载了 " + this.all_actions.Count + " 个行动");
    }

    /// <summary>
    /// 将 JSON 中的 effects 列表解析为效果类型列表
    /// </summary>
    private List<ActionEffectTypeEnum> ParseEffectTypes(List<ActionEffectJSON> effects)
    {
        if (effects == null || effects.Count == 0)
        {
            return null;
        }

        List<ActionEffectTypeEnum> types = new List<ActionEffectTypeEnum>();
        for (int i = 0; i < effects.Count; i++)
        {
            ActionEffectTypeEnum effect_type = (ActionEffectTypeEnum)0;
            Enum.TryParse(effects[i].type, out effect_type);
            types.Add(effect_type);
        }
        return types;
    }

    /// <summary>
    /// 将 JSON 中的 effects 列表解析为效果参数列表
    /// </summary>
    private List<ActionEffectContext> ParseEffectContexts(List<ActionEffectJSON> effects)
    {
        if (effects == null || effects.Count == 0)
        {
            return null;
        }

        List<ActionEffectContext> contexts = new List<ActionEffectContext>();
        for (int i = 0; i < effects.Count; i++)
        {
            ActionEffectJSON entry = effects[i];
            ActionEffectContext context = new ActionEffectContext();

            // 填入各字段
            context.money_change = entry.money_change;
            context.health_change = entry.health_change;
            context.energy_change = entry.energy_change;
            context.fullness_change = entry.fullness_change;
            context.item_id = entry.item_id;
            context.item_count = entry.item_count;
            context.panel_name = entry.panel_name;
            context.event_id = entry.event_id;
            context.log_message = entry.log_message;
            context.advance_slots = entry.advance_slots;

            // 解析属性变化列表（如果有）
            if (entry.attribute_changes != null && entry.attribute_changes.Count > 0)
            {
                context.attribute_changes = new Dictionary<AttributeTypeEnum, float>();
                for (int j = 0; j < entry.attribute_changes.Count; j++)
                {
                    AttributeTypeEnum attr_type = AttributeTypeEnum.Strength;
                    Enum.TryParse(entry.attribute_changes[j].type, out attr_type);
                    context.attribute_changes[attr_type] = entry.attribute_changes[j].value;
                }
            }

            contexts.Add(context);
        }
        return contexts;
    }

    /// <summary>
    /// 根据ID获取行动，找不到返回null
    /// </summary>
    public ActionData Get(int id)
    {
        if (this.all_actions.ContainsKey(id))
        {
            return this.all_actions[id];
        }
        return null;
    }
}
