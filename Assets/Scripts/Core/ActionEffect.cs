using UnityEngine;

/// <summary>
/// 行动效果执行器——单例，根据行动ID和执行者ID执行对应的效果逻辑
/// </summary>
public class ActionEffect : Singleton<ActionEffect>
{
    /// <summary>
    /// 行动执行后触发，供UI面板刷新
    /// </summary>
    public static event System.Action OnActionExecuted;

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

        if (action_id == 1)
        {
            // 购买交易品：花费金钱，获得货物（暂用金钱模拟）
            if (character.money >= 20)
            {
                character.money = character.money - 20;
                Debug.Log(character.name + "在市集采购了一批货物，花费了20文钱。");
            }
            else
            {
                Debug.Log(character.name + "囊中羞涩，买不起任何交易品。");
            }
        }
        else if (action_id == 2)
        {
            // 贩卖交易品：出售货物获得金钱
            character.money = character.money + 25;
            Debug.Log(character.name + "将在市集采购的货物卖出，赚了25文钱。");
        }
        else if (action_id == 3)
        {
            // 用膳：花钱恢复饱腹
            if (character.money >= 5)
            {
                character.money = character.money - 5;
                character.fullness = character.fullness + 30f;
                if (character.fullness > character.max_fullness)
                {
                    character.fullness = character.max_fullness;
                }
                Debug.Log(character.name + "花5文钱吃了一顿饭，饱腹恢复了。");
            }
            else
            {
                Debug.Log(character.name + "连5文钱都拿不出来，只能咽了咽口水。");
            }
        }
        else if (action_id == 4)
        {
            // 住宿：花钱恢复精力，推进到次日清晨
            if (character.money >= 15)
            {
                character.money = character.money - 15;
                character.energy = character.max_energy;
                // 推进到第二天清晨
                int slots_to_morning = (6 - (int)TimeManager.Instance.current_period) + 1;
                TimeManager.Instance.AdvanceTime(slots_to_morning);
                Debug.Log(character.name + "花15文钱住了一晚，精力恢复饱满。");
            }
            else
            {
                Debug.Log(character.name + "连住店的钱都没有，只能在街头凑合一晚。");
            }
        }
        else if (action_id == 5)
        {
            // 打工：消耗精力赚钱
            character.money = character.money + 10;
            character.energy = character.energy - 20f;
            character.fullness = character.fullness - 10f;
            if (character.energy < 0f)
            {
                character.energy = 0f;
            }
            if (character.fullness < 0f)
            {
                character.fullness = 0f;
            }
            Debug.Log(character.name + "干了一天苦力，赚了10文钱。精力下降了。");
        }
        else if (action_id == 6)
        {
            // 舍饭：寺庙免费施粥，少量恢复饱腹
            character.fullness = character.fullness + 15f;
            if (character.fullness > character.max_fullness)
            {
                character.fullness = character.max_fullness;
            }
            Debug.Log("寺庙施舍了一碗粥给" + character.name + "，虽然不多但能充饥。");
        }
        else if (action_id == 7)
        {
            // 借宿：寺庙免费借宿，少量恢复精力
            character.energy = character.energy + 30f;
            if (character.energy > character.max_energy)
            {
                character.energy = character.max_energy;
            }
            Debug.Log(character.name + "在寺庙借宿了一晚，精力恢复了一些。");
        }
        else if (action_id == 8)
        {
            // 闲逛：消耗一些精力，可能碰到有趣的事（未来接事件系统）
            character.energy = character.energy - 5f;
            if (character.energy < 0f)
            {
                character.energy = 0f;
            }
            Debug.Log(character.name + "在城中四处闲逛，打发时间。");
        }

        // 行动执行完毕，通知所有订阅者刷新
        OnActionExecuted?.Invoke();
    }
}
