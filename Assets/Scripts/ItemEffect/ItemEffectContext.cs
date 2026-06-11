/// <summary>
/// 物品效果参数——存储单个物品效果所需的全部参数
/// </summary>
public class ItemEffectContext
{
    // 效果类型
    public ItemEffectType type;
    // 效果数值（正数=增益，负数=减益）
    public float value;

    public ItemEffectContext(ItemEffectType type, float value)
    {
        this.type = type;
        this.value = value;
    }
}
