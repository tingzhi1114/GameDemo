using System.Collections.Generic;

/// <summary>
/// 行动字典——存放所有可用的行动定义
/// </summary>
public class ActionDictionary : Singleton<ActionDictionary>
{
    // 所有行动（key=行动ID）
    private Dictionary<int, ActionData> all_actions;

    private ActionDictionary()
    {
        this.all_actions = new Dictionary<int, ActionData>()
        {
            {
                1,
                new ActionData(
                    id: 1,
                    name: "购买交易品"
                )
            },
            {
                2,
                new ActionData(
                    id: 2,
                    name: "贩卖交易品"
                )
            },
            {
                3,
                new ActionData(
                    id: 3,
                    name: "用膳"
                )
            },
            {
                4,
                new ActionData(
                    id: 4,
                    name: "住宿"
                )
            },
            {
                5,
                new ActionData(
                    id: 5,
                    name: "打工"
                )
            },
            {
                6,
                new ActionData(
                    id: 6,
                    name: "舍饭"
                )
            },
            {
                7,
                new ActionData(
                    id: 7,
                    name: "借宿"
                )
            },
            {
                8,
                new ActionData(
                    id: 8,
                    name: "闲逛"
                )
            }
        };
    }

    /// <summary>
    /// 添加一个行动到字典中
    /// </summary>
    public void Add(ActionData action)
    {
        this.all_actions[action.id] = action;
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

    /// <summary>
    /// 移除指定ID的行动
    /// </summary>
    public void Remove(int id)
    {
        if (this.all_actions.ContainsKey(id))
        {
            this.all_actions.Remove(id);
        }
    }

    /// <summary>
    /// 获取所有行动（返回副本）
    /// </summary>
    public List<ActionData> GetAll()
    {
        return new List<ActionData>(this.all_actions.Values);
    }
}
