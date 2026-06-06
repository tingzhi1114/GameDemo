using System.Collections.Generic;

/// <summary>
/// 地点数据模型
/// </summary>
public class LocationData
{
    // 地点ID
    public int id;
    // 地点名称
    public string name;
    // 地点类型
    public LocationTypeEnum type;
    // 可通行的目的地列表（key=目标地点ID, value=路径长度）
    public Dictionary<int, float> connections;

    public LocationData(int id, string name, LocationTypeEnum type, Dictionary<int, float> connections)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.connections = connections;
    }

    /// <summary>
    /// 添加一条可通行的路径
    /// </summary>
    public void AddConnection(int target_id, float length)
    {
        connections[target_id] = length;
    }

    /// <summary>
    /// 获取前往某地的路径长度，若不可达返回 -1
    /// </summary>
    public float GetConnectionLength(int target_id)
    {
        if (connections.ContainsKey(target_id))
        {
            return connections[target_id];
        }
        return -1f;
    }
}
