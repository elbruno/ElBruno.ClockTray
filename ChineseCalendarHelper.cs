using System.Globalization;

namespace ClockTray;

/// <summary>
/// Converts Gregorian dates to Chinese lunar calendar representations including
/// Heavenly Stems (天干), Earthly Branches (地支), sexagenary cycle (干支),
/// Chinese zodiac (十二生肖), lunar month/day names, and Chinese day-of-week.
/// </summary>
public static class ChineseCalendarHelper
{
    private static readonly ChineseLunisolarCalendar Cal = new();

    // 天干 – Heavenly Stems (indices 0–9, mapped from celestial stem 1–10)
    private static readonly string[] HeavenlyStems =
        { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };

    // 地支 – Earthly Branches (indices 0–11, mapped from terrestrial branch 1–12)
    private static readonly string[] EarthlyBranches =
        { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };

    // 十二生肖 – Chinese Zodiac animals (aligned with Earthly Branches)
    private static readonly string[] ZodiacAnimals =
        { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };

    // Lunar month names (index 0 = 正月)
    private static readonly string[] ChineseMonths =
        { "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "冬", "腊" };

    // Units digits for day names (index 0 reserved for "十" when day == 10/20/30)
    private static readonly string[] DayUnits =
        { "", "一", "二", "三", "四", "五", "六", "七", "八", "九" };

    // 星期 – Chinese day-of-week (Sunday=0 … Saturday=6)
    private static readonly string[] DayOfWeekNames =
        { "日", "一", "二", "三", "四", "五", "六" };

    /// <summary>Returns the sexagenary (干支) year label, e.g. "丙午".</summary>
    public static string GetStemBranch(DateTime date)
    {
        int sexYear = Cal.GetSexagenaryYear(date);
        int stemIndex   = Cal.GetCelestialStem(sexYear)   - 1; // 1-based → 0-based
        int branchIndex = Cal.GetTerrestrialBranch(sexYear) - 1;
        return HeavenlyStems[stemIndex] + EarthlyBranches[branchIndex];
    }

    /// <summary>Returns the Chinese zodiac animal for the lunar year, e.g. "马".</summary>
    public static string GetZodiacAnimal(DateTime date)
    {
        int sexYear = Cal.GetSexagenaryYear(date);
        int branchIndex = Cal.GetTerrestrialBranch(sexYear) - 1;
        return ZodiacAnimals[branchIndex];
    }

    /// <summary>Returns the Chinese lunar month name, e.g. "二月" or "闰二月".</summary>
    public static string GetLunarMonthName(DateTime date)
    {
        int lunarYear  = Cal.GetYear(date);
        int month      = Cal.GetMonth(date);
        int leapMonth  = Cal.GetLeapMonth(lunarYear); // 0 = no leap month

        // When a leap month exists, GetLeapMonth returns the index of the leap month.
        // Months before the leap month keep their number; the leap month itself and
        // all subsequent months must be shifted back by 1 to get the display number.
        bool isLeap    = leapMonth != 0 && month == leapMonth;
        int displayMonth = (leapMonth != 0 && month >= leapMonth) ? month - 1 : month;

        string prefix = isLeap ? "闰" : "";
        return prefix + ChineseMonths[displayMonth - 1] + "月";
    }

    /// <summary>Returns the Chinese lunar day name, e.g. "初一", "十五", "廿九".</summary>
    public static string GetLunarDayName(DateTime date)
    {
        int day = Cal.GetDayOfMonth(date);
        return FormatChineseDay(day);
    }

    /// <summary>Returns the Chinese day-of-week character, e.g. "四" for Thursday.</summary>
    public static string GetChineseDayOfWeek(DateTime date)
        => DayOfWeekNames[(int)date.DayOfWeek];

    /// <summary>
    /// Formats a single summary line suitable for the overlay, e.g. "丙午 二月 廿九 四".
    /// Returns an empty string if the date falls outside the supported calendar range.
    /// </summary>
    public static string FormatCalendarLine(DateTime date)
    {
        try
        {
            string stemBranch = GetStemBranch(date);
            string lunarMonth = GetLunarMonthName(date);
            string lunarDay   = GetLunarDayName(date);
            string dayOfWeek  = GetChineseDayOfWeek(date);
            return $"{stemBranch} {lunarMonth} {lunarDay} {dayOfWeek}";
        }
        catch
        {
            return string.Empty;
        }
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static string FormatChineseDay(int day) => day switch
    {
        10 => "初十",
        20 => "二十",
        30 => "三十",
        _ when day < 10 => "初" + DayUnits[day],
        _ when day < 20 => "十" + DayUnits[day - 10],
        _               => "廿" + DayUnits[day - 20]
    };
}
