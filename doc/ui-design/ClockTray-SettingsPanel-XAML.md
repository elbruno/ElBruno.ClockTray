# ClockTray Settings Panel — XAML Layout Outline

**Sprint 1 Design Document**  
**Prepared by:** Blain (UI Developer)  
**Date:** March 2026  
**Target Framework:** WinUI 3 / Windows App SDK

---

## Overview

This document outlines the XAML structure for the ClockTray settings panel that will integrate with the PowerToys settings app. The panel follows PowerToys visual language and uses standard WinUI 3 controls for consistency, accessibility, and maintainability.

---

## Design Goals

- **Simplicity:** One master toggle + three optional configuration settings
- **Consistency:** Align with PowerToys settings UI patterns (reference: Awake, Color Picker modules)
- **Accessibility:** Full keyboard navigation, ARIA labels, high contrast support
- **Responsiveness:** Adapt to different window sizes and DPI scaling
- **Clarity:** Clear labels and help text for each setting

---

## XAML Structure

### Root Container

```xml
<Page
    x:Class="PowerToys.ClockTray.Settings.ClockTrayPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
    Background="{ThemeResource ApplicationPageBackgroundThemeBrush}"
    Padding="24,12">

    <StackPanel Spacing="24" MaxWidth="1200">
        <!-- Content goes here -->
    </StackPanel>
</Page>
```

**Rationale:**
- `MaxWidth="1200"` aligns with PowerToys settings page layout (prevents excessive width on large screens)
- `Padding="24,12"` matches PowerToys margin conventions
- `StackPanel` with `Spacing="24"` provides vertical rhythm consistent with PowerToys design

---

## Section 1: Header

```xml
<StackPanel Spacing="8">
    <TextBlock 
        Text="Clock Tray" 
        Style="{StaticResource HeadingTextBlockStyle}" />
    <TextBlock 
        Text="Toggle the Windows taskbar clock and time display with a single hotkey or menu click."
        Style="{StaticResource BodyTextBlockStyle}"
        Foreground="{ThemeResource TextSecondaryBrush}" 
        TextWrapping="Wrap" />
</StackPanel>
```

**Components:**
- **Title:** Uses `HeadingTextBlockStyle` (consistent with PowerToys module headers)
- **Description:** Secondary text explaining the utility's purpose; helps new users understand functionality

---

## Section 2: Master Toggle (Enable/Disable)

```xml
<Grid ColumnSpacing="12" Padding="0,12">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="Auto" />
    </Grid.ColumnDefinitions>

    <StackPanel Spacing="4" VerticalAlignment="Center">
        <TextBlock 
            Text="Enable ClockTray" 
            Style="{StaticResource BaseTextBlockStyle}" 
            FontWeight="SemiBold" />
        <TextBlock 
            Text="Turn on to enable the Clock Tray module. When disabled, no hotkeys are registered and the clock state is unchanged."
            Style="{StaticResource CaptionTextBlockStyle}"
            Foreground="{ThemeResource TextSecondaryBrush}" 
            TextWrapping="Wrap"
            MaxWidth="600" />
    </StackPanel>

    <muxc:ToggleSwitch 
        x:Name="EnableToggle"
        Grid.Column="1" 
        VerticalAlignment="Center"
        IsOn="{x:Bind ViewModel.IsEnabled, Mode=TwoWay}"
        AutomationProperties.Name="Enable Clock Tray module" />
</Grid>
```

**Components:**
- **Layout:** Grid with two columns (content left, toggle right)
- **Label:** Title + help text explaining the toggle's effect
- **ToggleSwitch:** WinUI 3's accessible toggle control
- **Binding:** `IsOn` binds to ViewModel.IsEnabled (two-way)
- **Accessibility:** `AutomationProperties.Name` for screen readers

---

## Section 3: Registry Method Selection

