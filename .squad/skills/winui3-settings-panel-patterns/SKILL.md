# WinUI 3 Settings Panel Design Patterns — PowerToys Integration

**Skill Level:** Intermediate (requires WinUI 3 / C# knowledge)  
**Applicable To:** PowerToys modules, Windows App SDK applications  
**Created:** March 10, 2026 (ClockTray Sprint 1)

---

## Overview

This skill documents proven patterns for designing settings panels in WinUI 3 that integrate with PowerToys settings app. These patterns are based on analysis of existing PowerToys modules (Awake, Color Picker, FancyZones) and Sprint 1 ClockTray design work.

---

## Pattern 1: Two-Column Label-Control Layout

### Use Case
Displaying settings with associated labels and help text in a compact, scannable format.

### Implementation

**XAML:**
```xml
<Grid ColumnSpacing="12">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" MinWidth="200" />
        <ColumnDefinition Width="Auto" MinWidth="300" />
    </Grid.ColumnDefinitions>

    <!-- Left: Label + Help Text -->
    <StackPanel Spacing="4" VerticalAlignment="Center">
        <TextBlock 
            Text="Setting Name" 
            Style="{StaticResource BaseTextBlockStyle}" 
            FontWeight="SemiBold" />
        <TextBlock 
            Text="Help text explaining the setting..."
            Style="{StaticResource CaptionTextBlockStyle}"
            Foreground="{ThemeResource TextSecondaryBrush}" 
            TextWrapping="Wrap" />
    </StackPanel>

    <!-- Right: Control -->
    <muxc:ToggleSwitch 
        Grid.Column="1"
        VerticalAlignment="Center"
        IsOn="{x:Bind ViewModel.Setting, Mode=TwoWay}" />
</Grid>
```

### Responsive Behavior
```csharp
// In Page code-behind or VisualStateManager
private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
{
    if (e.Size.Width < 600)
    {
        // Switch to single-column layout
        // Move control below label
    }
}
```

### Advantages
- **Scannable:** Users quickly spot setting names (bold, left) and controls (right)
- **Space Efficient:** Uses 70% + 30% split; works on tablets and desktops
- **Alignment:** All controls right-aligned creates visual coherence
- **Flexibility:** Help text can be short or long without breaking layout

### When to Use
- Settings that benefit from explanatory text
- Panels with 3–8 settings
- Desktop/tablet-focused apps (mobile may need single-column)

### When NOT to Use
- Simple dialogs with 1–2 controls (overkill)
- Mobile-first UX (use single-column stacking instead)

---

## Pattern 2: ToggleSwitch for Binary Enable/Disable

### Use Case
Master control to activate/deactivate a feature or module.

### Implementation

**XAML:**
```xml
<muxc:ToggleSwitch 
    x:Name="EnableFeature"
    IsOn="{x:Bind ViewModel.IsEnabled, Mode=TwoWay}"
    AutomationProperties.Name="Enable feature name" />
```

**ViewModel (C#):**
```csharp
public class SettingsViewModel : INotifyPropertyChanged
{
    private bool _isEnabled;
    
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (value != _isEnabled)
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
                // Cascade disable sub-settings
                OnPropertyChanged(nameof(IsSubSettingEnabled));
            }
        }
    }

    public bool IsSubSettingEnabled => IsEnabled; // Computed property
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

### Binding Sub-Settings
```xml
<!-- Sub-setting: Only enabled if master toggle is ON -->
<muxc:ComboBox 
    IsEnabled="{x:Bind ViewModel.IsEnabled, Mode=OneWay}"
    SelectedValue="{x:Bind ViewModel.SubSetting, Mode=TwoWay}" />
```

### Key Behaviors
1. **Immediate Feedback:** Toggle slides; no confirmation needed
2. **Cascading Disable:** Sub-settings greyed out when master is OFF
3. **No State Loss:** Turning OFF preserves config; turning back ON restores previous settings
4. **Keyboard Support:** Space/Enter toggles; Tab navigates

### Advantages
- **Familiar:** Standard Windows 11 UI control
- **Accessible:** WinUI 3 ToggleSwitch has built-in ARIA support
- **Lightweight:** No modal dialogs or confirmations (unless destructive)
- **Visual Clarity:** "On/Off" state immediately obvious

### Anti-Patterns to Avoid
- ❌ Using ToggleSwitch for multi-state settings (use ComboBox instead)
- ❌ Destroying config when toggling OFF (always preserve)
- ❌ Requiring confirmation for toggle (makes UX sluggish)

---

## Pattern 3: ComboBox for Discrete Options

### Use Case
Selecting one of 3–5 mutually exclusive options (method, algorithm, etc.).

### Implementation

**XAML:**
```xml
<muxc:ComboBox 
    x:Name="MethodSelection"
    SelectedValuePath="Tag"
    SelectedValue="{x:Bind ViewModel.SelectedMethod, Mode=TwoWay}"
    AutomationProperties.Name="Select method">
    <muxc:ComboBoxItem Tag="Auto" Content="Auto-detect (Recommended)" />
    <muxc:ComboBoxItem Tag="Modern" Content="Modern (Win11 23H2+)" />
    <muxc:ComboBoxItem Tag="Legacy" Content="Legacy (Win10)" />
</muxc:ComboBox>
```

**ViewModel (C#):**
```csharp
public class SettingsViewModel : INotifyPropertyChanged
{
    private string _selectedMethod = "Auto";

    public string SelectedMethod
    {
        get => _selectedMethod;
        set
        {
            if (value != _selectedMethod)
            {
                _selectedMethod = value;
                OnPropertyChanged(nameof(SelectedMethod));
                OnPropertyChanged(nameof(MethodDescription)); // Trigger help text update
            }
        }
    }

    public string MethodDescription => _selectedMethod switch
    {
        "Auto" => "Will use Modern on Win11 23H2+, Legacy on Win10.",
        "Modern" => "Seamless toggle. Recommended for Windows 11 23H2+.",
        "Legacy" => "May require Explorer restart. Use on Windows 10.",
        _ => ""
    };
}
```

### Dynamic Help Text Pattern
```xml
<!-- Help text updates automatically when selection changes -->
<TextBlock 
    Text="{x:Bind ViewModel.MethodDescription, Mode=OneWay}"
    Style="{StaticResource CaptionTextBlockStyle}"
    Foreground="{ThemeResource TextSecondaryBrush}" />
```

### Advantages
- **Compact:** Takes minimal vertical space (vs. radio buttons)
- **Discoverable:** Dropdown naturally suggests "choose one"
- **Scalable:** Easy to add 4th or 5th option
- **Context-Aware:** Help text can change based on selection

### When to Use
- 3–5 mutually exclusive options
- Settings with complex trade-offs (modern vs. legacy, quality vs. speed, etc.)
- Space-constrained panels

### When NOT to Use
- Binary choices (use ToggleSwitch instead)
- More than 7 options (use ListBox or NavigationView)
- Frequently changed options (radio buttons may be faster)

---

## Pattern 4: Record Button + Read-Only TextBox for Hotkey Entry

### Use Case
Capturing keyboard shortcuts without typos; validating in real-time.

### Implementation

**XAML:**
```xml
<StackPanel Spacing="8">
    <!-- Display current hotkey -->
    <TextBox 
        x:Name="HotkeyDisplay"
        Text="{x:Bind ViewModel.HotkeyDisplay, Mode=OneWay}"
        PlaceholderText="Press keys..."
        IsReadOnly="True"
        IsEnabled="{x:Bind ViewModel.IsEnabled, Mode=OneWay}" />
    
    <!-- Record button to start recording -->
    <muxc:Button 
        Content="Record Hotkey"
        Click="OnRecordHotkeyClick"
        IsEnabled="{x:Bind ViewModel.IsEnabled, Mode=OneWay}" />
</StackPanel>

<!-- Status message (validation feedback) -->
<TextBlock 
    x:Name="HotkeyStatus"
    Text="{x:Bind ViewModel.HotkeyStatusMessage, Mode=OneWay}"
    Foreground="{x:Bind ViewModel.HotkeyStatusColor, Mode=OneWay}" />
```

**ViewModel (C#):**
```csharp
public class SettingsViewModel : INotifyPropertyChanged
{
    private string _hotkeyDisplay = "Ctrl+Alt+T";
    private string _hotkeyStatusMessage = "";
    private bool _isRecording = false;

    public string HotkeyDisplay
    {
        get => _hotkeyDisplay;
        set { _hotkeyDisplay = value; OnPropertyChanged(nameof(HotkeyDisplay)); }
    }

    public string HotkeyStatusMessage
    {
        get => _hotkeyStatusMessage;
        set { _hotkeyStatusMessage = value; OnPropertyChanged(nameof(HotkeyStatusMessage)); }
    }

    public SolidColorBrush HotkeyStatusColor
    {
        get
        {
            return HotkeyStatusMessage.StartsWith("✅") ? new SolidColorBrush(Colors.Green) :
                   HotkeyStatusMessage.StartsWith("❌") ? new SolidColorBrush(Colors.Red) :
                   new SolidColorBrush(Colors.Orange);
        }
    }
}
```

**Code-Behind (C#):**
```csharp
private void OnRecordHotkeyClick(object sender, RoutedEventArgs e)
{
    ViewModel._isRecording = true;
    
    // Global hotkey listener capture next key press
    RegisterGlobalKeyListener();
    
    // Change button appearance/label
    (sender as Button).Content = "Recording...";
}

private void OnGlobalKeyPressed(int vkCode, int modifier)
{
    if (!ViewModel._isRecording) return;

    // Validate hotkey
    string hotkey = ConvertToHotkeyString(modifier, vkCode);
    
    if (ValidateHotkey(hotkey))
    {
        ViewModel.HotkeyDisplay = hotkey;
        ViewModel.HotkeyStatusMessage = "✅ Hotkey set successfully.";
        ViewModel._isRecording = false;
        // Update button
    }
    else
    {
        ViewModel.HotkeyStatusMessage = "❌ Hotkey must include a modifier key (Ctrl, Alt, Shift, Windows).";
        // Keep recording mode active
    }
}
```

### Validation Rules
1. **Modifier Required:** Ctrl, Alt, Shift, or Windows key
2. **Single Primary Key:** Exactly one non-modifier key
3. **No System Conflicts:** Check against Windows reserved shortcuts
4. **No PowerToys Conflicts:** Check against other modules' hotkeys

### Advantages
- **Prevents Typos:** Keyboard input only (not free typing)
- **Familiar Pattern:** OBS, game launchers use similar UI
- **Real-Time Validation:** Immediate feedback during recording
- **Clear Intent:** Record button clarifies the action

### When to Use
- Hotkey/keyboard shortcut configuration
- Any setting that requires exact keyboard input
- Advanced power-user features

---

## Pattern 5: InfoBar for Contextual Guidance

### Use Case
Providing background information without cluttering main UI.

### Implementation

**XAML:**
```xml
<muxc:InfoBar 
    Title="Feature Title"
    Message="Explanation of what this feature does and how it works. Can include recommendations."
    Severity="Informational"
    IsOpen="True"
    IsClosable="True" />
```

### Severity Levels
- **Informational:** General info, tips, explanations
- **Success:** Operation completed successfully
- **Warning:** User should be aware (e.g., action has side effects)
- **Error:** Something failed; user action needed

### When to Use
- First-time user guidance
- Explaining complex settings
- Providing context for mutually exclusive options
- Non-blocking warnings

### When NOT to Use
- Critical errors (use modal dialog instead)
- Every setting (would clutter UI)
- Information that fits in help text below control

---

## Pattern 6: Expander for Advanced/Power-User Options

### Use Case
Hiding diagnostic or advanced settings to reduce visual complexity.

### Implementation

**XAML:**
```xml
<muxc:Expander 
    Header="Advanced Options"
    IsExpanded="False">
    
    <StackPanel Spacing="12">
        <TextBlock Text="Option 1: Value" />
        <TextBlock Text="Option 2: Value" />
        <TextBlock Text="Option 3: Value" />
    </StackPanel>
</muxc:Expander>
```

### Behavior
- **Default:** Collapsed (hidden)
- **Click Header:** Expands/collapses with animation
- **Visual Cue:** Arrow rotates 90° to indicate expansion state

### Performance Tip
```csharp
// Only poll/update expanded content when Expander is open
private void OnExpanderStateChanged(object sender, RoutedEventArgs e)
{
    Expander expander = sender as Expander;
    
    if (expander.IsExpanded)
    {
        StartStatusPolling(); // Expensive operation
    }
    else
    {
        StopStatusPolling(); // Save CPU
    }
}
```

### When to Use
- Diagnostic info (OS build, method in use, etc.)
- Advanced/expert options
- Infrequently used settings
- Settings that are "nice to know" but not essential

### When NOT to Use
- Critical settings that users must configure
- Settings that users change frequently
- Information needed to understand the panel

---

## Pattern 7: Reset to Defaults with Confirmation

### Use Case
Allow users to restore factory settings without accidents.

### Implementation

**XAML:**
```xml
<muxc:Button 
    Content="Reset to Defaults"
    Click="OnResetClick"
    Background="{ThemeResource SubtleButtonBackground}" />

<!-- Confirmation ContentDialog -->
<muxc:ContentDialog 
    x:Name="ResetConfirmDialog"
    Title="Reset Settings to Defaults?"
    PrimaryButtonText="Reset"
    SecondaryButtonText="Cancel"
    PrimaryButtonClick="OnConfirmReset">
    
    <TextBlock TextWrapping="Wrap">
        <Run Text="This will reset:" FontWeight="SemiBold" /><LineBreak />
        <Run Text="• Hotkey: Ctrl+Alt+T" /><LineBreak />
        <Run Text="• Method: Auto-detect" /><LineBreak />
    </TextBlock>
</muxc:ContentDialog>
```

**Code-Behind (C#):**
```csharp
private async void OnResetClick(object sender, RoutedEventArgs e)
{
    ContentDialogResult result = await ResetConfirmDialog.ShowAsync();
    
    if (result == ContentDialogResult.Primary)
    {
        ViewModel.ResetToDefaults();
        ShowResetToast();
    }
}

private void ShowResetToast()
{
    var toast = new TeachingTip
    {
        Title = "Settings Reset",
        Subtitle = "All settings restored to defaults.",
        IsOpen = true
    };
    // Or use AppNotification API
}
```

### Advantages
- **Prevents Accidents:** Confirmation prevents accidental resets
- **Clear Consequences:** Dialog lists what will be reset
- **Non-Disruptive:** Confirmation is modal; no missed feedback
- **Recoverable:** User can manually undo if needed

---

## Pattern 8: Using Theme Resources for Light/Dark Mode

### Use Case
Ensuring UI renders correctly in both light and dark themes without manual adjustments.

### Implementation

**Best Practices:**
```xml
<!-- ✅ GOOD: Use ThemeResource bindings -->
<StackPanel Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
    <TextBlock 
        Text="Primary text"
        Foreground="{ThemeResource TextPrimaryBrush}" />
    <TextBlock 
        Text="Secondary text"
        Foreground="{ThemeResource TextSecondaryBrush}" />
</StackPanel>

<!-- ❌ BAD: Hardcoded colors -->
<StackPanel Background="#FFFFFF">
    <TextBlock Text="Text" Foreground="#000000" />
</StackPanel>
```

### Key Theme Resources
| Resource | Light | Dark |
|---|---|---|
| `ApplicationPageBackgroundThemeBrush` | White | Dark grey |
| `TextPrimaryBrush` | Black | White |
| `TextSecondaryBrush` | Grey | Light grey |
| `ControlBackgroundBrush` | Light grey | Darker grey |

### DPI Scaling Automatic
- Theme resources automatically scale at 100%, 125%, 150%, 175%, 200%+
- No manual margin/font adjustments needed

---

## Pattern 9: Two-Way Data Binding with MVVM

### Use Case
Keeping UI and config in sync automatically.

### Implementation

**ViewModel (C#):**
```csharp
public class SettingsViewModel : INotifyPropertyChanged
{
    private bool _isEnabled;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (value != _isEnabled)
            {
                _isEnabled = value;
                SaveConfigToJson(); // Persist on change
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SaveConfigToJson()
    {
        var config = new
        {
            enabled = _isEnabled,
            method = SelectedMethod,
            hotkey = HotkeyDisplay
        };
        
        File.WriteAllText(
            Path.Combine(AppContext.BaseDirectory, "config.json"),
            JsonSerializer.Serialize(config)
        );
    }
}
```

**XAML (Two-Way Binding):**
```xml
<muxc:ToggleSwitch 
    IsOn="{x:Bind ViewModel.IsEnabled, Mode=TwoWay}" />
```

### Binding Modes
- **TwoWay:** UI ↔ ViewModel (use for user inputs; saves on change)
- **OneWay:** ViewModel → UI (use for read-only displays)
- **OneWayToSource:** UI → ViewModel (rare; use for button clicks)

### Advantages
- **Automatic Sync:** No manual assignment needed
- **Persistence:** Change handler can save config automatically
- **Clean Code:** Binding expression replaces boilerplate getters/setters

---

## Common Pitfalls & How to Avoid Them

### Pitfall 1: Hardcoded Colors
**Problem:** ❌ `Foreground="#FF000000"` doesn't respect dark mode  
**Solution:** ✅ `Foreground="{ThemeResource TextPrimaryBrush}"`

### Pitfall 2: No Keyboard Navigation
**Problem:** ❌ All controls hidden behind tabs/clicks; tab key doesn't work  
**Solution:** ✅ Ensure all controls are Tab-navigable; use `IsTabStop="True"` if needed

### Pitfall 3: Blocking UI with Modals
**Problem:** ❌ Toast notifications replaced by modal dialogs; UX feels sluggish  
**Solution:** ✅ Use modals only for destructive actions; use toasts for routine feedback

### Pitfall 4: No Screen Reader Support
**Problem:** ❌ `AutomationProperties.Name` missing; screen readers can't read control labels  
**Solution:** ✅ Add `AutomationProperties.Name` to all interactive controls

### Pitfall 5: Config Not Persisted
**Problem:** ❌ Settings change on screen but aren't saved; lost on restart  
**Solution:** ✅ Save config in ViewModel property setter; use JSON for portability

### Pitfall 6: Sub-Settings Always Enabled
**Problem:** ❌ User disables module, but sub-settings remain enabled  
**Solution:** ✅ Bind sub-setting `IsEnabled` to master toggle via computed property

### Pitfall 7: No Input Validation
**Problem:** ❌ Invalid hotkeys accepted; conflicts with system shortcuts  
**Solution:** ✅ Validate on key capture (not after submission); show error/warning inline

### Pitfall 8: Unclear Error Messages
**Problem:** ❌ Toast says "Error occurred"; user doesn't know what to do  
**Solution:** ✅ "This hotkey is reserved by Windows. Choose a different combination."

---

## Testing Checklist for Settings Panels

- [ ] Light mode: Text readable, colors correct
- [ ] Dark mode: Text readable, colors correct
- [ ] High contrast: Passes accessibility color contrast test (4.5:1 minimum)
- [ ] 100% DPI: Layout not cramped
- [ ] 150% DPI: Layout still readable
- [ ] 200% DPI: All controls reachable without scrolling
- [ ] Keyboard navigation: Tab through all controls, Space/Enter activates buttons
- [ ] Screen reader: All controls announce name and state
- [ ] Saving: Change setting, restart app, setting persists
- [ ] Validation: Invalid inputs rejected with clear message
- [ ] Edge cases: Rapid changes, toggles while recording, config file missing

---

## References

- WinUI 3 Controls: https://learn.microsoft.com/en-us/windows/apps/design/controls-and-patterns/
- PowerToys Modules: https://github.com/microsoft/PowerToys/tree/main/src/modules/
- Accessibility: https://learn.microsoft.com/en-us/windows/apps/design/accessibility/
- Data Binding: https://learn.microsoft.com/en-us/windows/apps/xaml-platform/x-bind-markup-extension
