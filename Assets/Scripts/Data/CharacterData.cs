using System.Collections.Generic;

/// <summary>
/// 角色数据模型——存储一个角色的核心信息
/// </summary>
public class CharacterData
{
    // 角色唯一ID
    public int id;
    // 姓名
    public string name;
    // 性别（0 = 女, 1 = 男）
    public int gender;
    // 年龄
    public int age;
    // 当前所在地点ID（-1表示不在任何地点）
    public int current_location_id;
    // 当前场景ID（-1表示在大地图）
    public int current_scene_id;

    // 六维属性字典
    public Dictionary<AttributeTypeEnum, float> attributes;

    // 健康值及上限
    public float health;
    public float max_health;
    // 精力值及上限
    public float energy;
    public float max_energy;
    // 饱腹值及上限
    public float fullness;
    public float max_fullness;
    // 金钱
    public int money;

    public CharacterData(int id, string name, int gender, int age, Dictionary<AttributeTypeEnum, float> attributes, int current_location_id, int current_scene_id = -1)
    {
        this.id = id;
        this.name = name;
        this.gender = gender;
        this.age = age;
        this.attributes = attributes;
        this.current_location_id = current_location_id;
        this.current_scene_id = current_scene_id;

        // 默认状态值均为100
        this.health = 100f;
        this.max_health = 100f;
        this.energy = 100f;
        this.max_energy = 100f;
        this.fullness = 100f;
        this.max_fullness = 100f;
        // 初始金钱
        this.money = 100;
    }

    /// <summary>
    /// 修改某项属性值（加上delta，负数为减少）
    /// </summary>
    public void ModifyAttribute(AttributeTypeEnum type, float delta)
    {
        if (this.attributes.ContainsKey(type))
        {
            this.attributes[type] += delta;
        }
    }

    /// <summary>
    /// 获取某项属性值，若不存在返回0
    /// </summary>
    public float GetAttribute(AttributeTypeEnum type)
    {
        if (this.attributes.ContainsKey(type))
        {
            return this.attributes[type];
        }
        return 0f;
    }

    /// <summary>
    /// 根据饱腹值占上限的比值返回对应的状态文字
    /// </summary>
    public string GetFullnessStatus()
    {
        float ratio = this.fullness / this.max_fullness;

        if (ratio <= 0.1f)
        {
            return "快饿死了";
        }
        else if (ratio <= 0.25f)
        {
            return "饥饿";
        }
        else if (ratio <= 0.45f)
        {
            return "有点饿了";
        }
        else if (ratio <= 0.65f)
        {
            return "良好";
        }
        else if (ratio <= 0.8f)
        {
            return "有点饱了";
        }
        else if (ratio <= 0.95f)
        {
            return "饱了";
        }
        else
        {
            return "撑了";
        }
    }
}
