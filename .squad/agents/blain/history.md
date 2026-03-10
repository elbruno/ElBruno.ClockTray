# Blain — History

## Project Context
- **Project:** ClockTray — Windows system tray app to show/hide date and time in the taskbar
- **Stack:** C# / .NET / WPF or WinUI, Win32 interop
- **User:** Bruno Capuano
- **Phase:** Phase 2 PowerToys contribution (WinUI 3 settings panel design)

## Work Completed

### Sprint 1: UI Design & Documentation (March 10, 2026)

**Deliverables Created:**
1. **ClockTray-SettingsPanel-XAML.md** — Complete XAML structure documentation
   - Root container layout (MaxWidth 1200px, padding alignment)
   - Six major sections: Header, Master Toggle, Registry Method, Hotkey Picker, Status Display, Reset Button
   - Control types: ToggleSwitch, ComboBox, TextBox (read-only), Button, TextBlock, Expander
   - Two-column responsive grid layout with breakpoints for small screens
   - Theme resource bindings for light/dark mode and DPI scaling
   - Accessibility checklist: ARIA labels, keyboard navigation, high contrast support

2. **ClockTray-SettingsPanel-Mockup.md** — Visual description & UX flows
   - ASCII mockups of each section for visual reference
   - Detailed interaction flows: toggle behavior, method selection updates, hotkey recording
   - Status display with expandable/collapsible behavior
   - Animation specs: hover effects, state transitions, expander expansion (0.3s)
   - Theme support: light/dark mode with `ThemeResource` bindings
   - PowerToys design patterns observed: InfoBar for context, Expander for advanced options
   - Error states and edge cases documented

3. **ClockTray-SettingsPanel-Features.md** — Comprehensive behavior specification
   - Feature 1 (Enable/Disable Toggle): Default OFF (opt-in), state persistence, rapid toggle debounce
   - Feature 2 (Registry Method): Three options (Auto, Modern, Legacy) with intelligent auto-detection
   - Feature 3 (Hotkey Picker): Record button UX, validation rules (modifier required, no system conflicts)
   - Feature 4 (Status Display): Real-time polling, expandable panel, performance optimization
   - Feature 5 (Reset Button): Confirmation dialog, default value restoration
   - Win10 vs Win11 differences: Color palette, control spacing, DPI scaling support
   - Testing checklist: 14-point validation covering all features

## Learnings

### PowerToys Design Patterns Observed
1. **InfoBar for Contextual Information:** Registry method section uses InfoBar to explain choices without cluttering main UI
2. **Expander for Power-User Features:** Status section is collapsed by default; advanced users can expand for system diagnostics
3. **ComboBox for Discrete Choices:** Registry method uses dropdown instead of radio buttons (cleaner layout, easier to scan)
4. **Theme Resource Bindings:** All colors use `ThemeResource` (not hardcoded RGB) for automatic light/dark mode support
5. **ToggleSwitch over Checkbox:** Master toggle uses ToggleSwitch for immediate visual feedback (on/off is clearer)
6. **Toast Notifications:** Success/error messages use toast (non-blocking) instead of modal dialogs (less disruptive)
7. **Keyboard-First Navigation:** All controls reachable via Tab; Enter/Space activates; Escape cancels (standard Windows convention)
8. **Read-Only TextBox for Display:** Hotkey display is non-editable (prevents user confusion about how to change it)

### UI Design Decisions & Rationale
1. **Master Toggle First:** Placing Enable/Disable at the top makes it the "main" setting; sub-settings visually dependent (greyed out when off)
2. **Dynamic Help Text:** Method description updates based on selection (e.g., "Seamless on Win11 23H2+" vs "May require restart"), providing context-aware guidance
3. **Record Button for Hotkey Entry:** Avoids plain TextBox input (prevents typos); familiar pattern to power users (like OBS, game launchers)
4. **Two-Column Grid Layout:** Control on right (300px), label/help on left; responsive single-column on small screens (600px breakpoint)
5. **Validation Feedback Inline:** Status message below hotkey field (no modal dialogs); red text for errors, green for success, orange for warnings
6. **Reset Button with Confirmation:** Prevents accidental data loss; confirmation dialog lists what will be reset
7. **Status Expander (Collapsed by Default):** Reduces visual clutter for typical users; power users can expand to see OS build, current method, clock visibility

