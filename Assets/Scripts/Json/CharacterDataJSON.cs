using System;
using System.Collections.Generic;

/// <summary>
/// JSON反序列化辅助类——JSON中一条属性条目
/// </summary>
[Serializable]
public class AttributeJSON
{
    public string type;  // 字符串，解析时转为 AttributeTypeEnum
    public float value;
}

/// <summary>
/// JSON反序列化辅助类——JSON中一条角色条目
/// </summary>
[Serializable]
public class CharacterDataJSON
{
    public int id;
    public string name;
    public int gender;
    public int age;
    public int current_location_id;
    public int current_scene_id;
    public List<AttributeJSON> attributes;
}

/// <summary>
/// JSON反序列化辅助类——JSON根容器
/// </summary>
[Serializable]
public class CharacterDataListJSON
{
    public List<CharacterDataJSON> characters;
}
