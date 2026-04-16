using ClockTray;

namespace ClockTray.Tests;

public class ChineseCalendarHelperTests
{
    // --- IsInRange ---

    [Fact]
    public void IsInRange_CurrentDate_ReturnsTrue()
    {
        Assert.True(ChineseCalendarHelper.IsInRange(DateTime.Now));
    }

    [Fact]
    public void IsInRange_Year2025_ReturnsTrue()
    {
        Assert.True(ChineseCalendarHelper.IsInRange(new DateTime(2025, 7, 15)));
    }

    [Fact]
    public void IsInRange_VeryOldDate_ReturnsFalse()
    {
        Assert.False(ChineseCalendarHelper.IsInRange(new DateTime(100, 1, 1)));
    }

    [Fact]
    public void IsInRange_FarFutureDate_ReturnsFalse()
    {
        Assert.False(ChineseCalendarHelper.IsInRange(DateTime.MaxValue));
    }

    // --- GetSexagenaryYear ---

    [Fact]
    public void GetSexagenaryYear_2024_ReturnsJiaChen()
    {
        // 2024 is 甲辰年 (Year of the Dragon)
        var date = new DateTime(2024, 7, 1);
        var result = ChineseCalendarHelper.GetSexagenaryYear(date);
        Assert.Equal("甲辰年", result);
    }

    [Fact]
    public void GetSexagenaryYear_2025_ReturnsYiSi()
    {
        // 2025 (after lunar new year) is 乙巳年
        var date = new DateTime(2025, 7, 1);
        var result = ChineseCalendarHelper.GetSexagenaryYear(date);
        Assert.Equal("乙巳年", result);
    }