### PowerToys Architecture Insights
1. **Settings Panel Location:** PowerToys settings UI lives in `src/settings-ui/`; ClockTray settings page will be injected into main settings app
2. **Config Format:** PowerToys uses JSON configs stored in `%APPDATA%\PowerToys\modules\{ModuleName}\config.json`
3. **Module Lifecycle:** `PowertoyModuleIface` defines `enable()`, `disable()`, `send_config_json()`, `get_config()` methods
4. **Message Passing:** Settings panel talks to module via JSON (not direct C++ calls); decouples UI from logic
5. **WinUI 3 Standard:** All PowerToys modules use WinUI 3; design system provides consistent controls, colors, typography

### WinUI 3 Implementation Specifics
1. **Data Binding:** Use `x:Bind` (strongly typed, better performance) over `{Binding}` (loosely typed)
2. **MVVM Pattern:** ViewModel exposes properties (`IsEnabled`, `SelectedMethod`, `HotkeyDisplay`); UI binds to them
3. **Two-Way Binding for Toggles:** `IsOn="{x:Bind ViewModel.IsEnabled, Mode=TwoWay}"` keeps UI and config in sync
4. **TextBlock Styling:** Use predefined styles (`HeadingTextBlockStyle`, `BaseTextBlockStyle`, `CaptionTextBlockStyle`) for consistency
5. **Resource Binding:** All colors use `{ThemeResource}` (not hardcoded); DPI scaling handled automatically
6. **Accessibility:** `AutomationProperties.Name` required for all interactive controls (screen reader announcements)

### OS Compatibility Considerations
1. **Win10 vs Win11 UI:** Both supported; WinUI 3 handles rendering differences automatically
2. **DPI Scaling:** Test at 100%, 125%, 150%, 200% (Win10); 175%, 225%, 250% (Win11)
3. **High Contrast Mode:** Ensure colors remain readable; test with `Settings > Ease of Access > High Contrast`
4. **Theme Resources:** Use `ThemeResource` bindings to respect user's light/dark mode and high contrast settings
5. **Registry Path Differences:**
   - Modern (Win11 23H2+): `ShowSystrayDateTimeValueName` + `WM_SETTINGCHANGE` broadcast (seamless)
   - Legacy (Win10): `HideClock` + Explorer restart (disruptive but works)
   - Auto-detect: Runtime OS check, chooses appropriate method

### Validation & Error Handling
1. **Hotkey Validation Rules:**
   - Modifier key required (Ctrl, Alt, Shift, or Win)
   - Single primary key only
   - Must not conflict with Windows reserved shortcuts (Win+E, Win+D, etc.)
   - Must not conflict with other PowerToys modules
2. **Registry Write Failures:** Handle gracefully (show error toast, don't crash module)
3. **Hotkey Registration Failures:** Fallback to legacy method if modern method unavailable
4. **Config File Errors:** Load defaults if config corrupted; create backup before writing

### Performance Optimization
1. **Polling Only When Expanded:** Status display polls registry every 2 seconds, but only if Expander is open (save CPU)
2. **Debouncing:** Rapid toggles, combobox selections, and hotkey recording all debounced (100ms) to prevent thrashing
3. **Lazy Loading:** Status data (OS build, clock visibility) queried at display time, not cached
4. **Threading:** Registry polling runs on background thread; UI remains responsive

### Future Considerations
1. **Hotkey Conflict Detection:** Could add real-time checking as user types in record mode
2. **Quick Toggle Button:** "Test Now" button to verify toggle works without leaving settings
3. **Advanced Registry Paths:** Expert mode to customize registry paths (out of scope for Sprint 1)
4. **Telemetry:** Track which method users prefer, hotkey retention rate (for analytics)
5. **Undo Stack:** JSON-based undo for reset action (nice-to-have, not critical)

## Team Coordination Notes (Updated Sprint 1)
- **Coordination with Dutch (Architecture):** Tech spec complete; design aligns perfectly with four-class architecture. No conflicts in decision making.
- **Coordination with Molly (Documentation):** Documentation framework complete (overview, guide outline, PowerToys context); UI design will inform settings reference section in user guide
- **Coordination with Mac (Backend):** Module C++ implementation will expose `PowertoyModuleIface` methods; UI sends config JSON via `send_config_json()`. XAML structure is production-ready; can begin coding in Sprint 3
- **Coordination with Dillon (QA):** Test matrix should cover Win10 20H2, Win11 21H2/22H2/23H2, 100%/150%/200% DPI, light/dark mode. Behavior specs provide comprehensive test foundation.