```xml
<muxc:InfoBar 
    Title="Registry Method Selection"
    Message="Choose the method used to toggle the taskbar clock. PowerToys auto-detects your OS version and selects the appropriate method automatically."
    Severity="Informational"
    IsOpen="True"
    IsClosable="True" />

<Grid ColumnSpacing="12" Padding="0,12">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="Auto" MinWidth="300" />
    </Grid.ColumnDefinitions>

    <StackPanel Spacing="4" VerticalAlignment="Center">
        <TextBlock 
            Text="Registry Method" 
            Style="{StaticResource BaseTextBlockStyle}" 
            FontWeight="SemiBold" />
        <TextBlock 
            Text="Modern method (Win11 23H2+) is seamless. Legacy method (Win10) requires a brief Explorer restart."
            Style="{StaticResource CaptionTextBlockStyle}"
            Foreground="{ThemeResource TextSecondaryBrush}" 
            TextWrapping="Wrap"
            MaxWidth="600" />
    </StackPanel>

    <muxc:ComboBox 
        x:Name="MethodComboBox"
        Grid.Column="1"
        VerticalAlignment="Center"
        SelectedValuePath="Tag"
        SelectedValue="{x:Bind ViewModel.SelectedMethod, Mode=TwoWay}"
        AutomationProperties.Name="Select registry method"
        IsEnabled="{x:Bind ViewModel.IsEnabled, Mode=OneWay}">
        <muxc:ComboBoxItem Tag="Auto" Content="Auto-detect (Recommended)" />
        <muxc:ComboBoxItem Tag="Modern" Content="Modern (Win11 23H2+)" />
        <muxc:ComboBoxItem Tag="Legacy" Content="Legacy (Win10)" />
    </muxc:ComboBox>
</Grid>

<TextBlock 
    Text="{x:Bind ViewModel.MethodDescription, Mode=OneWay}"
    Style="{StaticResource CaptionTextBlockStyle}"
    Foreground="{ThemeResource TextSecondaryBrush}" 
    Margin="0,8,0,0"
    TextWrapping="Wrap" />
```

**Components:**
- **InfoBar:** Provides context about the registry method choice (PowerToys design pattern)
- **ComboBox:** Three options (Auto, Modern, Legacy) with descriptive names
- **Help Text:** Updates dynamically based on selected method
- **Disabled State:** ComboBox is disabled when module is off
- **Accessibility:** `AutomationProperties.Name` labels the dropdown

**Binding Details:**
- `SelectedValue` binds to ViewModel.SelectedMethod (enum or string)
- `IsEnabled` depends on `ViewModel.IsEnabled` (master toggle controls sub-settings)
- Help text updates via `ViewModel.MethodDescription` (computed property)

---

## Section 4: Hotkey Picker

```xml
<Grid ColumnSpacing="12" Padding="0,12">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="Auto" MinWidth="300" />
    </Grid.ColumnDefinitions>

    <StackPanel Spacing="4" VerticalAlignment="Center">
        <TextBlock 
            Text="Activation Hotkey" 
            Style="{StaticResource BaseTextBlockStyle}" 
            FontWeight="SemiBold" />
        <TextBlock 
            Text="Press any key combination to set. Hotkey must not conflict with system shortcuts (e.g., Win+Key). Default: Ctrl+Alt+T"
            Style="{StaticResource CaptionTextBlockStyle}"
            Foreground="{ThemeResource TextSecondaryBrush}" 
            TextWrapping="Wrap"
            MaxWidth="600" />
    </StackPanel>

    <StackPanel Grid.Column="1" Spacing="8" VerticalAlignment="Center">
        <TextBox 
            x:Name="HotkeyTextBox"
            PlaceholderText="Press keys..."
            Text="{x:Bind ViewModel.HotkeyDisplay, Mode=OneWay}"
            IsReadOnly="True"
            AutomationProperties.Name="Hotkey display (read-only)" />
        
        <muxc:Button 
            Content="Record Hotkey"
            Click="OnRecordHotkeyClick"
            IsEnabled="{x:Bind ViewModel.IsEnabled, Mode=OneWay}"
            AutomationProperties.Name="Start recording a new hotkey" />
    </StackPanel>
</Grid>

<TextBlock 
    x:Name="HotkeyStatusText"
    Style="{StaticResource CaptionTextBlockStyle}"
    Foreground="{ThemeResource TextSecondaryBrush}" 
    Margin="0,8,0,0"
    TextWrapping="Wrap" />
```

