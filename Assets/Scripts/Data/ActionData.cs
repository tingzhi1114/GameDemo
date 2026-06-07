/// <summary>
/// 行动数据——定义玩家在场景中可执行的一个操作
/// </summary>
public class ActionData
{
    // 行动ID
    public int id;
    // 行动名称（显示在按钮上的文字，如"交谈"、"购物"）
    public string name;

    public ActionData(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
}
