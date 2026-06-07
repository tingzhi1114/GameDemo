using System.Collections.Generic;

/// <summary>
/// 场景数据字典——存放所有场景
/// </summary>
public class SceneDictionary : Singleton<SceneDictionary>
{
    // 所有场景（key=场景ID）
    private Dictionary<int, SceneData> all_scenes;

    private SceneDictionary()
    {
        this.all_scenes = new Dictionary<int, SceneData>()
        {
            // 锦安城
            {
                1,
                new SceneData(
                    id: 1,
                    location_id: 1,
                    name: "锦安城",
                    children_ids: new List<int>() { 2, 3, 4 },
                    template_id: 4
                )
            },
            {
                2,
                new SceneData(
                    id: 2,
                    location_id: 1,
                    name: "市集",
                    parent_scene_id: 1,
                    template_id: 1
                )
            },
            {
                3,
                new SceneData(
                    id: 3,
                    location_id: 1,
                    name: "客栈",
                    parent_scene_id: 1,
                    template_id: 2
                )
            },
            {
                4,
                new SceneData(
                    id: 4,
                    location_id: 1,
                    name: "寺庙",
                    parent_scene_id: 1,
                    template_id: 3
                )
            },

            // 洛川城
            {
                5,
                new SceneData(
                    id: 5,
                    location_id: 2,
                    name: "洛川城",
                    children_ids: new List<int>() { 6, 7, 8 },
                    template_id: 4
                )
            },
            {
                6,
                new SceneData(
                    id: 6,
                    location_id: 2,
                    name: "市集",
                    parent_scene_id: 5,
                    template_id: 1
                )
            },
            {
                7,
                new SceneData(
                    id: 7,
                    location_id: 2,
                    name: "客栈",
                    parent_scene_id: 5,
                    template_id: 2
                )
            },
            {
                8,
                new SceneData(
                    id: 8,
                    location_id: 2,
                    name: "寺庙",
                    parent_scene_id: 5,
                    template_id: 3
                )
            }
        };
    }

    /// <summary>
    /// 添加一个场景到字典中
    /// </summary>
    public void Add(SceneData scene)
    {
        this.all_scenes[scene.id] = scene;
    }

    /// <summary>
    /// 根据ID获取场景，找不到返回null
    /// </summary>
    public SceneData Get(int id)
    {
        if (this.all_scenes.ContainsKey(id))
        {
            return this.all_scenes[id];
        }
        return null;
    }

    /// <summary>
    /// 移除指定ID的场景
    /// </summary>
    public void Remove(int id)
    {
        if (this.all_scenes.ContainsKey(id))
        {
            this.all_scenes.Remove(id);
        }
    }

    /// <summary>
    /// 获取所有场景（返回副本）
    /// </summary>
    public List<SceneData> GetAll()
    {
        return new List<SceneData>(this.all_scenes.Values);
    }

    /// <summary>
    /// 获取某个地点下的所有场景
    /// </summary>
    public List<SceneData> GetByLocation(int location_id)
    {
        List<SceneData> result = new List<SceneData>();
        foreach (SceneData scene in this.all_scenes.Values)
        {
            if (scene.location_id == location_id)
            {
                result.Add(scene);
            }
        }
        return result;
    }
}