**Components:**
- **TextBox (Read-only):** Displays current hotkey in human-readable form (e.g., "Ctrl+Alt+T")
- **Record Button:** Activates hotkey recording mode
- **Status Text:** Shows validation messages (e.g., "Hotkey conflicts with Windows shortcut", "Hotkey set successfully")
- **Disabled State:** Button is disabled when module is off

**Behavior:**
1. User clicks "Record Hotkey"
2. Button changes to "Recording..." (visual feedback)
3. Next key press is captured and validated
4. If valid: Display in TextBox, save to config
5. If invalid: Show error in HotkeyStatusText, allow retry

---

## Section 5: Status Display

```xml
<muxc:Expander 
    Header="System State"
    IsExpanded="False"
    Padding="0,12">

    <Grid RowSpacing="8">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <StackPanel Orientation="Horizontal" Spacing="12">
            <TextBlock 
                Text="Clock Visibility:" 
                Style="{StaticResource BaseTextBlockStyle}" />
            <TextBlock 
                Text="{x:Bind ViewModel.ClockVisibilityStatus, Mode=OneWay}"
                Style="{StaticResource BaseTextBlockStyle}" 
                FontWeight="SemiBold"
                Foreground="{x:Bind ViewModel.ClockVisibilityColor, Mode=OneWay}" />
        </StackPanel>

        <StackPanel Grid.Row="1" Orientation="Horizontal" Spacing="12">
            <TextBlock 
                Text="Current Method:" 
                Style="{StaticResource BaseTextBlockStyle}" />
            <TextBlock 
                Text="{x:Bind ViewModel.CurrentMethodName, Mode=OneWay}"
                Style="{StaticResource BaseTextBlockStyle}" 
                FontWeight="SemiBold" />
        </StackPanel>

        <StackPanel Grid.Row="2" Orientation="Horizontal" Spacing="12">
            <TextBlock 
                Text="OS Build:" 
                Style="{StaticResource BaseTextBlockStyle}" />
            <TextBlock 
                Text="{x:Bind ViewModel.OSBuildInfo, Mode=OneWay}"
                Style="{StaticResource BaseTextBlockStyle}" 
                FontWeight="SemiBold" />
        </StackPanel>
    </Grid>
</muxc:Expander>
```

**Components:**
- **Expander:** Collapses status section by default (hides complexity for casual users)
- **Status Items:** Clock visibility, active method, OS build info
- **Color-Coded Visibility:** Green = visible, Red = hidden (via ViewModel binding)
- **Read-Only:** Status is informational; user cannot edit directly

---

## Section 6: Footer & Reset Option

```xml
<Separator Margin="0,8" />

<StackPanel Spacing="8">
    <muxc:Button 
        Content="Reset to Defaults"
        Click="OnResetClick"
        Background="{ThemeResource SubtleButtonBackground}"
        AutomationProperties.Name="Reset all settings to default values" />
    
    <TextBlock 
        Text="This will reset the hotkey to Ctrl+Alt+T and set the registry method to Auto-detect."
        Style="{StaticResource CaptionTextBlockStyle}"
        Foreground="{ThemeResource TextSecondaryBrush}" />
</StackPanel>
```

**Components:**
- **Reset Button:** Restores defaults (common PowerToys pattern)
- **Help Text:** Explains what "reset" means

---

## Layout & Responsiveness

### Breakpoints

| Viewport Width | Behavior |
|---|---|
| < 600px | Single column layout (stack toggle and combobox vertically) |
| 600px – 1200px | Two-column grid as designed above |
| > 1200px | Two-column grid with `MaxWidth="1200"` applied to root |

**Responsive Grid Technique:**
```xml
<Grid ColumnSpacing="12">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" MinWidth="200" />
        <ColumnDefinition x:Name="RightColumn" Width="Auto" MinWidth="300" />
    </Grid.ColumnDefinitions>
    
    <!-- On small screens, hide RightColumn and reflow to single column -->
    <!-- Handled in code-behind or via VisualStateManager -->
</Grid>
```

### DPI Scaling

- Use `ThemeResource` for all colors, spacing, fonts (automatically scales with DPI)
- Use `em`-based sizing for text (relative to font size)
- Test on 100%, 125%, 150%, 200% DPI scaling

---

## Theming & Accessibility

### Light/Dark Theme Support

