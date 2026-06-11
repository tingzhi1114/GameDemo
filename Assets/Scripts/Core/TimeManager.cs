/// <summary>
/// 时间管理器——负责游戏内年/月/日/时段的推进
/// 每天6个时段：深夜/凌晨/上午/中午/下午/晚上
/// 每月固定30天，每年12个月
/// </summary>
public class TimeManager : Singleton<TimeManager>
{
    // 当前年份（从第1年开始）
    public int year;
    // 当前月份（1-12）
    public int month;
    // 当前日期（1-30）
    public int day;
    // 当前时段
    public TimePeriodEnum current_period;

    /// <summary>
    /// 每推进一个时段后触发，供 EventManager 和 WorldSimulator 订阅
    /// </summary>
    public event System.Action OnTimeAdvanced;

    private TimeManager()
    {
        year = 1;
        month = 1;
        day = 1;
        current_period = TimePeriodEnum.DeadOfNight;
    }

    /// <summary>
    /// 获取格式化的时间字符串，如 "1年1月1日 上午"
    /// </summary>
    public string GetDateString()
    {
        string[] period_names = { "深夜", "凌晨", "上午", "中午", "下午", "晚上" };
        string period_str = period_names[(int)current_period];
        return $"{year}年{month}月{day}日 {period_str}";
    }

    /// <summary>
    /// 推进 N 个时段，默认推进1个时段
    /// </summary>
    public void AdvanceTime(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            AdvanceOneSlot();
        }
        OnTimeAdvanced?.Invoke();
    }

    // 推进一个时段的内部逻辑
    private void AdvanceOneSlot()
    {
        // 按时段顺序推进
        if (current_period == TimePeriodEnum.DeadOfNight)
        {
            current_period = TimePeriodEnum.EarlyMorning;
        }
        else if (current_period == TimePeriodEnum.EarlyMorning)
        {
            current_period = TimePeriodEnum.Morning;
        }
        else if (current_period == TimePeriodEnum.Morning)
        {
            current_period = TimePeriodEnum.Noon;
        }
        else if (current_period == TimePeriodEnum.Noon)
        {
            current_period = TimePeriodEnum.Afternoon;
        }
        else if (current_period == TimePeriodEnum.Afternoon)
        {
            current_period = TimePeriodEnum.Evening;
        }
        else if (current_period == TimePeriodEnum.Evening)
        {
            // 晚上走完 → 进下一天
            current_period = TimePeriodEnum.DeadOfNight;
            day++;
            if (day > 30)
            {
                day = 1;
                month++;
                if (month > 12)
                {
                    month = 1;
                    year++;
                }
            }
        }
    }
}
