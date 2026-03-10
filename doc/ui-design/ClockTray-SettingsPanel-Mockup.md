# ClockTray Settings Panel — UI Mockup Description

**Sprint 1 Design Document**  
**Prepared by:** Blain (UI Developer)  
**Date:** March 2026  
**Target Framework:** WinUI 3 / Windows App SDK

---

## Overview

This document provides a visual description of the ClockTray settings panel layout, control placement, user interaction flows, and alignment with PowerToys design conventions. Use this as a reference guide for implementing the XAML layout and understanding the intended user experience.

---

## Visual Layout Description

### Page Structure (High-level)

The settings panel consists of six vertical sections with consistent spacing (24px between major sections):

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                 │
│  📋 HEADER SECTION                                              │
│  ├─ "Clock Tray" (heading)                                     │
│  └─ "Toggle the Windows taskbar clock..." (description)        │
│                                                                 │
│  ─────────────────────────────────────────────────────────────│
│                                                                 │
│  ⚙️  MASTER TOGGLE SECTION                                      │
│  ├─ "Enable ClockTray" [Label + Help Text] | [Toggle Switch] ⭕ │
│                                                                 │
│  ─────────────────────────────────────────────────────────────│
│                                                                 │
│  🔧 REGISTRY METHOD SECTION                                    │
│  ├─ [ℹ️  InfoBar: Registry Method Selection...]               │
│  ├─ "Registry Method" [Label + Help Text] | [ComboBox ▾ ]    │
│  └─ Dynamic help text (updates with selection)               │
│                                                                 │
│  ─────────────────────────────────────────────────────────────│
│                                                                 │
│  ⌨️  HOTKEY SECTION                                             │
│  ├─ "Activation Hotkey" [Label + Help Text]                   │
│  ├─ [TextBox: Ctrl+Alt+T (read-only)]  [Button: Record...]   │
│  └─ Status message (validation feedback)                     │
│                                                                 │
│  ─────────────────────────────────────────────────────────────│
│                                                                 │
│  📊 STATUS SECTION (Expandable)                               │
│  ├─ ▶ "System State" (collapsed by default)                   │
│  └─ (Expandable content not shown)                            │
│                                                                 │
│  ─────────────────────────────────────────────────────────────│
│                                                                 │
│  🔄 RESET SECTION                                              │
│  ├─ [Button: Reset to Defaults]                               │
│  └─ Help text explaining the action                           │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Detailed Section Views

---

## Section 1: Header

**Visual:**
```
╔════════════════════════════════════════════════════════════════╗
║                                                                ║
║  Clock Tray                                          (Heading) ║
║                                                                ║
║  Toggle the Windows taskbar clock and time display with a     ║
║  single hotkey or menu click.                    (Body Text)   ║
║                                                                ║
╚════════════════════════════════════════════════════════════════╝
```

**Description:**
- **Title:** "Clock Tray" in large, bold font (HeadingTextBlockStyle)
- **Subtitle:** Explains the utility's core functionality in secondary color text
- **Purpose:** Orients the user and communicates the module's value proposition
- **Spacing:** 8px vertical gap between title and description

**Design Notes:**
- Uses theme colors (automatically inverts in dark mode)
- Single column; spans full width
- No interactive elements in this section

---

## Section 2: Master Toggle (Enable/Disable)

**Visual:**
```
┌────────────────────────────────────────────────────────────────┐
│                                                                │
│ Enable ClockTray                        [⭕ Toggle Switch OFF] │
│                                                                │
│ Turn on to enable the Clock Tray module. When disabled, no    │
│ hotkeys are registered and the clock state is unchanged.      │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

**Layout:**
- **Left (70%):** Label + help text
- **Right (30%):** ToggleSwitch control, vertically centered

**Interaction:**
1. **Initial State:** Toggle defaults to OFF if module is disabled; ON if enabled
2. **User Action:** Click toggle or press Space/Enter when focused
3. **Visual Feedback:** Toggle slides to opposite position immediately
4. **Effect:**
   - Toggle ON → Module activates, hotkey listener starts, sub-settings enabled
   - Toggle OFF → Module deactivates, hotkey listener stops, sub-settings disabled (greyed out)

**Accessibility:**
- Screen readers announce: "Enable Clock Tray module, toggle button, off/on"
- Keyboard: Tab to reach, Space/Enter to toggle, Tab to continue

**Design Principles:**
- Prominent placement (first interactive control)
- Clear cause-effect relationship (toggle → module on/off)
- Help text clarifies what "disabled" means (no hotkey interference, clock state preserved)

---

## Section 3: Registry Method Selection

**Visual (Collapsed/Closed InfoBar):**
```
┌────────────────────────────────────────────────────────────────┐
│                                                                │
│ ℹ️  Registry Method Selection              [Close button ✕]   │
│ Choose the method used to toggle the taskbar clock. PowerToys │
│ auto-detects your OS version and selects the appropriate      │
│ method automatically.                                          │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

