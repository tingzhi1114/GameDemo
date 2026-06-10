using UnityEngine;

/// <summary>
/// 行动效果执行器——单例，根据行动ID和执行者ID执行对应的效果逻辑
/// 遍历 ActionData 中的 effect_types 列表，分发给对应的效果类执行
/// </summary>
public class ActionEffect : Singleton<ActionEffect>
{
    private ActionEffect()
    {
    }

    /// <summary>
    /// 执行指定ID的行动效果（传入行动ID和执行者角色ID）
    /// </summary>
    public void Execute(int action_id, int character_id)
    {
        CharacterData character = CharacterDictionary.Instance.Get(character_id);
        if (character == null)
        {
            Debug.LogError("角色不存在，无法执行行动");
            return;
        }

        ActionData action = ActionDictionary.Instance.Get(action_id);
        if (action == null)
        {
            Debug.LogError("行动不存在，action_id=" + action_id);
            return;
        }

        // 如果没有配置效果，直接返回
        if (action.effect_types == null || action.effect_contexts == null)
        {
            return;
        }

        // 依次执行每个效果，如果某个效果返回false则中断后续效果
        for (int i = 0; i < action.effect_types.Count; i++)
        {
            ActionEffectTypeEnum type = action.effect_types[i];
            ActionEffectContext context = action.effect_contexts[i];

            bool success = DispatchEffect(type, context, character);
            if (!success)
            {
                break;
            }
        }
    }

    // 根据效果类型分派到对应的效果类执行
    private bool DispatchEffect(ActionEffectTypeEnum type, ActionEffectContext context, CharacterData character)
    {
        if (type == ActionEffectTypeEnum.ModifyMoney)
        {
            return ModifyMoneyAE.Execute(character, context);
        }
        else if (type == ActionEffectTypeEnum.ModifyStatus)
        {
            return ModifyStatusAE.Execute(character, context);
        }
        else if (type == ActionEffectTypeEnum.ModifyAttribute)
        {
            return ModifyAttributeAE.Execute(character, context);
        }
        else if (type == ActionEffectTypeEnum.GainItem)
        {
            return GainItemAE.Execute(character, context);
        }
        else if (type == ActionEffectTypeEnum.LoseItem)
        {
            return LoseItemAE.Execute(character, context);
        }
        else if (type == ActionEffectTypeEnum.OpenPanel)
        {
            return OpenPanelAE.Execute(context);
        }
        else if (type == ActionEffectTypeEnum.TriggerEvent)
        {
            return TriggerEventAE.Execute(context);
        }
        else if (type == ActionEffectTypeEnum.AdvanceTime)
        {
            return AdvanceTimeAE.Execute(context);
        }

        return true;
    }
}
