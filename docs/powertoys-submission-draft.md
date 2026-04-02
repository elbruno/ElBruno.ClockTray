# PowerToys Contribution Submission Drafts
## ClockTray: Taskbar Clock Visibility Toggle

---

## SECTION 1: Comment for Issue #28769

I'd like to contribute **ClockTray** to PowerToys—a lightweight utility that gives users instant control over their taskbar date/time display via hotkey (Ctrl+Alt+T) or tray icon. The module solves the friction of hiding/showing the clock, which currently requires digging into Settings → Personalization → Taskbar. A working C# prototype on `feature/powertoys-contribution` validates that both Windows 10 (legacy registry path) and Windows 11 (modern seamless path) work reliably. The core implementation is ~150 LOC with zero external dependencies—ideal for PowerToys' lightweight philosophy. I'd love feedback on fit and next steps toward the C++ module implementation. See [ISSUE_LINK] for the full feature request.

---

## SECTION 2: GitHub Feature Request Issue

**Suggested Title:** 
ClockTray: Global Hotkey to Toggle Taskbar Clock Visibility

---

### Issue Body

## Problem Statement

Windows power users frequently want to reclaim taskbar screen real estate by hiding the taskbar date/time display, yet there's no quick, reversible way to toggle it:
- **Current workaround:** Settings → Personalization → Taskbar → Select icons to appear on the taskbar (buried 3+ levels deep)
- **Alternative:** Manual Registry editing or Group Policy (Pro/Enterprise only)

This friction is especially painful for users who work across multiple monitors, use full-screen applications, or want to toggle the clock seasonally.

**Why PowerToys:** This is exactly the kind of single-purpose, high-frequency power user problem PowerToys was designed to solve. Precedents include *Awake* (prevent sleep), *Color Picker* (quick sampling), and *Text Extractor* (OCR)—all solve targeted pain points with minimal overhead.

---

## Proposed Solution: ClockTray

ClockTray provides instant, reversible control over taskbar clock visibility via:
1. **Global hotkey** (default: Ctrl+Alt+T, customizable)
2. **System tray icon** with context menu
3. **Integrated settings panel** in PowerToys settings (enable/disable, customize hotkey, toggle clock state)

### How It Works

The module uses two registry-based methods depending on OS version:

- **Modern path (Windows 11 23H2+):** Modify `ShowSystrayDateTimeValueName` registry key + broadcast `WM_SETTINGCHANGE` → instant, seamless toggle
- **Legacy path (Windows 10 20H2+, Windows 11 pre-23H2):** Modify `HideClock` registry key + restart Explorer → toggle visible on next launch

The module auto-detects the OS and applies the appropriate method transparently.

---

## Why ClockTray Fits PowerToys

| Criterion | How ClockTray Aligns |
|-----------|-------------------|
| **Remove friction** | Reduces 5-step Settings navigation to 1 hotkey press |
| **Minimal scope** | Does one thing only (~150 LOC core logic, zero external dependencies) |
| **No duplication** | No existing PowerToys utility manages taskbar clock visibility |
| **Empower user control** | Users get instant, reversible control over a system element they couldn't easily toggle before |
| **Lightweight** | Minimal CPU/memory overhead; no background polling, pure event-driven architecture |

**Precedent examples:** Color Picker, Awake, FancyZones—all solve specific power user needs without reinventing Windows.

---

## Technical Approach

### Architecture
- **Language:** Modern C++ (C++17 minimum)
- **Framework:** PowerToys module plugin pattern (`PowertoyModuleIface` implementation)
- **Settings UI:** WinUI 3 (integrated into PowerToys settings app)
- **Dependencies:** None (Windows APIs only: Registry, Win32 shell, `WM_SETTINGCHANGE`)

### Module Structure
```
src/modules/ClockTray/
├── dll/
│   ├── ClockTray.cpp/h         (module interface + lifecycle)
│   ├── ClockToggler.cpp/h      (registry toggle logic)
│   └── HotkeyWindow.cpp/h      (Win32 message window)
├── settings/
│   └── ClockTraySettings.xaml  (WinUI 3 settings panel)
├── tests/
│   └── ClockTrayTests.cpp      (>80% coverage target)
└── CMakeLists.txt              (build config)
```

### Key Implementation Points
- **Hotkey binding:** Standard Win32 `RegisterHotKey` via message window
- **State persistence:** Module state (clock visible/hidden) via registry; user config (hotkey) via PowerToys settings JSON
- **OS detection:** Win32 version check; conditionally apply modern/legacy path
- **Error handling:** Robust logging, graceful fallback to alternative method if registry unavailable
- **Testing:** Unit tests for registry toggle, hotkey binding, and OS-specific paths (Win10 20H2 → Win11 23H2 matrix)

---

## OS Compatibility

- **Windows 10:** Build 20H2 and later (legacy registry path)
- **Windows 11:** All versions (modern path on 23H2+, legacy path on earlier)
- **Architecture:** x64 (PowerToys standard)

Both paths tested and validated via working C# prototype.

---

## Working Prototype

A fully functional C# proof-of-concept exists at:
**https://github.com/elbruno/ElBruno.ClockTray/tree/feature/powertoys-contribution**

This prototype validates:
- ✅ Both Win10 and Win11 registry paths work reliably
- ✅ Tray icon integration and hotkey binding functional
- ✅ Zero external dependencies
- ✅ Core concept (toggle logic) ~150 LOC

The C++ module will port this core logic while following PowerToys module architecture and adding comprehensive tests.

---

## Technical Specification

A detailed technical specification is available in the repository:
**`doc/ClockTray-TechSpec.md`** — covers problem statement, architecture, module structure, compatibility matrix, and implementation plan.

---

## Call to Action

**Questions for the team:**
1. Does ClockTray fit PowerToys' scope and philosophy in your assessment?
2. Are there any concerns with the two-path registry approach (modern/legacy)?
3. Should we prioritize one OS path over the other in initial release?
4. Any additional requirements for settings UI integration or module lifecycle?

**Proposed next steps:**
- Feedback on this feature request and technical fit
- Guidance on any breaking changes or constraints in current PowerToys architecture
- Approval to proceed with C++ implementation and PowerToys PR submission

We're eager to contribute and work with the team to meet PowerToys standards (code quality, test coverage, documentation).

---

## Labels Suggested
- `type: feature request`
- `type: utility`
- `help wanted` (if open to community collaboration)

