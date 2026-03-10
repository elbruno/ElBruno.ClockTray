# Blain's UI Design Decisions — Sprint 1

**Date:** March 10, 2026  
**Phase:** Sprint 1 (WinUI 3 Settings Panel Design)  
**Status:** Ready for Team Review

---

## Decision 1: ToggleSwitch vs Checkbox for Master Enable/Disable

**Decision:** Use `ToggleSwitch` (WinUI 3)

**Rationale:**
- **Immediate Feedback:** ToggleSwitch provides clear visual state change (on/off sliding animation)
- **PowerToys Convention:** All PowerToys modules use ToggleSwitch for enable/disable (see: Awake, Color Picker)
- **Accessibility:** WinUI 3 ToggleSwitch has built-in ARIA support; screen readers announce clearly
- **Familiar Pattern:** Power users recognize ToggleSwitch as the standard Windows 11 UI component
- **Keyboard Support:** Space/Enter toggles; Tab navigates (standard behavior)

**Alternative Considered:** Checkbox
- **Rejected Because:** Checkbox is less visually distinct; doesn't feel as "modern" for a toggle action

**Impact:** Module enable/disable is the primary user action; this control choice sets the tone for the entire panel

---

## Decision 2: ComboBox vs RadioButtons for Registry Method Selection

**Decision:** Use `ComboBox` (dropdown)

**Rationale:**
- **Cleaner Layout:** ComboBox takes minimal space; three radio buttons would dominate the panel
- **Discoverability:** Dropdown naturally suggests "choose one"; radio buttons less obvious for mutually exclusive options
- **PowerToys Pattern:** FancyZones, Color Picker use ComboBox for similar multi-choice settings
- **Accessibility:** ComboBox has strong keyboard support (Arrow keys navigate, Enter selects)
- **Help Text:** Dynamic help text (updates based on selection) pairs well with ComboBox

**Alternative Considered:** Radio Button Group
- **Rejected Because:** Takes up more vertical space; three options side-by-side or stacked both problematic

**Alternative Considered:** Buttons with Toggle State
- **Rejected Because:** Overly complex for simple selection; unclear which one is "active"

**Impact:** Method selection is a power-user setting; ComboBox keeps it accessible but not overwhelming

---

## Decision 3: Record Button vs TextBox Input for Hotkey Entry

**Decision:** Use dedicated "Record Hotkey" Button + read-only TextBox display

**Rationale:**
- **Prevents Typos:** TextBox input would allow "invalid" hotkeys (typos like "Crtl+Alt+T"); Record mode validates on actual key presses
- **Familiar Pattern:** OBS, many gaming apps use similar "record hotkey" UI (power users recognize it)
- **Clear Intent:** Record button makes it obvious that clicking initiates a specific action (not general text editing)
- **Validation at Capture Time:** Recording mode validates immediately (modifier key required, system conflicts, etc.)
- **Read-Only Display:** TextBox shows current hotkey but prevents accidental manual edits

**Alternative Considered:** Plain TextBox with Free Typing
- **Rejected Because:** User could type invalid combos ("hello", "ctrl"), wasting time on validation

**Alternative Considered:** Hotkey Picker Control (Custom)
- **Rejected Because:** No built-in WinUI 3 hotkey picker; would require custom control (out of scope for Sprint 1)

**Impact:** Hotkey entry is a critical user interaction; Record button pattern ensures quality input and prevents support requests from typos

---

## Decision 4: Expander for Status Display (Collapsed by Default)

**Decision:** Use `Expander`, collapsed by default

**Rationale:**
- **Reduces Clutter:** Status info (OS build, current method, clock visibility) is diagnostic; most users don't need it
- **Power-User Feature:** Advanced users can expand to troubleshoot or verify system state
- **PowerToys Pattern:** Awake module uses Expander for advanced options; follows established convention
- **Performance:** Polling registry for status only happens when Expander is open (CPU optimization)
- **Cognitive Load:** Typical users see 5 sections (not 8 with status expanded); cleaner mental model

