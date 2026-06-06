using System.Collections.Generic;

/// <summary>
/// 地点数据字典——存放所有地点
/// </summary>
public class LocationDictionary
{
    // 单例
    public static LocationDictionary Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LocationDictionary();
            }
            return instance;
        }
    }
    private static LocationDictionary instance;

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
                    name: "汴京",
                    type: LocationTypeEnum.City,
                    connections: new Dictionary<int, float>()
                    {
                        { 2, 3f },
                        { 3, 5f }
                    }
                )
            },

            {
                2,
                new LocationData(
                    id: 2,
                    name: "应天府",
                    type: LocationTypeEnum.City,
                    connections: new Dictionary<int, float>()
                    {
                        { 1, 3f },
                        { 4, 2f },
                        { 5, 4f }
                    }
                )
            },

            {
                3,
                new LocationData(
                    id: 3,
                    name: "洛阳",
                    type: LocationTypeEnum.City,
                    connections: new Dictionary<int, float>()
                    {
                        { 1, 5f },
                        { 6, 3f }
                    }
                )
            },

            {
                4,
                new LocationData(
                    id: 4,
                    name: "柳溪村",
                    type: LocationTypeEnum.Village,
                    connections: new Dictionary<int, float>()
                    {
                        { 2, 2f }
                    }
                )
            },

            {
                5,
                new LocationData(
                    id: 5,
                    name: "黑风林",
                    type: LocationTypeEnum.Forest,
                    connections: new Dictionary<int, float>()
                    {
                        { 2, 4f }
                    }
                )
            },

            {
                6,
                new LocationData(
                    id: 6,
                    name: "苍龙山",
                    type: LocationTypeEnum.Mountain,
                    connections: new Dictionary<int, float>()
                    {
                        { 3, 3f }
                    }
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