**Visual (Main Control):**
```
┌────────────────────────────────────────────────────────────────┐
│                                                                │
│ Registry Method                        [ComboBox ▼]           │
│                                     Auto-detect (Recommended)  │
│                                                                │
│ Modern method (Win11 23H2+) is seamless. Legacy method (Win10)│
│ requires a brief Explorer restart.                            │
│                                                                │
│ Current selection: Auto-detect                                │
│ This will use Modern on Win11 23H2+, Legacy on Win10.        │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

**Layout:**
- **Left (70%):** Label + help text
- **Right (30%):** ComboBox dropdown (width ~300px)

**ComboBox Options:**
1. **Auto-detect (Recommended)** [Default]
   - Smart: Detects OS, chooses Modern (Win11 23H2+) or Legacy (Win10)
   - Status: "This will use Modern on Win11 23H2+, Legacy on Win10."

2. **Modern (Win11 23H2+)**
   - Uses modern registry path: `ShowSystrayDateTimeValueName`
   - Seamless toggle (no Explorer restart needed)
   - Status: "Seamless toggle. Recommended for Win11 23H2+."

3. **Legacy (Win10)**
   - Uses legacy registry path: `HideClock`
   - Requires brief Explorer restart
   - Status: "May require Explorer restart. Use on Win10 or older Win11."

**Interaction:**
1. **Initial State:** ComboBox displays currently selected method (default: "Auto-detect")
2. **User Action:** Click ComboBox to open dropdown
3. **Selection:** User clicks desired option
4. **Visual Feedback:**
   - Selected option highlights
   - Help text below updates to describe the chosen method
   - ComboBox closes
4. **Effect:** Config is updated; next toggle uses the selected method

**Disabled State:**
- If master toggle (Section 2) is OFF, ComboBox is greyed out / disabled
- Tooltip shows: "Enable the module to customize this setting"

**Accessibility:**
- Screen readers announce: "Select registry method, combobox, currently Auto-detect"
- Keyboard: Tab to reach, Arrow keys to navigate options, Enter to select, Escape to cancel

**Design Notes:**
- InfoBar provides extra context without cluttering the UI
- "Recommended" tag guides new users toward Auto-detect
- Dynamic help text reduces confusion about method differences

---

## Section 4: Hotkey Picker

**Visual:**
```
┌────────────────────────────────────────────────────────────────┐
│                                                                │
│ Activation Hotkey                                              │
│                                                                │
│ Press any key combination to set. Hotkey must not conflict    │
│ with system shortcuts (e.g., Win+Key). Default: Ctrl+Alt+T   │
│                                                                │
│ ┌──────────────────────────────────────────────────────────┐  │
│ │ Ctrl+Alt+T                          [TextBox - Read-Only] │  │
│ └──────────────────────────────────────────────────────────┘  │
│                                                                │
│ [Button: Record Hotkey]                                        │
│                                                                │
│ ✅ Hotkey set successfully.                                   │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

**Layout:**
- **Left (70%):** Label + help text
- **Right (30%):** TextBox (read-only) + Record button stacked vertically

**TextBox (Read-only Display):**
- Shows current hotkey in human-readable format: "Ctrl+Alt+T"
- Grey background (indicates read-only)
- Placeholder text: "Press keys..." (shown before hotkey is set)
- Cannot be edited directly; users must click Record button

**Record Button:**
- Label: "Record Hotkey" (or "Recording..." when active)
- Enabled: Only when master toggle is ON
- Disabled: Greyed out if module is disabled

**Interaction Flow:**

**Normal Case (Valid Hotkey):**
1. User clicks "Record Hotkey" button
2. Button label changes to "Recording..." (visual feedback)
3. User presses a key combination (e.g., Ctrl+Shift+C)
4. Hotkey is captured and validated:
   - ✅ Not conflicting with Windows system shortcuts
   - ✅ Not conflicting with other PowerToys modules
   - ✅ Valid (at least one modifier key required)
5. If valid:
   - TextBox updates: "Ctrl+Shift+C"
   - Status message: "✅ Hotkey set successfully."
   - Config is saved
   - Button label reverts to "Record Hotkey"
   - Recording mode exits

**Error Case (Invalid Hotkey):**
1. User clicks "Record Hotkey"
2. Button label changes to "Recording..."
3. User presses invalid combo (e.g., just "Q" without modifier):
   - ❌ Validation fails (single key not allowed)
4. Status message: "❌ Hotkey must include at least one modifier key (Ctrl, Alt, Shift, or Windows)."
5. TextBox remains unchanged
6. Button remains in "Recording..." state (allow retry)
7. User presses another combo or presses Escape to cancel

