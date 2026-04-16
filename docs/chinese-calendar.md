# Chinese Calendar Feature Documentation

## Overview

ClockTray now includes comprehensive Chinese lunar calendar support, providing Chinese-speaking users in Asia-Pacific and around the world with culturally relevant calendar information. This feature adds an optional floating overlay that displays traditional lunar calendar data including the sexagenary cycle, Chinese zodiac, lunar date, and 24 solar terms.

This feature was developed in response to [Issue #3](https://github.com/elbruno/ElBruno.ClockTray/issues/3) and addresses the needs of users who prefer or rely on the traditional Chinese calendar for cultural and practical purposes. The lunar calendar remains significant in Chinese culture, traditional medicine, agriculture, and festivals throughout East and Southeast Asia.

## Features

### Sexagenary Cycle (六十甲子 / 干支)

The sexagenary cycle is a system of dating based on two separate cycles that interact to create a repeating pattern. It combines:

- **10 Heavenly Stems (天干)**: Derived from ancient cosmological principles representing the five elements (木火土金水 — wood, fire, earth, metal, water) in their yin and yang aspects
- **12 Earthly Branches (地支)**: Associated with twelve animals and directions, forming the basis of the Chinese zodiac

Together, these create 60 unique combinations (10 × 12 = 60) that form the 60-year cycle.

#### The 10 Heavenly Stems (天干)

| Number | Chinese | Pinyin | Element | Aspect |
|--------|---------|--------|---------|--------|
| 1 | 甲 | jiǎ | Wood | Yang |
| 2 | 乙 | yǐ | Wood | Yin |
| 3 | 丙 | bǐng | Fire | Yang |
| 4 | 丁 | dīng | Fire | Yin |
| 5 | 戊 | wù | Earth | Yang |
| 6 | 己 | jǐ | Earth | Yin |
| 7 | 庚 | gēng | Metal | Yang |
| 8 | 辛 | xīn | Metal | Yin |
| 9 | 壬 | rén | Water | Yang |
| 10 | 癸 | guǐ | Water | Yin |

#### The 12 Earthly Branches (地支)

| Number | Chinese | Pinyin | Zodiac Animal | Direction |
|--------|---------|--------|----------------|-----------|
| 1 | 子 | zǐ | Rat (鼠) | North |
| 2 | 丑 | chǒu | Ox (牛) | North-Northeast |
| 3 | 寅 | yín | Tiger (虎) | East-Northeast |
| 4 | 卯 | mǎo | Rabbit (兔) | East |
| 5 | 辰 | chén | Dragon (龙) | East-Southeast |
| 6 | 巳 | sì | Snake (蛇) | South-Southeast |
| 7 | 午 | wǔ | Horse (马) | South |
| 8 | 未 | wèi | Goat (羊) | South-Southwest |
| 9 | 申 | shēn | Monkey (猴) | West-Southwest |
| 10 | 酉 | yǒu | Rooster (鸡) | West |
| 11 | 戌 | xū | Dog (狗) | West-Northwest |
| 12 | 亥 | hài | Pig (猪) | North-Northwest |

#### The 60-Year Cycle

The sexagenary cycle repeats every 60 years. For example:
- 1924: 甲子年 (Jiǎzǐ — Rat)
- 1984: 甲子年 (Jiǎzǐ — Rat) [60 years later]
- 2044: 甲子年 (Jiǎzǐ — Rat) [60 years later again]

The year designation (干支) is determined by the first day of the lunar year and remains the same throughout that lunar year.

### Chinese Zodiac (十二生肖)

The Chinese zodiac comprises 12 animals, each representing a year in the lunar calendar. The zodiac animal designation depends on which Earthly Branch governs that year:

| Zodiac Animal | Chinese | Pinyin | English | Characteristics |
|---------------|---------|--------|---------|-----------------|
| 鼠 | Rat | shǔ | Resourceful, cunning, quick-witted |
| 牛 | Ox | niú | Honest, reliable, methodical |
| 虎 | Tiger | hǔ | Courageous, confident, competitive |
| 兔 | Rabbit | tù | Gentle, graceful, cautious |
| 龙 | Dragon | lóng | Mythical, powerful, auspicious |
| 蛇 | Snake | shé | Wise, mysterious, sophisticated |
| 马 | Horse | mǎ | Energetic, independent, passionate |
| 羊 | Goat | yáng | Creative, gentle, peace-loving |
| 猴 | Monkey | hóu | Playful, intelligent, curious |
| 鸡 | Rooster | jī | Honest, straightforward, punctual |
| 狗 | Dog | gǒu | Loyal, faithful, trustworthy |
| 猪 | Pig | zhū | Compassionate, generous, honest |

The zodiac animal is directly tied to the Earthly Branch (地支) of the year:
- Year of the Rat (子) — born in years ending in 0 (Gregorian)
- Year of the Ox (丑) — born in years ending in 1
- Year of the Tiger (寅) — born in years ending in 2
- ...and so on

People born in the same zodiac year are believed to share similar personality traits and destinies according to traditional Chinese astrology.

### Lunar Month and Day Names

The lunar calendar organizes time into months and days with distinct naming conventions:

#### Lunar Months

Lunar months are numbered sequentially throughout the year:

| Name | Chinese | Meaning | Notes |
|------|---------|---------|-------|
| 1st Month | 正月 | zhèngyuè | "First month" — contains the Lunar New Year |
| 2nd Month | 二月 | èryuè | "Second month" |
| 3rd Month | 三月 | sānyuè | "Third month" |
| 4th Month | 四月 | sìyuè | "Fourth month" |
| 5th Month | 五月 | wǔyuè | "Fifth month" |
| 6th Month | 六月 | liùyuè | "Sixth month" |
| 7th Month | 七月 | qīyuè | "Seventh month" |
| 8th Month | 八月 | bāyuè | "Eighth month" |
| 9th Month | 九月 | jiǔyuè | "Ninth month" |
| 10th Month | 十月 | shíyuè | "Tenth month" |
| 11th Month | 十一月 | shíyīyuè | "Eleventh month" |
| 12th Month | 腊月 | làyuè | "Twelfth month" — also called 蜡月 |

**Leap Months (闰月)**: The lunar calendar adds a leap month approximately every three years to stay aligned with the solar year. When a leap month occurs, it is denoted as "闰" (leap) plus the month number (e.g., 闰五月 — leap fifth month).

#### Lunar Days

Days are named using a specific system based on traditional Chinese numerals:

| Day Range | Format | Example |
|-----------|--------|---------|
| 1–10 | 初一 through 初十 | 初一 (Day 1), 初三 (Day 3), 初十 (Day 10) |
| 11–20 | 十一 through 二十 | 十一 (Day 11), 十五 (Day 15), 二十 (Day 20) |
| 21–30 | 廿一 through 三十 | 廿一 (Day 21), 廿五 (Day 25), 三十 (Day 30) |

Note: 廿 (niàn) is a special character meaning "twenty."

#### Chinese Day of the Week (星期)

After the day name, the day of the week is displayed:

| Day | Chinese | Pinyin |
|-----|---------|--------|
| Monday | 星期一 | xīngqī yī |
| Tuesday | 星期二 | xīngqī èr |
| Wednesday | 星期三 | xīngqī sān |
| Thursday | 星期四 | xīngqī sì |
| Friday | 星期五 | xīngqī wǔ |
| Saturday | 星期六 | xīngqī liù |
| Sunday | 星期日 | xīngqī rì |

**Example**: 初三三 would represent the 3rd day of the lunar month falling on a Wednesday.

### 24 Solar Terms (二十四节气)

The 24 solar terms (节气) are critical points in the traditional East Asian lunisolar calendar, marking the sun's position along the ecliptic. They are deeply rooted in Chinese agriculture, philosophy, and cultural practices. UNESCO recognizes them as a Masterpiece of the Intangible Heritage of Humanity.

The solar terms are determined by the sun's ecliptic longitude (0° = spring equinox, 90° = summer solstice, 180° = autumn equinox, 270° = winter solstice), dividing the year into 24 segments of 15° each.

#### The 24 Solar Terms

| # | Chinese | Pinyin | English | Approx. Date | Description |
|---|---------|--------|---------|--------------|-------------|
| 1 | 立春 | lìchūn | Start of Spring | Feb 3–5 | First day of spring; weather begins warming |
| 2 | 雨水 | yǔshuǐ | Rain Water | Feb 18–20 | Increased rainfall; spring plowing begins |
| 3 | 惊蛰 | jīngzhé | Awakening of Insects | Mar 5–7 | Temperature rises; insects awaken from hibernation |
| 4 | 春分 | chūnfēn | Spring Equinox | Mar 20–22 | Day and night equal length; spring peak |
| 5 | 清明 | qīngmíng | Pure Brightness | Apr 4–6 | Clear weather; Qingming tomb-sweeping festival |
| 6 | 谷雨 | gǔyǔ | Grain Rain | Apr 19–21 | Grain begins to mature; final frost usually passes |
| 7 | 立夏 | lìxià | Start of Summer | May 5–7 | Transition to summer; temperature increases |
| 8 | 小满 | xiǎomǎn | Grain Buds | May 20–22 | Grain ripens; "small full" as crops fill out |
| 9 | 芒种 | mángzhòng | Grain in Ear | Jun 5–7 | Time to harvest and sow; grain develops awns |
| 10 | 夏至 | xiàzhì | Summer Solstice | Jun 20–22 | Longest day of the year; peak summer heat begins |
| 11 | 小暑 | xiǎoshǔ | Minor Heat | Jul 6–8 | Before the hottest period; moderate temperature rise |
| 12 | 大暑 | dàshǔ | Major Heat | Jul 22–24 | Hottest time of the year; intense heat peaks |
| 13 | 立秋 | lìqiū | Start of Autumn | Aug 7–9 | Transition to autumn; cooler temperatures begin |
| 14 | 处暑 | chǔshǔ | End of Heat | Aug 22–24 | Heat subsides; typically ends summer heat wave |
| 15 | 白露 | báilù | White Dew | Sep 7–9 | Morning dew becomes visible; temperature drops |
| 16 | 秋分 | qiūfēn | Autumn Equinox | Sep 22–24 | Day and night equal length; autumn peak |
| 17 | 寒露 | hánlù | Cold Dew | Oct 8–9 | Weather becomes cold; first frost may appear soon |
| 18 | 霜降 | shuāngjiàng | Descent of Frost | Oct 23–24 | First frost appears; temperatures drop significantly |
| 19 | 立冬 | lìdōng | Start of Winter | Nov 7–8 | Transition to winter; cold deepens |
| 20 | 小雪 | xiǎoxuě | Minor Snow | Nov 21–22 | Temperature drops below freezing; snow may begin |
| 21 | 大雪 | dàxuě | Major Snow | Dec 6–8 | Heavy snow common; winter intensifies |
| 22 | 冬至 | dōngzhì | Winter Solstice | Dec 21–23 | Shortest day of the year; coldest period begins |
| 23 | 小寒 | xiǎohán | Minor Cold | Jan 5–7 | Very cold; often one of the year's coldest periods |
| 24 | 大寒 | dàhán | Major Cold | Jan 20–21 | Coldest time of the year; end of winter cycle |

#### Astronomical Basis

Each solar term represents a 15° increment along the sun's ecliptic path:
- **0°**: Spring Equinox (春分) — Sun enters Pisces
- **30°**: Clear and Bright (清明)
- **45°**: Start of Summer (立夏)
- **90°**: Summer Solstice (夏至) — Sun enters Cancer
- **180°**: Autumn Equinox (秋分) — Sun enters Virgo
- **270°**: Winter Solstice (冬至) — Sun enters Sagittarius

This astronomical alignment makes the solar terms predictive for seasonal weather patterns and agricultural activities.

#### Cultural Significance

The 24 solar terms remain integral to:
- **Agriculture**: Indicating optimal planting and harvesting times
- **Traditional Medicine**: Influencing treatment recommendations and health practices
- **Festivals**: Marking important cultural celebrations (e.g., Qingming for tomb-sweeping)
- **Weather Prediction**: Providing seasonal weather patterns throughout the year
- **Philosophy**: Reflecting ancient Chinese understanding of cosmic cycles

## Technical Implementation

### Dependencies

The Chinese calendar features rely on:

- **`System.Globalization.ChineseLunisolarCalendar`**: Built-in .NET class providing lunar date calculations based on the Chinese Lunar Calendar system. This eliminates the need for external libraries or complex mathematical algorithms.
- **No external NuGet packages required**: All lunar calendar conversions use native .NET Framework classes.
- **Solar terms**: Use astronomical approximation algorithms to calculate the date when the sun reaches specific ecliptic longitudes.

### Architecture

Three new files implement the Chinese calendar overlay feature:

#### `ChineseCalendarHelper.cs`

A static helper class that wraps `System.Globalization.ChineseLunisolarCalendar` and provides convenience methods for:
- Converting Gregorian dates to lunar dates
- Retrieving sexagenary cycle information (Heavenly Stems and Earthly Branches)
- Determining the current Chinese zodiac animal
- Generating traditional Chinese month and day names
- Calculating leap months
- Formatting lunar date strings in the display format

This class acts as the primary interface for lunar calendar operations throughout the application.

#### `SolarTermCalculator.cs`

Handles 24 solar terms calculation using astronomical principles:
- Calculates the date when the sun reaches each 15° increment along the ecliptic
- Stores pre-calculated dates for all 24 terms for the current and upcoming year
- Provides methods to identify the current or next upcoming solar term
- Formats solar term information for display

The calculation is based on the sun's ecliptic longitude and uses iterative refinement to pinpoint the exact date and time when the sun reaches each critical angle.

#### `LunarClockOverlay.cs`

A WinForms window that displays the lunar calendar overlay:
- Inherits from `Form` and uses P/Invoke to set advanced window properties
- Implements draggable overlay functionality (click anywhere to drag)
- Updates display every second via a `Timer` component
- Right-click to close
- Automatically positions in the bottom-right corner above the taskbar by default
- Applies dark theme styling with semi-transparent appearance
- Integrates with `ChineseCalendarHelper` and `SolarTermCalculator` for data

### Overlay Window Behavior

#### Window Style and Appearance

The overlay is configured with specific Windows window styles to ensure it behaves correctly:

- **Always-on-Top (Topmost)**: Uses `SetWindowPos(..., HWND_TOPMOST, ...)` to keep the overlay visible above other windows
- **Borderless**: No title bar or window controls; clean, minimal appearance
- **No System Menu**: Users cannot minimize, maximize, or resize via standard window controls
- **Extended Styles**: 
  - `WS_EX_NOACTIVATE`: Prevents the overlay from stealing focus when clicked or displayed
  - `WS_EX_TOOLWINDOW`: Hidden from Alt+Tab window switcher; treated as a tool window
  - `WS_EX_LAYERED`: Enables transparency and semi-transparent rendering

#### User Interaction

- **Dragging**: Click anywhere on the overlay to drag and reposition it
- **Right-Click to Close**: Right-click menu displays an option to close the overlay
- **Updates**: The display refreshes every second to show current time and updated solar term information
- **Default Position**: Initially positioned in the bottom-right corner above the taskbar

#### Visual Styling

- **Background**: Dark theme with RGB color (30, 25, 64) — a deep navy-purple
- **Text Color**: White text at 92% opacity for readability against the dark background
- **Font**: Automatically selects best available CJK font (see Font Selection section)
- **Size**: Compact window approximately 200px × 100px (adjustable)

### Font Selection

Chinese characters require specific fonts with full CJK (Chinese, Japanese, Korean) support. The overlay implements an intelligent font fallback chain:

1. **Microsoft YaHei UI** (微软雅黑 UI)
   - Default Chinese font on Windows 10/11
   - Modern design; excellent readability
   - Full Unicode support including all CJK characters

2. **Microsoft YaHei** (微软雅黑)
   - Slightly older version; available on Windows 7+
   - Fallback if Microsoft YaHei UI is unavailable

3. **SimHei** (黑体)
   - Traditional simplified Chinese font
   - Legacy fallback for systems without Microsoft fonts
   - May appear less polished but still readable

4. **Segoe UI** (非-CJK Fallback)
   - Windows default UI font
   - Used only if no CJK fonts are available
   - May display Chinese characters with square boxes if CJK font support is truly absent

The application tests font availability at startup and selects the first available option from this chain. Users can check which font was selected in the application logs or overlay title bar (if shown during development).

## Display Format

The overlay displays lunar calendar information in a compact, vertically-stacked format:

```
11:14:05  26-04-16
乙巳年 蛇 三月 十九 三
节气: 谷雨 (04-20)
```

### Format Breakdown

**Line 1: Time and Gregorian Date**
- `11:14:05` — Current time in 24-hour HH:MM:SS format
- `26-04-16` — Gregorian date in DD-MM-YY format (April 16, 2026)

**Line 2: Lunar Calendar Information**
- `乙巳年` — Sexagenary cycle designation (Heavenly Stem + Earthly Branch) for the lunar year
  - 乙 (Stem 2 — Yin Wood)
  - 巳 (Branch 6 — Snake)
  - Together: Year of the Snake
- `蛇` — Chinese zodiac animal (Snake)
- `三月` — Lunar month (3rd month; use 闰三月 for leap 3rd month)
- `十九` — Lunar day (19th day using traditional naming: 十九)
- `三` — Chinese day of the week (Wednesday — 星期三)

**Line 3: Solar Term**
- `节气: 谷雨 (04-20)` — Current or next upcoming solar term
  - 谷雨 — "Grain Rain" (6th solar term)
  - (04-20) — Approximate date in MM-DD format (April 20)

### Example Display Variations

**Winter Solstice (冬至)**
```
22:35:12  21-12-25
乙未年 羊 十一月 初七 日
节气: 冬至 (12-21)
```

**Lunar New Year (正月初一)**
```
00:00:01  01-02-24
甲龙年 龙 正月 初一 一
节气: 立春 (02-04)
```

**With Leap Month**
```
15:47:33  10-06-25
乙巳年 蛇 闰五月 初八 五
节气: 夏至 (06-21)
```

## Configuration

Currently, the Chinese calendar overlay is a toggle-on/toggle-off feature accessible via the system tray right-click context menu → **Show Chinese Calendar**. The overlay state persists across application restarts.

### Future Enhancement Possibilities

While the current implementation provides core functionality, the following enhancements could be added in future versions:

- **Customizable Position**: Allow users to save and restore preferred overlay position
- **Font Size Adjustment**: Slider or dropdown to scale the overlay for different preferences
- **Color Themes**: Alternative color schemes (light mode, seasonal themes, high-contrast mode)
- **Display Options**: Toggle which calendar elements to show:
  - Hide/show Gregorian time
  - Hide/show sexagenary cycle
  - Hide/show zodiac animal
  - Hide/show solar term information
- **Timezone Support**: Display lunar calendar for different time zones
- **Multi-Monitor Support**: Remember which monitor to display on
- **Opacity Control**: Adjust window transparency via context menu
- **Click-Through Mode**: Allow clicks to pass through to windows behind the overlay

## References

### Official Documentation

- **System.Globalization.ChineseLunisolarCalendar**: https://learn.microsoft.com/en-us/dotnet/api/system.globalization.chineselunissolarcalendar
  - Official .NET documentation for the lunar calendar system used in this feature

### Wikipedia

- **Chinese Calendar**: https://en.wikipedia.org/wiki/Chinese_calendar
  - Comprehensive overview of the lunar calendar structure, months, days, and intercalation system

- **Solar Term**: https://en.wikipedia.org/wiki/Solar_term
  - Detailed explanation of the 24 solar terms, their astronomical basis, and cultural significance

- **Sexagenary Cycle**: https://en.wikipedia.org/wiki/Sexagenary_cycle
  - Historical context, application in various East Asian cultures, and the 60-year repeating pattern

- **Chinese Zodiac**: https://en.wikipedia.org/wiki/Chinese_zodiac
  - Overview of the 12 zodiac animals, personality associations, and astrological interpretations

### Additional Resources

- **Traditional Chinese Calendar**: https://en.wikipedia.org/wiki/Traditional_Chinese_calendar
  - Deep dive into calendar philosophy and historical development

- **East Asian Lunisolar Calendars**: https://en.wikipedia.org/wiki/Lunisolar_calendar
  - How lunar and solar calendars interact across multiple cultures

- **UNESCO Intangible Heritage - Solar Terms**: https://ich.unesco.org/
  - Recognition of solar terms as cultural heritage

## Support

For questions, issues, or feature requests related to the Chinese calendar feature, please open an issue on the [GitHub repository](https://github.com/elbruno/ElBruno.ClockTray/issues).

---

**Last Updated**: 2024  
**Feature Branch**: feature/chinese-calendar-complete