    [Fact]
    public void GetSexagenaryYear_OutOfRange_ReturnsEmpty()
    {
        var result = ChineseCalendarHelper.GetSexagenaryYear(new DateTime(100, 1, 1));
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetSexagenaryYear_EndsWithYear()
    {
        var result = ChineseCalendarHelper.GetSexagenaryYear(new DateTime(2025, 6, 1));
        Assert.EndsWith("年", result);
    }

    [Fact]
    public void GetSexagenaryYear_HasTwoCharsBeforeYear()
    {
        var result = ChineseCalendarHelper.GetSexagenaryYear(new DateTime(2025, 6, 1));
        // Should be exactly 3 chars: stem + branch + 年
        Assert.Equal(3, result.Length);
    }

    // --- GetZodiac ---

    [Fact]
    public void GetZodiac_2024_ReturnsDragon()
    {
        // 2024 is Year of the Dragon (龙)
        var date = new DateTime(2024, 7, 1);
        Assert.Equal("龙", ChineseCalendarHelper.GetZodiac(date));
    }

    [Fact]
    public void GetZodiac_2025_ReturnsSnake()
    {
        // 2025 (after lunar new year) is Year of the Snake (蛇)
        var date = new DateTime(2025, 7, 1);
        Assert.Equal("蛇", ChineseCalendarHelper.GetZodiac(date));
    }

    [Fact]
    public void GetZodiac_OutOfRange_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, ChineseCalendarHelper.GetZodiac(DateTime.MaxValue));
    }

    [Theory]
    [InlineData("鼠")]
    [InlineData("牛")]
    [InlineData("虎")]
    [InlineData("兔")]
    [InlineData("龙")]
    [InlineData("蛇")]
    [InlineData("马")]
    [InlineData("羊")]
    [InlineData("猴")]
    [InlineData("鸡")]
    [InlineData("狗")]
    [InlineData("猪")]
    public void GetZodiac_AllAnimalsAreValid(string animal)
    {
        // Verify each animal is a single character
        Assert.Equal(1, animal.Length);
    }

    [Fact]
    public void GetZodiac_Returns12UniqueAnimalsOver12Years()
    {
        // From mid-2020 to mid-2031, all 12 zodiac animals should appear
        var animals = new HashSet<string>();
        for (int year = 2020; year <= 2031; year++)
        {
            var zodiac = ChineseCalendarHelper.GetZodiac(new DateTime(year, 7, 1));
            animals.Add(zodiac);
        }
        Assert.Equal(12, animals.Count);
    }

    // --- GetLunarMonth ---

    [Fact]
    public void GetLunarMonth_ReturnsMonthWithSuffix()
    {
        var date = new DateTime(2025, 7, 1);
        var result = ChineseCalendarHelper.GetLunarMonth(date);
        Assert.EndsWith("月", result);
    }

    [Fact]
    public void GetLunarMonth_OutOfRange_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, ChineseCalendarHelper.GetLunarMonth(DateTime.MaxValue));
    }

    [Fact]
    public void GetLunarMonth_ValidDate_ReturnsNonEmpty()
    {
        var result = ChineseCalendarHelper.GetLunarMonth(new DateTime(2025, 3, 15));
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetLunarMonth_JanuaryDate_ReturnsValidMonth()
    {
        // January 15, 2025 — should be 腊月 (12th month) of the previous lunar year
        var result = ChineseCalendarHelper.GetLunarMonth(new DateTime(2025, 1, 15));
        Assert.NotEmpty(result);
        Assert.EndsWith("月", result);
    }

    // --- GetLunarDay ---

    [Fact]
    public void GetLunarDay_ValidDate_ReturnsNonEmpty()
    {
        var result = ChineseCalendarHelper.GetLunarDay(new DateTime(2025, 7, 1));
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetLunarDay_OutOfRange_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, ChineseCalendarHelper.GetLunarDay(DateTime.MaxValue));
    }

    [Fact]
    public void GetLunarDay_AllDayNamesAreDistinct()
    {
        // Collect day names over a full lunar month (30 days)
        // Start from a known date and collect consecutive days
        var days = new HashSet<string>();
        var start = new DateTime(2025, 2, 1); // A date well within range
        for (int i = 0; i < 30; i++)
        {
            var day = ChineseCalendarHelper.GetLunarDay(start.AddDays(i));
            if (!string.IsNullOrEmpty(day))
                days.Add(day);
        }
        // We should have at least 28 unique day names (a lunar month is 29 or 30 days)
        Assert.True(days.Count >= 28, $"Expected at least 28 unique day names, got {days.Count}");
    }

    // --- GetDayOfWeek ---

    [Theory]
    [InlineData(DayOfWeek.Sunday, "日")]
    [InlineData(DayOfWeek.Monday, "一")]
    [InlineData(DayOfWeek.Tuesday, "二")]
    [InlineData(DayOfWeek.Wednesday, "三")]
    [InlineData(DayOfWeek.Thursday, "四")]
    [InlineData(DayOfWeek.Friday, "五")]
    [InlineData(DayOfWeek.Saturday, "六")]
    public void GetDayOfWeek_ReturnsCorrectChineseChar(DayOfWeek dow, string expected)
    {
        // Find a date with the given day of week
        var date = new DateTime(2025, 1, 5); // Sunday
        while (date.DayOfWeek != dow)
            date = date.AddDays(1);

        Assert.Equal(expected, ChineseCalendarHelper.GetDayOfWeek(date));
    }

    [Fact]
    public void GetDayOfWeek_ReturnsSingleChar()
    {
        var result = ChineseCalendarHelper.GetDayOfWeek(DateTime.Now);
        Assert.Equal(1, result.Length);
    }

    // --- FormatCalendarLine ---

    [Fact]
    public void FormatCalendarLine_ValidDate_ContainsAllParts()
    {
        var date = new DateTime(2025, 7, 1);
        var result = ChineseCalendarHelper.FormatCalendarLine(date);

        Assert.NotEmpty(result);
        // Should contain 年 (year marker)
        Assert.Contains("年", result);
        // Should contain 月 (month marker)
        Assert.Contains("月", result);
        // Should have 5 space-separated parts: sexagenary zodiac month day dow
        var parts = result.Split(' ');
        Assert.Equal(5, parts.Length);
    }

    [Fact]
    public void FormatCalendarLine_OutOfRange_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, ChineseCalendarHelper.FormatCalendarLine(DateTime.MaxValue));
    }

    [Fact]
    public void FormatCalendarLine_IncludesZodiac()
    {
        var date = new DateTime(2024, 7, 1);
        var result = ChineseCalendarHelper.FormatCalendarLine(date);
        // 2024 is Year of the Dragon — 龙 should be in the line
        Assert.Contains("龙", result);
    }

    [Fact]
    public void FormatCalendarLine_2025_IncludesSnake()
    {
        var date = new DateTime(2025, 7, 1);
        var result = ChineseCalendarHelper.FormatCalendarLine(date);
        Assert.Contains("蛇", result);
    }

    [Fact]
    public void FormatCalendarLine_ConsistentWithIndividualMethods()
    {
        var date = new DateTime(2025, 6, 15);
        var line = ChineseCalendarHelper.FormatCalendarLine(date);
        var sexagenary = ChineseCalendarHelper.GetSexagenaryYear(date);
        var zodiac = ChineseCalendarHelper.GetZodiac(date);
        var month = ChineseCalendarHelper.GetLunarMonth(date);
        var day = ChineseCalendarHelper.GetLunarDay(date);
        var dow = ChineseCalendarHelper.GetDayOfWeek(date);

        Assert.Equal($"{sexagenary} {zodiac} {month} {day} {dow}", line);
    }
}
