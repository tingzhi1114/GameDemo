using System;
using System.Collections.Generic;

/// <summary>
/// JSON反序列化辅助类——连接条目（目标地点ID, 路程时段数）
/// </summary>
[Serializable]
public class ConnectionJSON
{
    public int target_id;
    public int distance;
}

/// <summary>
/// JSON反序列化辅助类——生产力条目（某物品ID的生产力系数）
/// </summary>
[Serializable]
public class ProductivityJSON
{
    public int item_id;
    public float value;
}

/// <summary>
/// JSON反序列化辅助类——JSON中一条地点条目
/// </summary>
[Serializable]
public class LocationDataJSON
{
    public int id;
    public string name;
    public string type;                         // 字符串，解析时转为 LocationTypeEnum
    public int top_scene_id;
    public List<ConnectionJSON> connections;     // 可选，无则为null
    public List<ProductivityJSON> productivity; // 可选，生产力系数列表
}

/// <summary>
/// JSON反序列化辅助类——JSON根容器
/// </summary>
[Serializable]
public class LocationDataListJSON
{
    public List<LocationDataJSON> locations;
}
