# ClockTray Settings Panel — Features & Behavior Documentation

**Sprint 1 Design Document**  
**Prepared by:** Blain (UI Developer)  
**Date:** March 2026  
**Target Framework:** WinUI 3 / Windows App SDK

---

## Overview

This document specifies the detailed behavior of each feature in the ClockTray settings panel, including default values, validation rules, user expectations, and edge case handling. It serves as the specification for the WinUI 3 implementation and testing.

---

## Feature 1: Enable/Disable Toggle

### Purpose
Master control to activate or deactivate the entire ClockTray module. When disabled, the module consumes no resources and does not interfere with system shortcuts.

### Control
- **Type:** `ToggleSwitch` (WinUI 3)
- **Label:** "Enable ClockTray"
- **Help Text:** "Turn on to enable the Clock Tray module. When disabled, no hotkeys are registered and the clock state is unchanged."

### Default Value
- **On First Install:** OFF (disabled)
- **Reasoning:** Opt-in approach; user must explicitly enable to activate hotkey listening

### State Behavior

| State | Effect | Other Settings | Hotkey | Resources |
|---|---|---|---|---|
| **ON** | Module active, listening for hotkey | All enabled | Registered in OS | ~2-5 MB memory, hotkey listener active |
| **OFF** | Module inactive, clock state untouched | All disabled (greyed out) | Not registered | Minimal (~0.5 MB) |

### Interaction Flow
1. **Toggle ON:**
   - Registry method setting enabled
   - Hotkey picker enabled
   - Hotkey is registered with Windows
   - Module begins listening for key presses
   - Status section polls clock visibility
   - Toast notification: "Clock Tray enabled"

2. **Toggle OFF:**
   - All sub-settings disabled (visually)
   - Hotkey unregistered from Windows
   - Module stops listening
   - Status section stops polling
   - Current clock visibility state is preserved (no change)
   - Toast notification: "Clock Tray disabled"

### Validation Rules
- No validation needed (toggle is binary: on/off)

### Persistence
- State saved to config file immediately upon toggle
- Config file location: `%APPDATA%\PowerToys\modules\ClockTray\config.json`

### Edge Cases

**Rapid Toggle (Spam Toggle ON/OFF):**
- **Behavior:** Debounce toggle changes by 100ms
- **Result:** Only the final state is applied
- **Reason:** Prevents resource thrashing and hotkey re-registration overhead
- **Example:** User clicks toggle 5 times in 300ms → waits 100ms after last click → applies final state once

**Toggle OFF while Recording Hotkey:**
- **Behavior:** Cancel recording mode, preserve previous hotkey
- **Result:** Record button reverts to normal state, hotkey unchanged
- **Reason:** User may have toggled off by accident; don't lose custom hotkey

**Toggle OFF while Hotkey is Pressed:**
- **Behavior:** Current hotkey press is ignored
- **Result:** Toggle OFF completes, hotkey unregistered, next press has no effect
- **Reason:** No resource contention; clean shutdown

**Admin Privileges Required:**
- **On Toggle ON:** If not admin, show error: "Administrator privileges required to register hotkey. Run PowerToys as admin."
- **Workaround:** User must restart PowerToys with admin rights
- **Note:** PowerToys typically runs as admin; this is a safety check

---

## Feature 2: Registry Method Selection

### Purpose
Choose which registry modification technique to use when toggling the clock. Different Windows versions support different methods.

### Control
- **Type:** `ComboBox` (WinUI 3)
- **Label:** "Registry Method"
- **Help Text:** "Modern method (Win11 23H2+) is seamless. Legacy method (Win10) requires a brief Explorer restart."
- **InfoBar:** Explains the purpose and auto-detection capability

### Default Value
- **On First Install:** "Auto-detect (Recommended)"
- **Reasoning:** Simplifies setup for end users; one-click convenience

### Options & Behaviors

