using System.Collections.Generic;

/// <summary>
/// 角色数据字典——存放所有角色
/// </summary>
public class CharacterDictionary
{
    // 单例
    public static CharacterDictionary Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CharacterDictionary();
            }
            return instance;
        }
    }
    private static CharacterDictionary instance;

    // 所有角色（key=角色ID）
    private Dictionary<int, CharacterData> all_characters;

    private CharacterDictionary()
    {
        this.all_characters = new Dictionary<int, CharacterData>()
        {
            {
                1,
                new CharacterData(
                    id: 1,
                    name: "无名氏",
                    gender: 1,
                    age: 16,
                    attributes: new Dictionary<AttributeTypeEnum, float>()
                    {
                        { AttributeTypeEnum.Strength, 50f },
                        { AttributeTypeEnum.Agility, 50f },
                        { AttributeTypeEnum.Wit, 50f },
                        { AttributeTypeEnum.Charm, 50f },
                        { AttributeTypeEnum.Physique, 50f },
                        { AttributeTypeEnum.Luck, 50f }
                    },
                    current_location_id: 1
                )
            },

            {
                2,
                new CharacterData(
                    id: 2,
                    name: "赵掌柜",
                    gender: 1,
                    age: 35,
                    attributes: new Dictionary<AttributeTypeEnum, float>()
                    {
                        { AttributeTypeEnum.Strength, 40f },
                        { AttributeTypeEnum.Agility, 45f },
                        { AttributeTypeEnum.Wit, 60f },
                        { AttributeTypeEnum.Charm, 55f },
                        { AttributeTypeEnum.Physique, 45f },
                        { AttributeTypeEnum.Luck, 40f }
                    },
                    current_location_id: 1
                )
            },

            {
                3,
                new CharacterData(
                    id: 3,
                    name: "钱铁匠",
                    gender: 1,
                    age: 42,
                    attributes: new Dictionary<AttributeTypeEnum, float>()
                    {
                        { AttributeTypeEnum.Strength, 60f },
                        { AttributeTypeEnum.Agility, 40f },
                        { AttributeTypeEnum.Wit, 35f },
                        { AttributeTypeEnum.Charm, 40f },
                        { AttributeTypeEnum.Physique, 65f },
                        { AttributeTypeEnum.Luck, 45f }
                    },
                    current_location_id: 2
                )
            },

            {
                4,
                new CharacterData(
                    id: 4,
                    name: "孙老农",
                    gender: 1,
                    age: 55,
                    attributes: new Dictionary<AttributeTypeEnum, float>()
                    {
                        { AttributeTypeEnum.Strength, 45f },
                        { AttributeTypeEnum.Agility, 50f },
                        { AttributeTypeEnum.Wit, 40f },
                        { AttributeTypeEnum.Charm, 50f },
                        { AttributeTypeEnum.Physique, 50f },
                        { AttributeTypeEnum.Luck, 50f }
                    },
                    current_location_id: 4
                )
            }
        };
    }

    /// <summary>
    /// 添加一个角色到字典中
    /// </summary>
    public void Add(CharacterData character)
    {
        this.all_characters[character.id] = character;
    }

    /// <summary>
    /// 根据ID获取角色，找不到返回null
    /// </summary>
    public CharacterData Get(int id)
    {
        if (this.all_characters.ContainsKey(id))
        {
            return this.all_characters[id];
        }
        return null;
    }

    /// <summary>
    /// 移除指定ID的角色
    /// </summary>
    public void Remove(int id)
    {
        if (this.all_characters.ContainsKey(id))
        {
            this.all_characters.Remove(id);
        }
    }

    /// <summary>
    /// 获取所有角色（返回副本，外部修改不影响内部）
    /// </summary>
    public List<CharacterData> GetAll()
    {
        return new List<CharacterData>(this.all_characters.Values);
    }
}