**Alternative Considered:** Always-Visible Status Section
- **Rejected Because:** Adds visual noise; takes up 15% of panel height unnecessarily

**Alternative Considered:** Separate "System Info" Page/Tab
- **Rejected Because:** PowerToys settings are typically single-page per module; navigation adds friction

**Impact:** Status display is diagnostic; keeping it collapsed respects user's primary goal (configuring the module)

---

## Decision 5: Toast Notifications vs Modal Dialogs for Feedback

**Decision:** Use Toast notifications for success/error messages; Modal only for destructive actions (Reset confirmation)

**Rationale:**
- **Non-Blocking:** Toasts don't interrupt user flow; modals demand user attention
- **PowerToys Convention:** All modules use toasts for routine feedback (setting saved, toggle applied, etc.)
- **Accessibility:** Toast messages read by screen readers; users can continue navigation while message plays
- **Dismissal:** Toasts auto-dismiss after 3–5 seconds (or can be dismissed manually); modals require explicit action

**Exception:** Reset to Defaults
- **Uses Modal Confirmation:** Destructive action (resets all settings); confirmation prevents accidents
- **Justification:** Standard UX pattern for destructive operations

**Impact:** Keeps the UI feeling responsive and non-intrusive; users won't feel "stuck" confirming messages

---

## Decision 6: Two-Column Grid Layout (Label Left, Control Right)

**Decision:** Use Grid with two columns: Label/Help (70%, left) + Control (30%, right, MinWidth 300px)

**Rationale:**
- **Label Proximity:** Help text sits immediately below label; control nearby
- **Responsive:** On small screens (<600px), switches to single column (control below label)
- **Alignment:** Label and help text left-aligned; controls right-aligned (visually organized)
- **PowerToys Standard:** FancyZones, Awake use similar two-column layout
- **Readability:** Ample space for label/help text; controls have room to breathe

**Alternative Considered:** Single-Column (Stack Everything)
- **Rejected Because:** Makes panel unnecessarily tall; less efficient use of screen space

**Alternative Considered:** Label Above Control (Stacked)
- **Rejected Because:** Creates visual "towers" of content; less scannable

**Impact:** Layout strongly influences how users read the panel; two-column layout balances information density with readability

---

## Decision 7: WinUI 3 Theme Resources (No Hardcoded Colors)

**Decision:** Use `{ThemeResource}` bindings for all colors, fonts, spacing

**Rationale:**
- **Light/Dark Mode:** Automatic inversion; colors are correct in both themes without manual adjustment
- **DPI Scaling:** Theme resources scale automatically at 100%, 125%, 150%, 200% (Win10) and beyond (Win11)
- **High Contrast Mode:** Respects user's high contrast settings (if enabled in Accessibility)
- **Consistency:** All PowerToys modules use same theme resources; unified visual language
- **Maintainability:** If PowerToys design system changes, settings panel updates automatically

**Alternative Considered:** Manual Color Adjustments per Theme
- **Rejected Because:** Error-prone; requires testing every theme change; violates DRY principle

**Impact:** Ensures settings panel looks correct regardless of OS version, user theme preference, or accessibility settings

---

## Decision 8: Hotkey Validation Rules & Conflict Detection

**Decision:** Implement multi-level validation:
1. Modifier key required (Ctrl, Alt, Shift, or Windows)
2. Single primary key only
3. Warn against Windows reserved shortcuts (Win+E, etc.)
4. Block conflicts with other PowerToys modules

**Rationale:**
- **User Experience:** Prevent invalid input (typos, incomplete combos)
- **System Stability:** Block hotkeys that conflict with OS or other apps (prevent broken behavior)
- **Conflict Resolution:** Clear error messages guide user to choose alternative hotkey
- **Win-Win:** Validation happens real-time during recording (not after submission); user gets instant feedback

**Validation Levels:**
- **Level 1 (Hard Block):** Modifier required, single key only, PowerToys conflicts
- **Level 2 (Soft Warn):** Windows reserved shortcuts, common third-party hotkeys

