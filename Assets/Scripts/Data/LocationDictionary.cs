using System.Collections.Generic;

/// <summary>
/// 地点数据字典——存放所有地点
/// </summary>
public class LocationDictionary : Singleton<LocationDictionary>
{
    // 所有地点（key=地点ID）
    private Dictionary<int, LocationData> all_locations;

    private LocationDictionary()
    {
        this.all_locations = new Dictionary<int, LocationData>()
        {
            {
                1,
                new LocationData(
                    id: 1,
                    name: "锦安城",
                    type: LocationTypeEnum.City,
                    connections: new Dictionary<int, int>()
                    {
                        { 2, 24 }
                    },
                    top_scene_id: 1
                )
            },
            {
                2,
                new LocationData(
                    id: 2,
                    name: "洛川城",
                    type: LocationTypeEnum.City,
                    connections: new Dictionary<int, int>()
                    {
                        { 1, 24 }
                    },
                    top_scene_id: 5
                )
            }
        };
    }

    /// <summary>
    /// 添加一个地点到字典中
    /// </summary>
    public void Add(LocationData location)
    {
        this.all_locations[location.id] = location;
    }

    /// <summary>
    /// 根据ID获取地点，找不到返回null
    /// </summary>
    public LocationData Get(int id)
    {
        if (this.all_locations.ContainsKey(id))
        {
            return this.all_locations[id];
        }
        return null;
    }

    /// <summary>
    /// 移除指定ID的地点
    /// </summary>
    public void Remove(int id)
    {
        if (this.all_locations.ContainsKey(id))
        {
            this.all_locations.Remove(id);
        }
    }

    /// <summary>
    /// 获取所有地点（返回副本，外部修改不影响内部）
    /// </summary>
    public List<LocationData> GetAll()
    {
        return new List<LocationData>(this.all_locations.Values);
    }
}