**Option 1: Auto-detect (Recommended)**
- **Registry Path:** Dynamically determined at runtime
- **OS Detection:** Queries Windows build number via `RtlGetVersion()` P/Invoke
- **Logic:**
  ```
  If Windows 11 AND build >= 23H2:
      Use Modern method
  Else if Windows 10 OR older Win11:
      Use Legacy method
  Else:
      Log error, disable module with user notification
  ```
- **Help Text:** "Auto-detect will use Modern on Win11 23H2+, Legacy on Win10."
- **Pros:** Future-proof; no manual reconfiguration needed
- **Cons:** Adds slight startup overhead (OS detection runs once per launch)

**Option 2: Modern (Win11 23H2+)**
- **Registry Path:** `HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\ShowSystrayDateTimeValueName`
- **Registry Value:** DWORD, 0 (hidden) or 1 (visible)
- **Broadcast Method:** `WM_SETTINGCHANGE` message to all top-level windows
- **Toggle Latency:** <100ms (near-instant, no restart needed)
- **Pros:** Seamless, no Explorer restart, immediate feedback
- **Cons:** Only works on Win11 23H2+; fails silently on older versions
- **Help Text:** "Seamless toggle. Recommended for Windows 11 23H2+."

**Option 3: Legacy (Win10)**
- **Registry Path:** `HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\HideClock`
- **Registry Value:** DWORD, 0 (show) or 1 (hide) [inverted logic!]
- **Post-Toggle Action:** Restart Explorer process (`explorer.exe`)
- **Toggle Latency:** 1–3 seconds (Explorer restart overhead)
- **Pros:** Works on Win10 and older Win11 versions
- **Cons:** Disruptive (taskbar flickers, open folder windows may close)
- **Help Text:** "May require Explorer restart. Use on Windows 10 or older Windows 11."

### Selection Behavior
1. **User Selects Option:**
   - Dropdown opens, showing three options
   - User clicks one
   - Dropdown closes, selection updates
   - Config is saved immediately

2. **Help Text Updates:**
   - Auto-detect: "This will use Modern on Win11 23H2+, Legacy on Win10."
   - Modern: "Seamless toggle. Recommended for Windows 11 23H2+."
   - Legacy: "May require Explorer restart. Use on Windows 10 or older Windows 11."

3. **Next Toggle Action:**
   - Uses the newly selected method
   - No restart or reload needed

### Validation Rules
- **Allowed:** Any of three options
- **Disabled State:** If master toggle (Feature 1) is OFF, ComboBox is disabled
- **No User Input Validation:** ComboBox only allows predefined options; invalid input impossible

### Persistence
- State saved to config immediately
- Loaded on next PowerToys launch

### Edge Cases

**User Selects Modern on Windows 10:**
- **Behavior:** ComboBox accepts selection, config saves
- **First Toggle:** Modern method runs, registry write succeeds
- **Problem:** `WM_SETTINGCHANGE` broadcast doesn't affect Win10 taskbar (designed for Win11)
- **Result:** Clock state doesn't visually update on taskbar
- **User Sees:** Clock toggle appears to fail (no change on screen)
- **Mitigation:** Show warning dialog: "Modern method may not work on Windows 10. Recommend switching to Auto-detect."

