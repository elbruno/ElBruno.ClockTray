using ClockTray;

namespace ClockTray.Tests;

public class SolarTermCalculatorTests
{
    // --- GetSolarTermsForYear ---

    [Fact]
    public void GetSolarTermsForYear_Returns24Terms()
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        Assert.Equal(24, terms.Length);
    }

    [Fact]
    public void GetSolarTermsForYear_AllTermsHaveNames()
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        foreach (var term in terms)
        {
            Assert.False(string.IsNullOrWhiteSpace(term.ChineseName), "Chinese name should not be empty");
            Assert.False(string.IsNullOrWhiteSpace(term.EnglishName), "English name should not be empty");
        }
    }

    [Fact]
    public void GetSolarTermsForYear_AllTermsInCorrectYear()
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        foreach (var term in terms)
        {
            Assert.Equal(2025, term.Date.Year);
        }
    }

    [Fact]
    public void GetSolarTermsForYear_TermsAreChronological()
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        for (int i = 1; i < terms.Length; i++)
        {
            Assert.True(terms[i].Date >= terms[i - 1].Date,
                $"Term {terms[i].ChineseName} ({terms[i].Date:yyyy-MM-dd}) should be >= " +
                $"{terms[i - 1].ChineseName} ({terms[i - 1].Date:yyyy-MM-dd})");
        }
    }

    [Fact]
    public void GetSolarTermsForYear_HasUniqueChineseNames()
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        var names = terms.Select(t => t.ChineseName).ToList();
        Assert.Equal(24, names.Distinct().Count());
    }

    [Fact]
    public void GetSolarTermsForYear_HasUniqueEnglishNames()
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        var names = terms.Select(t => t.EnglishName).ToList();
        Assert.Equal(24, names.Distinct().Count());
    }

    [Theory]
    [InlineData(2020)]
    [InlineData(2024)]
    [InlineData(2025)]
    [InlineData(2030)]
    [InlineData(2050)]
    public void GetSolarTermsForYear_WorksForVariousYears(int year)
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(year);
        Assert.Equal(24, terms.Length);
        Assert.All(terms, t => Assert.Equal(year, t.Date.Year));
    }

    // --- Known solar term dates (approximate, ±1 day tolerance) ---

    [Fact]
    public void GetSolarTermsForYear_2025_SpringEquinoxAroundMarch20()
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        var springEquinox = terms.First(t => t.ChineseName == "春分");
        Assert.Equal(3, springEquinox.Date.Month);
        Assert.InRange(springEquinox.Date.Day, 19, 22);
    }

    [Fact]
    public void GetSolarTermsForYear_2025_SummerSolsticeAroundJune21()
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        var summerSolstice = terms.First(t => t.ChineseName == "夏至");
        Assert.Equal(6, summerSolstice.Date.Month);
        Assert.InRange(summerSolstice.Date.Day, 20, 22);
    }

    [Fact]
    public void GetSolarTermsForYear_2025_AutumnEquinoxAroundSept23()
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        var autumnEquinox = terms.First(t => t.ChineseName == "秋分");
        Assert.Equal(9, autumnEquinox.Date.Month);
        Assert.InRange(autumnEquinox.Date.Day, 22, 24);
    }

    [Fact]
    public void GetSolarTermsForYear_2025_WinterSolsticeAroundDec22()
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        var winterSolstice = terms.First(t => t.ChineseName == "冬至");
        Assert.Equal(12, winterSolstice.Date.Month);
        Assert.InRange(winterSolstice.Date.Day, 21, 23);
    }

    [Fact]
    public void GetSolarTermsForYear_ContainsAllExpectedTermNames()
    {
        var expectedNames = new[]
        {
            "小寒", "大寒", "立春", "雨水", "惊蛰", "春分",
            "清明", "谷雨", "立夏", "小满", "芒种", "夏至",
            "小暑", "大暑", "立秋", "处暑", "白露", "秋分",
            "寒露", "霜降", "立冬", "小雪", "大雪", "冬至"
        };

        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        var actualNames = terms.Select(t => t.ChineseName).ToHashSet();

        foreach (var name in expectedNames)
        {
            Assert.Contains(name, actualNames);
        }
    }

    // --- GetSolarTerm ---

    [Fact]
    public void GetSolarTerm_OnSolarTermDate_ReturnsTermName()
    {
        // Find a known solar term date from the calculator itself
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        var firstTerm = terms[0]; // 小寒

        var result = SolarTermCalculator.GetSolarTerm(firstTerm.Date);
        Assert.NotNull(result);
        Assert.Equal(firstTerm.ChineseName, result);
    }

    [Fact]
    public void GetSolarTerm_OnNonSolarTermDate_ReturnsNull()
    {
        // Find two consecutive terms and pick a day in between
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        var term1 = terms[0];
        var term2 = terms[1];

        // Pick a date in between (at least 2 days after term1, if there's room)
        if ((term2.Date - term1.Date).Days > 2)
        {
            var midDate = term1.Date.AddDays(2);
            var result = SolarTermCalculator.GetSolarTerm(midDate);
            Assert.Null(result);
        }
    }

    // --- GetCurrentOrNextSolarTerm ---

    [Fact]
    public void GetCurrentOrNextSolarTerm_ReturnsValidTerm()
    {
        var result = SolarTermCalculator.GetCurrentOrNextSolarTerm(DateTime.Now);
        Assert.False(string.IsNullOrWhiteSpace(result.ChineseName));
        Assert.False(string.IsNullOrWhiteSpace(result.EnglishName));
    }

    [Fact]
    public void GetCurrentOrNextSolarTerm_ReturnsFutureOrTodayDate()
    {
        var today = DateTime.Today;
        var result = SolarTermCalculator.GetCurrentOrNextSolarTerm(today);
        Assert.True(result.Date >= today,
            $"Expected date >= {today:yyyy-MM-dd}, got {result.Date:yyyy-MM-dd}");
    }

    [Fact]
    public void GetCurrentOrNextSolarTerm_OnSolarTermDate_ReturnsThatTerm()
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(2025);
        var term = terms[5]; // 春分

        var result = SolarTermCalculator.GetCurrentOrNextSolarTerm(term.Date);
        Assert.Equal(term.ChineseName, result.ChineseName);
        Assert.Equal(term.Date, result.Date);
    }

    [Fact]
    public void GetCurrentOrNextSolarTerm_EndOfYear_ReturnsNextYearTerm()
    {
        // December 31 — all 2025 terms have passed, should get first 2026 term
        var result = SolarTermCalculator.GetCurrentOrNextSolarTerm(new DateTime(2025, 12, 31));
        // Should be 小寒 of 2026
        Assert.Equal(2026, result.Date.Year);
        Assert.Equal("小寒", result.ChineseName);
    }

    [Fact]
    public void GetCurrentOrNextSolarTerm_January1_ReturnsUpcomingTerm()
    {
        var result = SolarTermCalculator.GetCurrentOrNextSolarTerm(new DateTime(2025, 1, 1));
        // First solar term of 2025 is 小寒 around Jan 5-6
        Assert.Equal(2025, result.Date.Year);
        Assert.Equal(1, result.Date.Month);
    }

    // --- SolarTermInfo record ---

    [Fact]
    public void SolarTermInfo_RecordEquality()
    {
        var a = new SolarTermCalculator.SolarTermInfo("春分", "Spring Equinox", new DateTime(2025, 3, 20));
        var b = new SolarTermCalculator.SolarTermInfo("春分", "Spring Equinox", new DateTime(2025, 3, 20));
        Assert.Equal(a, b);
    }

    [Fact]
    public void SolarTermInfo_RecordInequality()
    {
        var a = new SolarTermCalculator.SolarTermInfo("春分", "Spring Equinox", new DateTime(2025, 3, 20));
        var b = new SolarTermCalculator.SolarTermInfo("秋分", "Autumn Equinox", new DateTime(2025, 9, 23));
        Assert.NotEqual(a, b);
    }

    // --- Leap year handling ---

    [Theory]
    [InlineData(2024)] // leap year
    [InlineData(2025)] // non-leap year
    [InlineData(2000)] // century leap year
    [InlineData(2100)] // century non-leap year
    public void GetSolarTermsForYear_HandlesLeapYears(int year)
    {
        var terms = SolarTermCalculator.GetSolarTermsForYear(year);
        Assert.Equal(24, terms.Length);

        // All dates should be valid
        foreach (var term in terms)
        {
            Assert.True(term.Date.Year == year);
            Assert.True(term.Date.Month >= 1 && term.Date.Month <= 12);
            Assert.True(term.Date.Day >= 1 && term.Date.Day <= DateTime.DaysInMonth(year, term.Date.Month));
        }
    }
}
