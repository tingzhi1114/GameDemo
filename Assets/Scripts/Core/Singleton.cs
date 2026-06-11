using System;

/// <summary>
/// 单例基类——所有非MonoBehaviour的单例继承此类
/// 子类构造器必须是 private
/// </summary>
public abstract class Singleton<T> where T : class
{
    private static T instance;

    /// <summary>
    /// 单例实例，首次访问时自动创建
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Activator.CreateInstance(typeof(T), nonPublic: true) as T;
            }
            return instance;
        }
    }

    /// <summary>
    /// 主动触发单例创建，用于在初始化阶段控制创建顺序
    /// </summary>
    public static void EnsureCreated()
    {
        if (instance == null)
        {
            instance = Activator.CreateInstance(typeof(T), nonPublic: true) as T;
        }
    }
}