**Conflict Case (System Reserved):**
1. User presses Win+E (reserved for Explorer):
2. Validation warns: "⚠️ This hotkey is reserved by Windows. Choose a different combination."
3. Recording continues (allow retry)

**Disabled State:**
- If master toggle is OFF:
  - Button is disabled (greyed out)
  - TextBox displays current/default hotkey (read-only, disabled appearance)
  - Status message hidden

**Status Message Styling:**
- ✅ Success (green): "Hotkey set successfully."
- ❌ Error (red): "Hotkey must include at least one modifier key..."
- ⚠️ Warning (orange): "This hotkey is reserved by Windows..."

**Accessibility:**
- Screen readers announce: "Activation Hotkey, read-only textbox, Ctrl+Alt+T, button Record Hotkey"
- Keyboard: Tab to button, Space/Enter to start recording, then normal keyboard input captured
- Focus management: After successful recording, focus returns to Record button

**Design Notes:**
- Read-only TextBox is clearer than disabled TextBox (user knows it's for display, not editing)
- Record button pattern is familiar to power users (common in rebinding/macro software)
- Status messages provide immediate feedback without modal dialogs
- Validation prevents user frustration from silent failures

---

## Section 5: Status Display (Expandable)

**Visual (Collapsed):**
```
┌────────────────────────────────────────────────────────────────┐
│                                                                │
│ ▶ System State                                                 │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

**Visual (Expanded):**
```
┌────────────────────────────────────────────────────────────────┐
│                                                                │
│ ▼ System State                                                 │
│                                                                │
│ Clock Visibility:       Visible (or Hidden)        (Green)    │
│                                                                │
│ Current Method:         Modern                                 │
│                                                                │
│ OS Build:               Windows 11 Build 23H2                 │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

**Sections:**
1. **Clock Visibility:** Displays "Visible" (green) or "Hidden" (red)
   - Refreshes every 2-5 seconds (polling the actual taskbar state)
   - Non-interactive; informational only

2. **Current Method:** Shows which registry method is active
   - Example: "Modern" or "Legacy"
   - Updates if user switches methods in Registry Method section

3. **OS Build:** Shows Windows version and build number
   - Example: "Windows 11 Build 23H2" or "Windows 10 Build 20H2"
   - Helps users understand why Auto-detect chose a certain method

**Interaction:**
1. **Initial State:** Expander is collapsed (▶ arrow points right)
2. **User Action:** Click on "System State" header or arrow
3. **Visual Feedback:** Arrow rotates 90° (▼ points down), content expands
4. **Content:** Status items slide in (smooth animation)
5. **Repeat Click:** Collapses again (content slides up, arrow rotates back)

**Why Expandable?**
- Reduces visual clutter for typical users
- Power users can inspect system state when troubleshooting
- Status updates are live (only shown if expanded, to avoid resource waste)

**Accessibility:**
- Screen readers announce: "System State, expandable button, not expanded"
- When expanded: "Expanded, Clock Visibility, Visible, Current Method, Modern, OS Build, Windows 11 Build 23H2"
- Keyboard: Tab to reach, Space/Enter to toggle expand/collapse

**Color Coding:**
- **Green:** Clock is visible (running as expected)
- **Red:** Clock is hidden (toggle was successful)
- **Grey:** Unknown or error state (add tooltip for diagnostics)

---

## Section 6: Reset Button

**Visual:**
```
┌────────────────────────────────────────────────────────────────┐
│                                                                │
│ [Button: Reset to Defaults]                                    │
│                                                                │
│ This will reset the hotkey to Ctrl+Alt+T and set the registry │
│ method to Auto-detect.                                         │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

**Button:**
- Label: "Reset to Defaults"
- Style: Subtle button (less prominent than primary actions)
- Enabled: Always (even if module is disabled)

**Interaction:**
1. **User Action:** Click "Reset to Defaults" button
2. **Immediate Effect:** (Option A) Dialog confirmation:
   ```
   ┌──────────────────────────────────────────┐
   │ Reset to Defaults?                       │
   │                                          │
   │ Hotkey: Ctrl+Alt+T                       │
   │ Method: Auto-detect                      │
   │                                          │
   │ [Cancel] [Reset]                         │
   └──────────────────────────────────────────┘
   ```
3. **After Confirmation:** All settings revert
   - Hotkey TextBox updates to "Ctrl+Alt+T"
   - Method ComboBox updates to "Auto-detect"
   - Toast notification: "Settings reset to defaults."
4. **Visual Feedback:** Button briefly highlights or shows loading state

**Alternative: No Confirmation**
- Simply reset immediately with toast notification: "Settings reset to defaults. Press Ctrl+Z to undo." (if undo supported)

**Accessibility:**
- Screen readers announce: "Button, Reset to Defaults"
- Keyboard: Tab to reach, Space/Enter to click

**Design Notes:**
- Subtle button style prevents accidental resets
- Confirmation dialog is recommended to prevent data loss
- Toast notification provides reassurance of successful reset

---

## Responsive Design Behavior

### Desktop (1200px+)
- All sections display at full width (MaxWidth 1200px applied)
- Two-column layout for Settings (Control on right, label on left)
- Status section remains expanded or collapsed per user preference

### Tablet (600px–1200px)
- Two-column layout preserved but may reflow to single column on smaller tablets
- Controls remain accessible but spacing may adjust
- StackPanel Spacing reduced from 24px to 16px to fit smaller screens

### Mobile (< 600px)
- Single-column layout (all controls stack vertically)
- ComboBox and TextBox full width
- Button text may abbreviate (e.g., "Record" instead of "Record Hotkey")
- Status expander hidden by default to save space

---

## Animation & Transitions

**Hover Effects:**
- Buttons: Subtle background color shift on hover
- Expander: Arrow rotates smoothly (0.2s), content fades in (0.3s)

**State Transitions:**
- Toggle switch: Smooth slide animation (0.1s)
- ComboBox dropdown: Slide down from control (0.15s)
- Status updates: Fade in (0.2s) when text changes

**No Distracting Effects:**
- Minimal animation maintains professional, business-like appearance
- Aligns with PowerToys design (functional, not decorative)

---

## Theme Support

### Light Mode
- **Background:** White or light grey
- **Text:** Dark grey or black (primary), medium grey (secondary)
- **Accents:** Blue (active/focus), green (success), red (error)

### Dark Mode
- **Background:** Dark grey or near-black
- **Text:** White or light grey (primary), medium grey (secondary)
- **Accents:** Light blue, green, red (same hue, adjusted for contrast)

**No Manual Theme Switching:**
- Uses `ThemeResource` bindings; automatically respects system dark/light mode
- No hardcoded colors; all from PowerToys design system

---

## Interaction Patterns (PowerToys Conventions)

1. **InfoBar for Contextual Info:** Section 3 uses InfoBar to explain registry methods
2. **Expander for Advanced Options:** Status section uses expander to hide power-user content
3. **ComboBox for Discrete Choices:** Registry method uses dropdown (not radio buttons)
4. **Read-Only TextBox for Display:** Hotkey display is non-editable (clarity)
5. **Subtle Button for Destructive Actions:** Reset button uses subtle style (prevent accidents)
6. **Dynamic Help Text:** Method description updates with selection (context-aware guidance)
7. **Color Coding:** Status visibility uses green/red (but text label provides redundancy)
8. **Toast Notifications:** Success/reset messages use toast, not modal dialogs (non-blocking)

---

## Error States & Edge Cases

### Module Disabled
- All sub-settings visually disabled (greyed out, reduced opacity)
- Buttons unable to click
- Help text explains: "Enable the module to configure this setting"

### No Hotkey Set
- TextBox shows placeholder: "Press keys..."
- Status: None (or "No hotkey configured; click Record to set one")

### Conflicting Hotkey
- TextBox shows previous hotkey (unchanged)
- Status: Red/orange warning message
- Recording mode remains active (allow user to try again)

### Registry Operation Failed
- Status message: "❌ Failed to update clock visibility. Check administrator permissions."
- Expander automatically expands to show OS/method details for debugging

### Rapid Toggles (Stress Test)
- Module handles multiple toggle requests within 100ms
- Only the last request is executed (debounced)
- UI remains responsive (no freezing)

---

## Summary of Key UX Decisions

| Aspect | Choice | Why |
|---|---|---|
| **Master Control** | ToggleSwitch (not checkbox) | Immediate visual feedback, familiar pattern |
| **Method Selection** | ComboBox (not radio buttons) | Cleaner layout, Auto-detect as default |
| **Hotkey Entry** | Record button (not freetext) | Prevents typing mistakes, clear interaction |
| **Status Info** | Expander (collapsed by default) | Reduces clutter, power users can expand |
| **Reset Action** | Subtle button (not primary) | Prevents accidental resets |
| **Help Text** | Dynamic (updates with selection) | Context-aware guidance, no static text |
| **Theme** | System theme (light/dark auto) | Consistent with PowerToys, no hardcoded colors |
| **Validation** | Inline messages (no modals) | Non-blocking, power-user friendly |

---

## References & Design System

- **PowerToys Design System:** Uses WinUI 3 default controls and theme resources
- **Fluent Design Principles:** Depth, motion, material (applied subtly)
- **Accessibility Standards:** WCAG 2.1 AA (keyboard navigation, color contrast, ARIA labels)
- **Example PowerToys Modules:** Awake, Color Picker, FancyZones (reference for UI patterns)
