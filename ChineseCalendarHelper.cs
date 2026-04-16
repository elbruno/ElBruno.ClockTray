using System.Globalization;

namespace ClockTray;

/// <summary>
/// Provides Chinese lunar calendar formatting using the sexagenary cycle (干支),
/// zodiac animals (生肖), and traditional numeral day/month names.
/// </summary>
public static class ChineseCalendarHelper
{
    private static readonly ChineseLunisolarCalendar LunarCalendar = new();

    private static readonly string[] HeavenlyStems =
        ["甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸"];

    private static readonly string[] EarthlyBranches =
        ["子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥"];

    private static readonly string[] ZodiacAnimals =
        ["鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪"];

    private static readonly string[] MonthNames =
        ["正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "冬", "腊"];

    private static readonly string[] DayNames =
    [
        "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十",
        "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十",
        "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十"
    ];

    private static readonly string[] DayOfWeekChars = ["日", "一", "二", "三", "四", "五", "六"];

    /// <summary>
    /// Returns true if the date is within the supported range of ChineseLunisolarCalendar.
    /// </summary>
    public static bool IsInRange(DateTime date)
    {
        return date >= LunarCalendar.MinSupportedDateTime
            && date <= LunarCalendar.MaxSupportedDateTime;
    }

    /// <summary>
    /// Returns the sexagenary year name (干支年), e.g. "甲子".
    /// </summary>
    public static string GetSexagenaryYear(DateTime date)
    {
        if (!IsInRange(date)) return string.Empty;

        int sexagenaryYear = LunarCalendar.GetSexagenaryYear(date);
        int stemIndex = LunarCalendar.GetCelestialStem(sexagenaryYear) - 1;
        int branchIndex = LunarCalendar.GetTerrestrialBranch(sexagenaryYear) - 1;

        return $"{HeavenlyStems[stemIndex]}{EarthlyBranches[branchIndex]}年";
    }

    /// <summary>
    /// Returns the Chinese zodiac animal for the given date, e.g. "马".
    /// </summary>
    public static string GetZodiac(DateTime date)
    {
        if (!IsInRange(date)) return string.Empty;

        int sexagenaryYear = LunarCalendar.GetSexagenaryYear(date);
        int branchIndex = LunarCalendar.GetTerrestrialBranch(sexagenaryYear) - 1;

        return ZodiacAnimals[branchIndex];
    }

    /// <summary>
    /// Returns the lunar month name, with leap month prefix (闰) if applicable.
    /// E.g. "二月" or "闰四月".
    /// </summary>
    public static string GetLunarMonth(DateTime date)
    {
        if (!IsInRange(date)) return string.Empty;

        int year = LunarCalendar.GetYear(date);
        int month = LunarCalendar.GetMonth(date);
        int leapMonth = LunarCalendar.GetLeapMonth(year);

        bool isLeap = leapMonth > 0 && month == leapMonth;
        int displayMonth = month;

        if (leapMonth > 0 && month >= leapMonth)
            displayMonth = month - 1;

        string prefix = isLeap ? "闰" : "";
        return $"{prefix}{MonthNames[displayMonth - 1]}月";
    }

    /// <summary>
    /// Returns the lunar day in traditional numerals, e.g. "廿九".
    /// </summary>
    public static string GetLunarDay(DateTime date)
    {
        if (!IsInRange(date)) return string.Empty;

        int day = LunarCalendar.GetDayOfMonth(date);
        return DayNames[day - 1];
    }

    /// <summary>
    /// Returns the Chinese character for the day of the week, e.g. "四" for Thursday.
    /// </summary>
    public static string GetDayOfWeek(DateTime date)
    {
        return DayOfWeekChars[(int)date.DayOfWeek];
    }

    /// <summary>
    /// Formats a complete Chinese calendar line including zodiac:
    /// "干支年 生肖 月 日 周", e.g. "丙午年 马 二月 廿九 四"
    /// Returns empty string if date is outside supported range.
    /// </summary>
    public static string FormatCalendarLine(DateTime date)
    {
        if (!IsInRange(date)) return string.Empty;

        string sexagenary = GetSexagenaryYear(date);
        string zodiac = GetZodiac(date);
        string month = GetLunarMonth(date);
        string day = GetLunarDay(date);
        string dow = GetDayOfWeek(date);

        return $"{sexagenary} {zodiac} {month} {day} {dow}";
    }
}
