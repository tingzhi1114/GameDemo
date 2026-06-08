using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地点数据字典——从 Locations.json 加载所有地点
/// </summary>
public class LocationDictionary : Singleton<LocationDictionary>
{
    // 所有地点（key=地点ID）
    private Dictionary<int, LocationData> all_locations;
    // 下一个可用的自增ID
    private int next_id;

    private LocationDictionary()
    {
        this.all_locations = new Dictionary<int, LocationData>();

        // 从 Resources/Data/Locations.json 加载地点数据
        TextAsset json_text = Resources.Load<TextAsset>("Data/Locations");
        if (json_text == null)
        {
            Debug.LogError("Locations.json 未找到，请确保 Assets/Resources/Data/Locations.json 存在");
            return;
        }

        LocationDataListJSON database = JsonUtility.FromJson<LocationDataListJSON>(json_text.text);
        if (database == null || database.locations == null)
        {
            Debug.LogError("Locations.json 解析失败，请检查 JSON 格式");
            return;
        }

        // 遍历JSON条目，逐条解析为LocationData
        for (int i = 0; i < database.locations.Count; i++)
        {
            LocationDataJSON entry = database.locations[i];
            LocationData location = ParseEntry(entry);
            if (location != null)
            {
                this.all_locations[location.id] = location;
            }
        }

        // 以JSON中最大ID为基础，后续新增地点的ID递增
        int max_id = 0;
        foreach (int key in this.all_locations.Keys)
        {
            if (key > max_id)
            {
                max_id = key;
            }
        }
        this.next_id = max_id + 1;

        Debug.Log("LocationDictionary: 从 Locations.json 加载了 " + this.all_locations.Count + " 个地点");
    }

    /// <summary>
    /// 将一条 JSON 条目解析为 LocationData
    /// </summary>
    private LocationData ParseEntry(LocationDataJSON entry)
    {
        // 将字符串type解析为枚举，解析失败默认City
        LocationTypeEnum location_type = LocationTypeEnum.City;
        Enum.TryParse(entry.type, out location_type);

        // 将 connections 列表转为字典
        Dictionary<int, int> conn_dict = new Dictionary<int, int>();
        if (entry.connections != null && entry.connections.Count > 0)
        {
            for (int i = 0; i < entry.connections.Count; i++)
            {
                conn_dict[entry.connections[i].target_id] = entry.connections[i].distance;
            }
        }

        return new LocationData(
            id: entry.id,
            name: entry.name,
            type: location_type,
            connections: conn_dict,
            top_scene_id: entry.top_scene_id
        );
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
    /// 获取所有地点（返回副本，外部修改不影响内部）
    /// </summary>
    public List<LocationData> GetAll()
    {
        return new List<LocationData>(this.all_locations.Values);
    }

    /// <summary>
    /// 添加一个地点到字典中，自动分配自增ID
    /// </summary>
    public int Add(LocationData location)
    {
        int new_id = this.next_id;
        this.next_id++;
        location.id = new_id;
        this.all_locations[new_id] = location;
        return new_id;
    }

    /// <summary>
    /// 移除指定ID的地点
    /// </summary>
    public void Remove(int id)
    {
        this.all_locations.Remove(id);
    }
}
