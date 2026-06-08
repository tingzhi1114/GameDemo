using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色数据字典——从 Characters.json 加载所有角色
/// </summary>
public class CharacterDictionary : Singleton<CharacterDictionary>
{
    // 所有角色（key=角色ID）
    private Dictionary<int, CharacterData> all_characters;
    // 下一个可用的自增ID
    private int next_id;

    private CharacterDictionary()
    {
        this.all_characters = new Dictionary<int, CharacterData>();

        // 从 Resources/Data/Characters.json 加载角色数据
        TextAsset json_text = Resources.Load<TextAsset>("Data/Characters");
        if (json_text == null)
        {
            Debug.LogError("Characters.json 未找到，请确保 Assets/Resources/Data/Characters.json 存在");
            return;
        }

        CharacterDataListJSON database = JsonUtility.FromJson<CharacterDataListJSON>(json_text.text);
        if (database == null || database.characters == null)
        {
            Debug.LogError("Characters.json 解析失败，请检查 JSON 格式");
            return;
        }

        // 遍历JSON条目，逐条解析为CharacterData
        for (int i = 0; i < database.characters.Count; i++)
        {
            CharacterDataJSON entry = database.characters[i];
            CharacterData character = ParseEntry(entry);
            if (character != null)
            {
                this.all_characters[character.id] = character;
            }
        }

        // 以JSON中最大ID为基础，后续新增角色的ID递增
        int max_id = 0;
        foreach (int key in this.all_characters.Keys)
        {
            if (key > max_id)
            {
                max_id = key;
            }
        }
        this.next_id = max_id + 1;

        Debug.Log("CharacterDictionary: 从 Characters.json 加载了 " + this.all_characters.Count + " 个角色");
    }

    /// <summary>
    /// 将一条 JSON 条目解析为 CharacterData
    /// </summary>
    private CharacterData ParseEntry(CharacterDataJSON entry)
    {
        // 将 attributes 列表转为字典
        Dictionary<AttributeTypeEnum, float> attr_dict = new Dictionary<AttributeTypeEnum, float>();
        if (entry.attributes != null && entry.attributes.Count > 0)
        {
            for (int i = 0; i < entry.attributes.Count; i++)
            {
                AttributeTypeEnum attr_type = AttributeTypeEnum.Strength;
                Enum.TryParse(entry.attributes[i].type, out attr_type);
                attr_dict[attr_type] = entry.attributes[i].value;
            }
        }

        return new CharacterData(
            id: entry.id,
            name: entry.name,
            gender: entry.gender,
            age: entry.age,
            attributes: attr_dict,
            current_location_id: entry.current_location_id,
            current_scene_id: entry.current_scene_id
        );
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
    /// 添加一个角色到字典中，自动分配自增ID
    /// </summary>
    public int Add(CharacterData character)
    {
        int new_id = this.next_id;
        this.next_id++;
        character.id = new_id;
        this.all_characters[new_id] = character;
        return new_id;
    }

    /// <summary>
    /// 移除指定ID的角色（角色死亡时调用）
    /// </summary>
    public void Remove(int id)
    {
        this.all_characters.Remove(id);
    }
}
