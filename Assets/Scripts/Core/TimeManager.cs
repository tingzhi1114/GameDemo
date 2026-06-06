using System;

/// <summary>
/// 时间管理器——负责游戏内年/月/旬的推进
/// </summary>
public class TimeManager
{
    // 单例
    public static TimeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TimeManager();
            }
            return instance;
        }
    }
    private static TimeManager instance;

    // 当前年份（从第1年开始）
    public int year;
    // 当前月份（1-12）
    public int month;
    // 当前处于哪一旬
    public PeriodEnum current_period;

    /// <summary>
    /// 每推进一旬时触发，供其他模块监听
    /// </summary>
    public event Action OnPeriodChanged;

    private TimeManager()
    {
        year = 1;
        month = 1;
        current_period = PeriodEnum.Early;
    }

    /// <summary>
    /// 获取格式化的时间字符串，如 "1年1月上旬"
    /// </summary>
    public string GetDateString()
    {
        string period_str = "";
        if (current_period == PeriodEnum.Early)
        {
            period_str = "上旬";
        }
        else if (current_period == PeriodEnum.Mid)
        {
            period_str = "中旬";
        }
        else if (current_period == PeriodEnum.Late)
        {
            period_str = "下旬";
        }
        return $"{year}年{month}月{period_str}";
    }

    /// <summary>
    /// 推进 N 旬，默认推进1旬
    /// </summary>
    public void AdvancePeriod(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            AdvanceOnePeriod();
        }
    }

    // 推进一旬的内部逻辑
    private void AdvanceOnePeriod()
    {
        switch (current_period)
        {
            case PeriodEnum.Early:
                current_period = PeriodEnum.Mid;
                break;
            case PeriodEnum.Mid:
                current_period = PeriodEnum.Late;
                break;
            case PeriodEnum.Late:
                // 下旬走完 → 进下个月
                current_period = PeriodEnum.Early;
                month++;
                if (month > 12)
                {
                    month = 1;
                    year++;
                }
                break;
        }
        OnPeriodChanged?.Invoke();
    }
}