**Impact:** Validation is critical to module reliability; prevents support requests from "hotkey doesn't work"

---

## Decision 9: Dynamic Help Text (Updates Based on Selection)

**Decision:** MethodDescription property updates automatically when user changes Registry Method selection

**Rationale:**
- **Context-Aware Guidance:** Each method option has nuanced pros/cons; help text explains specific choice
- **Reduces Documentation Burden:** User doesn't need external wiki; information is inline
- **Space Efficient:** Single help text area (not three separate labels for each option)
- **PowerToys Pattern:** Similar to Awake module (help text changes based on setting)

**Examples:**
- Auto-detect: "This will use Modern on Win11 23H2+, Legacy on Win10."
- Modern: "Seamless toggle. Recommended for Windows 11 23H2+."
- Legacy: "May require Explorer restart. Use on Windows 10 or older Windows 11."

**Impact:** Improves user education; reduces friction when choosing method

---

## Decision 10: Default Module State = OFF (Opt-In)

**Decision:** Enable/Disable toggle defaults to OFF on first install

**Rationale:**
- **Consent:** User explicitly enables module (respects principle of least surprise)
- **Safety:** Hotkey not registered until user confirms intent (doesn't hijack keyboard globally)
- **Adoption:** Users who install but don't need module won't incur registry/hotkey overhead
- **Onboarding:** First-time user sees clear "Enable ClockTray" step; understands what they're activating

**Alternative Considered:** Default to ON
- **Rejected Because:** Forces all users to be "opt-out" (less respectful); may hijack users' keyboard expectations

**Impact:** Influences first-time user experience; sets tone for module as respectful, non-intrusive

---

## Decision 11: Config Format = JSON (PowerToys Standard)

**Decision:** Settings stored in `%APPDATA%\PowerToys\modules\ClockTray\config.json`

**Schema:**
```json
{
  "enabled": false,
  "method": "Auto",
  "hotkey": "Ctrl+Alt+T"
}
```

**Rationale:**
- **PowerToys Standard:** All modules use JSON; ensures consistency
- **Human-Readable:** JSON is easy to inspect/debug (power users can edit manually)
- **Portable:** JSON is platform-agnostic (future C++ implementation can use same config format)
- **Serialization:** WinUI ViewModel can serialize/deserialize via `JsonSerializer`

**Alternative Considered:** Windows Registry (`.REG`)
- **Rejected Because:** PowerToys convention is JSON; mixing .REG and JSON is inconsistent

**Alternative Considered:** XML
- **Rejected Because:** Verbose; JSON is modern PowerToys standard

**Impact:** Config format is the bridge between UI and C++ module; JSON ensures seamless handoff during Phase 2

---

## Team Review Notes

**This decision document is ready for team input:**
- [ ] **Dutch (Architecture Lead):** Confirm layout and design patterns align with PowerToys architecture
- [ ] **Mac (Backend):** Confirm config JSON schema is implementable in C++; no conflicts with module interface
- [ ] **Dillon (QA):** Confirm validation rules and error handling are testable; suggest test cases
- [ ] **Bruno (User):** Confirm settings panel meets project requirements; no missing features

---

## Next Steps (Sprint 2–3 Implementation)

1. **Mac:** Implement `PowertoyModuleIface` in C++; expose settings JSON interface
2. **Blain:** Implement XAML + ViewModel; bind to module's config methods
3. **Dillon:** Test UI on Win10/Win11; validate hotkey conflicts, registry methods, edge cases
4. **Dutch:** Code review for PowerToys standards; prepare submission checklist

---

## Sign-Off

| Role | Status |
|---|---|
| **Blain (UI Designer)** | ✅ Confirmed (March 10, 2026) |
| **Dutch (Lead)** | ⏳ Pending review |
| **Mac (Backend)** | ⏳ Pending review |
| **Dillon (QA)** | ⏳ Pending review |