```xml
<!-- All colors reference theme resources -->
Background="{ThemeResource ApplicationPageBackgroundThemeBrush}"
Foreground="{ThemeResource TextPrimaryBrush}"
Foreground="{ThemeResource TextSecondaryBrush}" <!-- for secondary/disabled text -->
```

**Dark Mode:** TextBlock colors invert automatically; no hardcoded RGB values.

### Accessibility Checklist

- [ ] All interactive controls have `AutomationProperties.Name`
- [ ] Color is not the only differentiator (e.g., status text uses "Visible"/"Hidden", not just green/red)
- [ ] Keyboard navigation works (Tab through all controls, Enter/Space activates buttons)
- [ ] Focus indicators are visible (WinUI provides default; don't remove)
- [ ] Labels are associated with controls (use `for` in HTML; use `AutomationProperties.LabeledBy` in XAML)
- [ ] High contrast mode supported (test with `Settings > Ease of Access > High Contrast`)

---

## Control Summary Table

| Control | Type | Binding | Purpose |
|---|---|---|---|
| EnableToggle | ToggleSwitch | IsEnabled (ViewModel) | Master module enable/disable |
| MethodComboBox | ComboBox | SelectedValue (ViewModel) | Registry method selection |
| HotkeyTextBox | TextBox | Text (ViewModel, OneWay) | Display current hotkey |
| RecordButton | Button | IsEnabled (ViewModel) | Start hotkey recording |
| HotkeyStatusText | TextBlock | Text (ViewModel, OneWay) | Validation/status messages |
| ClockVisibilityStatus | TextBlock | Text (ViewModel, OneWay) | Show current clock state |
| ResetButton | Button | Click event | Restore defaults |

---

## ViewModel Interface (C# Backend)

The XAML binds to a ViewModel exposing these properties:

```csharp
public class ClockTraySettingsViewModel : INotifyPropertyChanged
{
    public bool IsEnabled { get; set; }
    public string SelectedMethod { get; set; } // "Auto", "Modern", "Legacy"
    public string MethodDescription { get; set; } // Dynamic help text
    public string HotkeyDisplay { get; set; } // Human-readable hotkey (e.g., "Ctrl+Alt+T")
    public string ClockVisibilityStatus { get; set; } // "Visible" or "Hidden"
    public SolidColorBrush ClockVisibilityColor { get; set; } // Green or Red
    public string CurrentMethodName { get; set; } // Which method is active
    public string OSBuildInfo { get; set; } // "Windows 11 Build 23H2"
    public event PropertyChangedEventHandler? PropertyChanged;
}
```

---

## Design Decisions

1. **ToggleSwitch over RadioButton:**
   - ToggleSwitch provides immediate visual feedback (on/off is clearer than radio buttons)
   - Aligns with PowerToys conventions (see: FancyZones, Awake modules)

2. **ComboBox for Registry Method:**
   - Allows three discrete choices without visual clutter
   - Auto-detect as default reduces cognitive load for average users

3. **Read-Only TextBox for Hotkey Display:**
   - Prevents accidental editing; clear that it's display-only
   - Record button clarifies the action needed to change hotkey

4. **Expander for Status:**
   - Hides diagnostic info by default (power-user feature)
   - Reduces visual complexity for typical users

5. **Dynamic Help Text:**
   - MethodDescription updates based on SelectedMethod choice
   - Provides context-aware guidance without cluttering the UI

---

## Future Enhancements

- **Hotkey Conflict Detection:** Warn if user's chosen hotkey conflicts with Windows shortcuts
- **Quick Toggle Button:** Add a "Test Now" button to toggle the clock without leaving settings
- **Telemetry:** Track usage (e.g., which method is most common, default hotkey retention)
- **Advanced Options:** Expandable section for OS-specific registry path customization (expert users)

---

## References

- PowerToys Settings UI Patterns: https://github.com/microsoft/PowerToys/tree/main/src/settings-ui
- WinUI 3 Controls Gallery: https://github.com/microsoft/WinUI-Gallery
- PowerToys Awake Module: https://github.com/microsoft/PowerToys/tree/main/src/modules/Awake
- WinUI 3 Accessibility Guide: https://learn.microsoft.com/en-us/windows/apps/design/accessibility/
