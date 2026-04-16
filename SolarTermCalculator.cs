namespace ClockTray;

/// <summary>
/// Calculates the 24 solar terms (二十四节气) using a simplified astronomical approximation.
/// Each solar term corresponds to the sun reaching a specific ecliptic longitude (multiples of 15°).
/// </summary>
public static class SolarTermCalculator
{
    /// <summary>
    /// Information about a single solar term.
    /// </summary>
    public readonly record struct SolarTermInfo(string ChineseName, string EnglishName, DateTime Date);

    private static readonly (string Chinese, string English, int Month, int BaseDay, int Longitude)[] SolarTermData =
    [
        ("小寒", "Minor Cold",        1,  6, 285),
        ("大寒", "Major Cold",        1, 20, 300),
        ("立春", "Start of Spring",   2,  4, 315),
        ("雨水", "Rain Water",        2, 19, 330),
        ("惊蛰", "Awakening",         3,  6, 345),
        ("春分", "Spring Equinox",    3, 21,   0),
        ("清明", "Clear and Bright",  4,  5,  15),
        ("谷雨", "Grain Rain",        4, 20,  30),
        ("立夏", "Start of Summer",   5,  6,  45),
        ("小满", "Grain Buds",        5, 21,  60),
        ("芒种", "Grain in Ear",      6,  6,  75),
        ("夏至", "Summer Solstice",   6, 21,  90),
        ("小暑", "Minor Heat",        7,  7, 105),
        ("大暑", "Major Heat",        7, 23, 120),
        ("立秋", "Start of Autumn",   8,  7, 135),
        ("处暑", "End of Heat",       8, 23, 150),
        ("白露", "White Dew",         9,  8, 165),
        ("秋分", "Autumn Equinox",    9, 23, 180),
        ("寒露", "Cold Dew",         10,  8, 195),
        ("霜降", "Frost's Descent",  10, 23, 210),
        ("立冬", "Start of Winter",  11,  7, 225),
        ("小雪", "Minor Snow",       11, 22, 240),
        ("大雪", "Major Snow",       12,  7, 255),
        ("冬至", "Winter Solstice",  12, 22, 270),
    ];

    /// <summary>
    /// Returns the solar term name if the given date falls on a solar term, or null otherwise.
    /// </summary>
    public static string? GetSolarTerm(DateTime date)
    {
        var terms = GetSolarTermsForYear(date.Year);
        foreach (var term in terms)
        {
            if (term.Date.Date == date.Date)
                return term.ChineseName;
        }
        return null;
    }

    /// <summary>
    /// Returns the current solar term (if today is one) or the next upcoming solar term with its date.
    /// </summary>
    public static SolarTermInfo GetCurrentOrNextSolarTerm(DateTime date)
    {
        var today = date.Date;

        // Check current year and next year to handle December edge cases
        var terms = GetSolarTermsForYear(today.Year);
        foreach (var term in terms)
        {
            if (term.Date.Date >= today)
                return term;
        }

        // All terms this year have passed; return first term of next year
        var nextYearTerms = GetSolarTermsForYear(today.Year + 1);
        return nextYearTerms[0];
    }

    /// <summary>
    /// Returns all 24 solar terms with their approximate dates for the given year.
    /// Uses a simplified astronomical calculation based on the sun's ecliptic longitude.
    /// </summary>
    public static SolarTermInfo[] GetSolarTermsForYear(int year)
    {
        var results = new SolarTermInfo[24];

        for (int i = 0; i < 24; i++)
        {
            var (chinese, english, month, baseDay, longitude) = SolarTermData[i];
            int adjustedDay = CalculateSolarTermDay(year, month, baseDay, longitude);

            // Clamp to valid day range for the month
            int daysInMonth = DateTime.DaysInMonth(year, month);
            adjustedDay = Math.Clamp(adjustedDay, 1, daysInMonth);

            results[i] = new SolarTermInfo(chinese, english, new DateTime(year, month, adjustedDay));
        }

        return results;
    }

    /// <summary>
    /// Calculates the approximate day of a solar term using a simplified ephemeris approach.
    /// Based on the year's offset from 2000 and the ~6-hour annual shift with leap year corrections.
    /// </summary>
    private static int CalculateSolarTermDay(int year, int month, int baseDay, int longitude)
    {
        // Julian century from J2000.0
        double y = year + (month - 1.0) / 12.0;

        // The solar terms shift by approximately 0.2422 days per year
        // with a leap-year correction that resets the cycle every 4 years.
        const double shift = 0.2422;

        // Base year for calibration
        const int baseYear = 2000;
        int dy = year - baseYear;

        // Century correction: Gregorian calendar adjustment
        double centuryCorrection = 0.0;
        if (year >= 2000)
        {
            // For 21st century, use the base days directly with shift
            centuryCorrection = 0.0;
        }
        else if (year >= 1900)
        {
            // 20th century has a +1 day offset for most terms
            centuryCorrection = 1.0;
        }

        // Leap year count since base year (number of Feb 29s that have occurred)
        int leapCount = CountLeapYears(baseYear, year);

        // Approximate calculation:
        // day ≈ baseDay + (dy × shift) - leapCount + centuryCorrection
        // The shift accumulates ~6 hours per year, and each leap year corrects by ~1 day
        double rawDay = baseDay + centuryCorrection + (dy * shift) - leapCount;

        return (int)Math.Round(rawDay);
    }

    /// <summary>
    /// Counts the number of leap years in the range [fromYear, toYear) exclusive of toYear.
    /// </summary>
    private static int CountLeapYears(int fromYear, int toYear)
    {
        if (toYear <= fromYear) return 0;

        // Count leap years from fromYear to toYear-1
        return CountLeapYearsUpTo(toYear - 1) - CountLeapYearsUpTo(fromYear - 1);
    }

    private static int CountLeapYearsUpTo(int year)
    {
        if (year < 0) return 0;
        return year / 4 - year / 100 + year / 400;
    }
}
