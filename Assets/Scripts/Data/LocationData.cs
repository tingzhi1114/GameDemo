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
    // 可通行的目的地列表（key=目标地点ID, value=路径长度/时段数）
    public Dictionary<int, int> connections;
    // 进入该地点后的顶级场景ID
    public int top_scene_id;

    public LocationData(int id, string name, LocationTypeEnum type, Dictionary<int, int> connections, int top_scene_id = -1)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.connections = connections;
        this.top_scene_id = top_scene_id;
    }

    /// <summary>
    /// 获取前往某地的路径长度，若不可达返回 -1
    /// </summary>
    public int GetConnectionLength(int target_id)
    {
        if (connections.ContainsKey(target_id))
        {
            return connections[target_id];
        }
        return -1;
    }
}
