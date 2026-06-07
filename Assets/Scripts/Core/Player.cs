/// <summary>
/// 玩家单例——持有角色ID，通过ID到CharacterDictionary中查找对应的角色
/// </summary>
public class Player : Singleton<Player>
{
    // 玩家对应的角色ID（指向CharacterDictionary中的某个CharacterData）
    public int character_id;

    private Player()
    {
        this.character_id = -1;
    }

    /// <summary>
    /// 获取玩家对应的角色数据
    /// </summary>
    public CharacterData Get()
    {
        return CharacterDictionary.Instance.Get(this.character_id);
    }
}