**User Selects Legacy on Windows 11 23H2:**
- **Behavior:** ComboBox accepts selection, config saves
- **First Toggle:** Legacy method runs (Explorer restart)
- **Observation:** Works, but disruptive (flickers taskbar, kills file explorer windows)
- **User Sees:** Legacy works, but with annoying side effects
- **Mitigation:** None (user explicitly chose legacy; respect their choice, but don't recommend)

**Auto-detect on Unknown Windows Version:**
- **Behavior:** OS detection runs, doesn't recognize build number
- **Fallback:** Default to Legacy method (safest for older systems)
- **User Notification:** Toast: "Unrecognized Windows version. Using Legacy method."
- **Result:** Module still functional, but user may see Explorer restart

**User Switches Methods Mid-Session:**
- **Behavior:** Config updates, takes effect on next toggle
- **Example:** User is on Win11 23H2, switches from Auto-detect to Legacy
- **First Toggle:** Uses Legacy method (Explorer restart)
- **Subsequent Toggles:** All use Legacy unless user changes selection again
- **No Restart Needed:** Method change doesn't require PowerToys restart

**Rapid Method Changes:**
- **Behavior:** ComboBox prevents multiple selections firing simultaneously
- **Debounce:** 100ms delay between allowed selection changes
- **Result:** Only final selection applied
- **Why:** Prevents config file corruption if user accidentally multi-clicks

---

## Feature 3: Activation Hotkey

### Purpose
Allow users to customize the keyboard shortcut that toggles the clock on/off. Reduces dependence on tray menu.

### Control
- **Type:** TextBox (read-only) + Button (Record)
- **Label:** "Activation Hotkey"
- **Help Text:** "Press any key combination to set. Hotkey must not conflict with system shortcuts (e.g., Win+Key). Default: Ctrl+Alt+T"

### Default Value
- **On First Install:** Ctrl+Alt+T
- **Reasoning:** Safe, unlikely to conflict with common shortcuts; memorable (T = Tray)

### Hotkey Input Method: Record Button

**Recording Mode Activation:**
1. User clicks "Record Hotkey" button
2. Button label changes to "Recording..." (visual feedback)
3. Button appearance changes (e.g., background color shift, slight glow)
4. TextBox placeholder changes to "Press any key combination now..."
5. Hotkey listener enters "record mode"
6. Next key combination pressed is captured

**Key Capture:**
- **What's Captured:** Modifier keys (Ctrl, Alt, Shift, Win) + Primary key (A–Z, 0–9, function keys, special keys)
- **Modifiers Required:** At least one modifier (Ctrl, Alt, Shift, or Win) must be held
- **Duration:** Record mode stays active until:
  - Valid hotkey is entered and validated (success → record mode exits)
  - Invalid hotkey is entered and rejected (error → record mode continues for retry)
  - User presses Escape (cancel → record mode exits, previous hotkey unchanged)
  - User toggles module OFF (cancel → record mode exits, previous hotkey unchanged)

**Example Valid Hotkeys:**
- Ctrl+Alt+T (current default)
- Shift+F12
- Win+Comma
- Ctrl+Shift+J
- Alt+PageUp

**Example Invalid Hotkeys:**
- T (no modifier key) → Error: "Hotkey must include at least one modifier key"
- Ctrl+Ctrl (duplicate) → Error: "Please press a key combination with a modifier and a single key"
- Win (modifier only, no primary key) → Error: "Please press a key combination with a modifier and a single key"

### Validation Rules

**Rule 1: Modifier Key Required**
- At least one of: Ctrl, Alt, Shift, Windows key must be held
- **Error Message:** "Hotkey must include at least one modifier key (Ctrl, Alt, Shift, or Windows)."
- **Example Failure:** Just pressing "Q" without modifiers

**Rule 2: Single Primary Key**
- Exactly one non-modifier key must be pressed
- **Error Message:** "Please press a key combination with a modifier and a single key."
- **Example Failure:** Ctrl+Alt+T+D (too many keys); Ctrl+Shift (no primary key)

**Rule 3: No System Reserved Shortcuts**
- Hotkey must not conflict with Windows system shortcuts
- **Reserved Shortcuts Include:**
  - Win+E (Explorer)
  - Win+D (Show Desktop)
  - Win+V (Clipboard History)
  - Win+Shift+S (Screenshot)
  - Ctrl+Esc (Start Menu)
  - Alt+Tab (Switch Apps)
  - Many others (full list maintained in code)
- **Error Message:** "This hotkey is reserved by Windows. Choose a different combination."
- **Severity:** Warning (user can still force if they want, but not recommended)

**Rule 4: No PowerToys Module Conflicts**
- Hotkey must not conflict with other PowerToys modules' hotkeys
- **Example Conflict:** Ctrl+Alt+T already used by ColorPicker
- **Error Message:** "This hotkey is already used by PowerToys Color Picker. Choose a different combination."
- **Severity:** Error (prevent conflict; user must choose different hotkey)

**Rule 5: No Third-Party App Conflicts** (Optional, if feasible)
- Optionally warn if hotkey is commonly used by other apps
- **Example:** Ctrl+Alt+S used by many screenshot tools
- **Error Message (Warning):** "This hotkey is commonly used by other applications. You may experience conflicts."
- **Severity:** Informational (allow user to proceed if they want)

### Validation Behavior

**On Valid Hotkey:**
1. TextBox updates to display new hotkey (e.g., "Ctrl+Shift+J")
2. Status message: "✅ Hotkey set successfully."
3. Status message color: Green
4. Button reverts to "Record Hotkey" label
5. Record mode exits
6. Config is saved immediately
7. New hotkey is registered with Windows
8. Toast notification: "Hotkey set to Ctrl+Shift+J"

**On Invalid Hotkey:**
1. TextBox remains unchanged (displays previous hotkey)
2. Status message displays error (red text)
3. Button remains in "Recording..." state
4. Record mode remains active (user can retry)
5. After 5 seconds of inactivity, button reverts to "Record Hotkey" and record mode exits (timeout)

**On Validation Warning (Conflict with Reserved/Common Hotkey):**
1. TextBox displays the attempted hotkey (preview)
2. Status message: "⚠️ This hotkey is reserved by Windows. Are you sure?"
3. Status message color: Orange/amber
4. Record mode remains active (user can press Escape to cancel, or press another combo)
5. Option: Add secondary button "Use Anyway" to force override (expert users)

### Persistence
- Hotkey is saved to config immediately after successful validation
- On PowerToys launch, hotkey is loaded from config and registered with Windows

### Accessibility
- **Screen Readers:** "Activation Hotkey, read-only textbox, Ctrl+Alt+T, button Record Hotkey"
- **Keyboard Navigation:** Tab to Record button, Space/Enter to activate recording
- **Recording Feedback:** Button label and status text provide audio cues via screen reader
- **Escape Key:** Can press Escape to cancel recording mode (accessible alternative to timeout)

### Edge Cases

**User Presses Same Hotkey Again:**
- **Scenario:** Current hotkey is Ctrl+Alt+T, user records Ctrl+Alt+T again
- **Behavior:** Validation passes (no conflict with self)
- **Result:** Hotkey "updates" to same value (harmless no-op)
- **Status:** "✅ Hotkey set successfully." (same as normal case)

**User Records Hotkey While Hotkey is Being Pressed Elsewhere:**
- **Scenario:** User has Ctrl+Alt+T bound to another app, clicks Record, that app's hotkey fires
- **Behavior:** Record mode captures the keys from the global listener (correct keys detected)
- **Result:** Hotkey records as intended
- **Note:** PowerToys runs at higher hook priority; should capture before other apps

**Rapid Record Button Clicks:**
- **Behavior:** Debounce; only first click starts recording
- **Subsequent Clicks:** Ignored while record mode active
- **Result:** Single record session per button activation
- **Exit:** User must press valid/invalid hotkey or Escape to exit

**Record Button Disabled (Module OFF):**
- **Appearance:** Button is greyed out, cursor shows "not allowed"
- **Click Behavior:** Click has no effect
- **Tooltip:** "Enable the module to record a hotkey"

**Timeout During Recording (5 Seconds of Inactivity):**
- **Behavior:** If user starts recording but doesn't press any key for 5 seconds
- **Action:** Automatically exit record mode
- **Button:** Reverts to "Record Hotkey"
- **Status:** Clears (no error message; just timeout)
- **Reason:** Prevent UI stuckness; allow user to start over

**Power User: Direct Config Edit**
- **Scenario:** User manually edits `config.json` and changes hotkey directly
- **Behavior:** On next PowerToys launch, hotkey is loaded from config
- **Result:** New hotkey is registered, TextBox displays new hotkey
- **No Validation:** Direct edits bypass validation (trust power user)
- **Risk:** Invalid hotkey in config → registration fails silently, module disables hotkey listening

---

## Feature 4: Status Display (Expandable)

### Purpose
Show real-time system state information to help users understand the current configuration and debug issues.

### Control
- **Type:** `Expander` (WinUI 3)
- **Label:** "System State"
- **Default State:** Collapsed (hidden by default)
- **Expand/Collapse Behavior:** Click on header to toggle

### Status Items

**Item 1: Clock Visibility**
- **Label:** "Clock Visibility:"
- **Display:** "Visible" (green) or "Hidden" (red)
- **Data Source:** Queries registry in real-time (not cached)
- **Refresh Rate:** Every 2 seconds when expander is open (polling)
- **Purpose:** Shows actual taskbar clock state
- **Example:** If user manually changed registry outside of ClockTray, this reflects it

**Item 2: Current Method**
- **Label:** "Current Method:"
- **Display:** "Auto-detect", "Modern", or "Legacy" (reflects user's selection from Feature 2)
- **Data Source:** Loaded from config
- **Refresh Rate:** Manual (updates only when user changes selection)
- **Purpose:** Confirms which method is in use

**Item 3: OS Build**
- **Label:** "OS Build:"
- **Display:** "Windows 11 Build 23H2" or "Windows 10 Build 20H2" (actual version/build)
- **Data Source:** Queried once at launch via Windows API
- **Refresh Rate:** Static (doesn't change during session)
- **Purpose:** Helps explain why Auto-detect chose a certain method

### Expand Behavior
1. **User Clicks "System State" Header or Arrow:**
   - Arrow rotates 90° (▼ if expanded, ▶ if collapsed)
   - Content expands (slide animation, 0.3s)
   - Status polling starts (refreshes every 2 seconds)

2. **Content Visible:**
   - All three items display
   - Clock Visibility updates dynamically
   - Other items remain static

3. **User Clicks Again (Collapse):**
   - Content collapses (slide animation, 0.3s)
   - Arrow rotates back (▶)
   - Polling stops (save CPU resources)

### Performance Considerations
- **Polling Only When Expanded:** Registry queries are expensive; avoid continuous polling if expander is closed
- **Polling Interval:** 2 seconds (balance between freshness and CPU usage)
- **Caching:** Clock visibility state is cached; only update on polling interval change
- **No Blocking:** Polling runs on background thread; UI remains responsive

### Accessibility
- **Screen Readers:** "System State, expandable button, not expanded" (collapsed) or "expanded" (expanded)
- **Keyboard Navigation:** Tab to reach, Space/Enter to expand/collapse
- **Status Announcement:** When expanded, screen reader announces all three status items

### Edge Cases

**Expander Expanded, User Toggles Module OFF:**
- **Behavior:** Status items remain visible
- **Clock Visibility:** Shows current state (doesn't change; module doesn't affect clock)
- **Current Method:** Still shows the configured method (but it's not in use since module is off)
- **Purpose:** Help user understand state even when module is off

**User Manually Changes Registry (Outside of ClockTray):**
- **Scenario:** User edits registry directly or another app changes it
- **Behavior:** Next time Clock Visibility is polled (2 seconds), it reflects the change
- **Example:** User hides clock via Settings app; Clock Visibility updates to "Hidden" in status
- **Purpose:** Real-time feedback that ClockTray is aware of external changes

**OS Build Changes (Rare, e.g., Windows Update):**
- **Behavior:** OS Build remains static during session (set at launch)
- **After PowerToys Restart:** OS Build updates to new version
- **Why:** Updating OS Build mid-session risks crashes; safer to read once at startup

**Status Display Shows Different State Than Actual Toggle:**
- **Scenario:** Status shows "Visible", but user's taskbar clock is actually hidden
- **Causes:**
  - Registry changed externally
  - Different user account (HKCUchecks only current user)
  - Explorer.exe crashed and restarted (Modern method may lose state)
- **User Action:** Check registry manually or restart Explorer to resync
- **Error Message:** Could add: "❓ Status mismatch detected. Try toggling again or restart Explorer."

---

## Feature 5: Reset to Defaults

### Purpose
Restore all settings to factory defaults with a single click. Useful if user wants to start fresh or troubleshoot.

### Control
- **Type:** Button
- **Label:** "Reset to Defaults"
- **Style:** Subtle (less prominent than primary actions)
- **Help Text:** "This will reset the hotkey to Ctrl+Alt+T and set the registry method to Auto-detect."

### Defaults Being Reset
| Setting | Default Value |
|---|---|
| Enable/Disable | OFF (module disabled) |
| Registry Method | "Auto-detect (Recommended)" |
| Activation Hotkey | Ctrl+Alt+T |

### Reset Behavior

**Interaction Flow:**
1. **User Clicks "Reset to Defaults" Button**
2. **Confirmation Dialog (Recommended):**
   ```
   ┌───────────────────────────────────────────┐
   │ Reset Settings to Defaults?               │
   │                                           │
   │ This will reset:                          │
   │ • Hotkey: Ctrl+Alt+T                      │
   │ • Method: Auto-detect                     │
   │ • Module will be disabled                 │
   │                                           │
   │ [Cancel]  [Reset]                         │
   └───────────────────────────────────────────┘
   ```
3. **User Confirms "Reset":**
   - Enable/Disable toggle switches to OFF
   - Registry Method ComboBox switches to "Auto-detect"
   - Activation Hotkey TextBox updates to "Ctrl+Alt+T"
   - Config file is overwritten with default values
   - Module is disabled (no hotkey listening)
   - Toast notification: "✅ Settings reset to defaults."
4. **User Cancels:**
   - Dialog closes
   - No changes made
   - Settings remain as-is

### Persistence
- All defaults are written to config file immediately
- Config changes take effect immediately (no restart needed)

### Edge Cases

**Reset While Recording Hotkey:**
- **Behavior:** Recording mode is cancelled
- **Result:** Hotkey reverts to default, record button returns to normal state
- **Timing:** If user clicks Reset while holding down a key in record mode, that key press is ignored

**Reset While Module is ON:**
- **Behavior:** Module is disabled as part of reset
- **Effect:** Hotkey listener stops, Explorer restart doesn't occur (no toggle happens)
- **Result:** All settings reset, module left in OFF state
- **User Must:** Re-enable module manually if desired

**Reset While Configuration File is Locked:**
- **Scenario:** Another PowerToys instance or editor has config.json open
- **Behavior:** Config write operation fails
- **Error Message:** "⚠️ Failed to save settings. Check that PowerToys is not running multiple instances."
- **User Action:** Close other instances and try again

**Multiple Resets in Succession:**
- **Behavior:** Each reset completely overwrites config
- **Result:** No data corruption; each reset identical to previous
- **Timing:** Debounce prevents accidental double-resets (<100ms)

**Undo After Reset:**
- **Behavior:** No undo button
- **Alternative:** User can manually edit config file or use previous hotkey to toggle (if they remember it)
- **Future:** Could implement JSON-based undo stack, but out of scope for Sprint 1

---

## Global Behaviors & Constraints

### Configuration Persistence
- **File Location:** `%APPDATA%\PowerToys\modules\ClockTray\config.json`
- **Format:** JSON (matches PowerToys standard)
- **Schema:**
  ```json
  {
    "enabled": false,
    "method": "Auto",
    "hotkey": "Ctrl+Alt+T"
  }
  ```
- **Fallback:** If config file corrupted or missing, load defaults
- **Backup:** On every successful change, create backup of previous config

### Module State Management
- **State Persistence:** Module state (ON/OFF) saved to config, persists across restarts
- **Hotkey Registration:** Only active when module is ON
- **Clock State:** Never changed by module state (toggling module OFF doesn't affect clock)

### Thread Safety
- **Config Updates:** Serialize all writes to config file (prevent concurrent access)
- **Registry Writes:** Use Win32 API locks to prevent race conditions
- **Hotkey Registration:** Queue hotkey changes (avoid simultaneous registrations)

### Error Handling
- **Registry Write Fails:** "⚠️ Failed to update clock visibility. Check administrator permissions."
- **Hotkey Registration Fails:** "⚠️ Hotkey registration failed. Ensure the hotkey is not in use by another application."
- **Config Read/Write Fails:** "⚠️ Configuration error. Using default settings."
- **OS Detection Fails:** "⚠️ Unable to detect Windows version. Using Legacy method as fallback."

### User Notifications
- **Success (Toast):** "✅ Clock Tray enabled", "✅ Hotkey set to Ctrl+Shift+J"
- **Error (Toast):** "⚠️ Failed to update registry"
- **Info (Toast):** "ℹ️ Using Modern method (Win11 23H2+)"

### Logging
- **Log Location:** `%APPDATA%\PowerToys\logs\ClockTray.log`
- **Log Level:** Info (success/toggle), Warning (conflicts, fallbacks), Error (failures)
- **Example Entries:**
  ```
  [2026-03-10 14:23:45] INFO: Module enabled
  [2026-03-10 14:23:46] INFO: Hotkey registered: Ctrl+Alt+T
  [2026-03-10 14:24:12] INFO: Clock visibility toggled to Hidden (Modern method)
  [2026-03-10 14:25:30] ERROR: Registry write failed (permission denied)
  ```

---

## Win10 vs Win11 UI Differences

| Aspect | Windows 10 | Windows 11 |
|---|---|---|
| **Theme Colors** | Supports light/dark (different palette) | Supports light/dark (WinUI 3 standard) |
| **Control Spacing** | Slightly larger margins | Refined spacing |
| **Font Rendering** | Segoe UI (12pt) | Segoe UI variable (14pt) |
| **Fluent Design** | Partial (Acrylic, transparency) | Full (Mica, light effects) |
| **Expander Animation** | Supported (may be jerky) | Smooth animation |
| **High Contrast Mode** | Supported | Supported |
| **DPI Scaling** | 100%, 125%, 150%, 200% | 100%, 125%, 150%, 175%, 200%, 225%, 250% |

**Implementation Strategy:**
- Use `ThemeResource` bindings (auto-adapt to OS theme)
- Test on both Win10 20H2 and Win11 21H2, 22H2, 23H2
- Fallback fonts/colors for older Win10 builds if needed

---

## Success Criteria (Testing Checklist)

- [ ] Enable/Disable toggle: Correctly enables/disables module and sub-settings
- [ ] Registry method selection: ComboBox shows all three options, dynamic help text updates
- [ ] Hotkey recording: Valid hotkeys accepted, invalid hotkeys rejected with clear errors
- [ ] Hotkey validation: System reserved hotkeys warn user, conflicts prevent selection
- [ ] Status display: Clock visibility updates in real-time, expander collapses/expands smoothly
- [ ] Reset button: Clicking reset restores all defaults without errors
- [ ] Config persistence: Settings survive PowerToys restart
- [ ] Light/Dark theme: UI correctly inverts in both themes
- [ ] Accessibility: Keyboard navigation works, screen reader announces all elements
- [ ] Error handling: All errors display clear messages, module recovers gracefully
- [ ] Performance: UI remains responsive during registry operations and polling
- [ ] Win10/Win11: UI renders correctly on both OS versions

---

## References

- **WinUI 3 Controls:** https://learn.microsoft.com/en-us/windows/apps/design/controls-and-patterns/
- **PowerToys Architecture:** https://github.com/microsoft/PowerToys/tree/main/src/modules/
- **Windows Registry API:** https://learn.microsoft.com/en-us/windows/win32/api/winreg/
- **Hotkey Registration (Win32):** https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey
- **Accessibility Guidelines:** https://learn.microsoft.com/en-us/windows/apps/design/accessibility/
