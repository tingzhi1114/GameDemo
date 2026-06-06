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

    // 六维属性字典
    public Dictionary<AttributeTypeEnum, float> attributes;

    public CharacterData(int id, string name, int gender, int age, Dictionary<AttributeTypeEnum, float> attributes, int current_location_id)
    {
        this.id = id;
        this.name = name;
        this.gender = gender;
        this.age = age;
        this.attributes = attributes;
        this.current_location_id = current_location_id;
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
}
