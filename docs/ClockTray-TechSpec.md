# ClockTray: Taskbar Clock Visibility Toggle Module
## A PowerToys Utility for Enhanced Taskbar Control

**Version:** 1.0  
**Date:** March 2026  
**Lead Architect:** Dutch (ElBruno team)  
**Target:** Microsoft PowerToys Repository Contribution  
**Expected Duration:** 5-week implementation sprint

---

## 1. Problem Statement

### Power Users and Taskbar Real Estate

Windows power users frequently demand fine-grained control over taskbar elements. The taskbar's date/time display—while useful to some—consumes valuable screen real estate on compact monitors, ultrawide displays, and during presentations. Moreover, the taskbar itself has limited customization options in Windows, and users often resort to third-party tools or manual registry edits to reclaim that space.

**The Core Pain Point:** Today, there is no easy, reversible way to toggle the taskbar clock on/off without:
1. Diving into Windows Settings (Date & time → Notification area → Select icons to appear on the taskbar) — buried 3 layers deep
2. Manually editing the Windows Registry
3. Using opaque Group Policy Editor (gpedit.msc) on Pro/Enterprise editions only

This friction frustrates power users who may want to toggle the clock seasonally, per-monitor, or during specific workflows (gaming, presentations, focus sessions).

### Why PowerToys?

PowerToys exists to remove exactly this kind of friction. Its philosophy is "power users, we hear you; let's build what you need." ClockTray embodies this philosophy: it's a single-purpose, low-overhead utility that gives users back control over their taskbar.

**Precedent:** PowerToys' *Awake* (prevent sleep), *Color Picker* (quick color sampling), and *Text Extractor* (OCR from screen) all solve single, high-frequency pain points. ClockTray follows the same pattern.

---

## 2. Proposed Solution

### ClockTray: One-Click Taskbar Clock Toggle

ClockTray provides:

1. **System Tray Integration**: A single tray icon with context menu offering:
   - "Hide Clock" / "Show Clock" (one-click toggle)
   - Settings (open PowerToys settings → ClockTray panel)
   - Exit

2. **Global Hotkey**: Customizable keyboard shortcut (default: Ctrl+Alt+T) to toggle the clock instantly without opening a menu

3. **Settings Panel**: Integrated into PowerToys Settings app:
   - Enable/disable the module
   - Toggle current clock visibility state
   - Customize hotkey binding
   - Choose toggle method (Modern path for Win11 23H2+; Legacy for older builds)

4. **OS Compatibility**: Seamless operation across Windows 10 (build 20H2+) and all Windows 11 versions

5. **Zero External Dependencies**: Relies only on Windows APIs (Registry, Win32 shell APIs, WM_SETTINGCHANGE broadcasts)

### How It Works (Technical Summary)

ClockTray uses two complementary registry methods based on the host OS:

- **Modern Method (Win11 23H2+)**: Modifies `HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\ShowSystrayDateTimeValueName` (DWORD, 1=show, 0=hide), then broadcasts `WM_SETTINGCHANGE` with parameter "TraySettings". The OS applies the change instantly without restarts.

- **Legacy Method (Win10 20H2+, Win11 pre-23H2)**: Modifies `HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\HideClock` (DWORD, 1=hide, 0=show), then restarts the Windows Explorer process. The clock re-appears/disappears on next launch.

The module auto-detects the OS version and applies the appropriate method transparently.

---

## 3. Architecture Overview

### Module Structure (C++ Implementation)

ClockTray will be implemented as a **PowerToys Module DLL** following the standard `PowertoyModuleIface` architecture used by all PowerToys utilities.

#### Directory Layout
```
src/modules/ClockTray/
├── CMakeLists.txt                 # Build configuration
├── CMakeLists_gtest.txt           # Unit test configuration
├── ClockTray.vcxproj             # Visual Studio project
├── dll/
│   ├── ClockTray.cpp             # Main module implementation
│   ├── ClockTray.h               # Module header
│   ├── dllmain.cpp               # DLL entry point
│   ├── pch.h / pch.cpp           # Precompiled headers
│   ├── exports.cpp               # PowertoyModuleIface exports
│   └── resource.rc               # Icon, version info
├── settings/
│   ├── ClockTraySettings.xaml    # WinUI 3 settings panel
│   ├── ClockTraySettings.xaml.cpp
│   └── ClockTraySettings.xaml.h
├── tests/
│   ├── ClockTrayTests.cpp        # Unit tests (gtest)
│   ├── MockRegistry.h            # Test helpers
│   └── MockWindowsAPIs.h
└── docs/
    ├── README.md                  # User-facing guide
    └── DEVELOPMENT.md             # Contribution guide
```

#### PowertoyModuleIface Implementation

All ClockTray functionality exports through the standard `PowertoyModuleIface` interface (defined in PowerToys' `modules/interface/powertoy_module_interface.h`):

```cpp
// Core lifecycle
extern "C" __declspec(dllexport) PowertoyModule* powertoy_create();
extern "C" __declspec(dllexport) void powertoy_destroy(PowertoyModule* module);

// Module state
void enable();                           // Activate module, register hotkey
void disable();                          // Deactivate module, unregister hotkey
bool is_enabled();                       // Return current enabled state

// Configuration
void send_config_json(const wchar_t* config);  // Receive settings from UI
void get_config(const wchar_t* buffer);        // Report current settings

// Metadata
const wchar_t* get_name();                     // Return "ClockTray"
const wchar_t* get_description();              // Return user-facing description
const wchar_t* get_config_editor_path();       // WinUI settings panel path
bool is_elevated();                            // Return whether module needs admin rights
```

#### Core Classes

1. **ClockTray (main module class)**
   - Manages module lifecycle (enable/disable)
   - Coordinates with RegistryToggler and HotkeyHandler
   - Stores configuration state
   - Implements PowertoyModuleIface

2. **RegistryToggler**
   - Encapsulates registry manipulation
   - Detects OS version (build number) and chooses modern or legacy path
   - Reads/writes registry values safely
   - Broadcasts `WM_SETTINGCHANGE` for modern path
   - Handles Explorer restart for legacy path

3. **HotkeyHandler**
   - Registers/unregisters global hotkeys via Win32 API
   - Listens for hotkey messages via hidden window (message-only window)
   - Invokes RegistryToggler on hotkey press
   - Supports customizable hotkey bindings from settings

4. **ConfigManager**
   - Serializes/deserializes JSON settings
   - Manages user preferences (hotkey binding, enabled/disabled state)
   - Persists config to PowerToys' central settings store

#### Data Flow

```
PowerToys Settings UI (WinUI 3)
    ↓ (settings JSON)
ConfigManager
    ↓ (hotkey binding)
HotkeyHandler (global hotkey registration)
    ↓ (user presses hotkey)
Message-only window receives WM_HOTKEY
    ↓
RegistryToggler.Toggle()
    ↓
Check OS version → Modern or Legacy path
    ├─ Modern: Write registry + WM_SETTINGCHANGE broadcast
    └─ Legacy: Write registry + Explorer.exe restart
    ↓
Taskbar clock appears/disappears
```

#### Interaction with PowerToys Infrastructure

- **Settings Storage**: ClockTray configuration stored in PowerToys' centralized settings JSON, managed via `JsonHelpers` and `Settings` classes from PowerToys core.

- **Settings UI Panel**: WinUI 3 XAML panel hosted within PowerToys Settings app (same mechanism as Color Picker, Awake).

- **Logging**: Uses PowerToys' `Logger` utility for diagnostic output (debug/error level).

- **Resource Strings**: Localization via PowerToys' `.resw` resource format (future-proof for multi-language support).

---

## 4. Key Features

### For End Users

1. **One-Click Toggle**: Tray icon context menu provides instant on/off switch
2. **Global Hotkey**: Ctrl+Alt+T (customizable) for power users
3. **Visual Feedback**: Tray icon indicates current state (optional tooltip: "Clock: visible" / "Clock: hidden")
4. **No Restarts Needed** (on Win11 23H2+): Modern path applies changes instantly
5. **Settings Integration**: Full integration with PowerToys Settings, no separate UI
6. **Seamless OS Handling**: Automatically uses modern or legacy path based on build version

### For Developers

1. **Clean Architecture**: Separation of concerns (Registry, Hotkey, Config layers)
2. **Comprehensive Test Coverage**: Unit tests mock all Win32 APIs; no integration test dependencies
3. **Well-Documented Code**: Inline comments explain registry paths and OS-specific logic
4. **Reusable Patterns**: Registry and hotkey helpers can be extracted for other modules (precedent: PowerToys' `Logger`, `JsonHelpers`)
5. **Low Maintenance Burden**: ~800 LOC core implementation; no external dependencies; self-contained

---

## 5. Technical Dependencies

### Required Windows APIs

| API | Module | Purpose | Min Windows |
|-----|--------|---------|-------------|
| `RegOpenKeyExW` | Registry | Open registry key | Win10 20H2 |
| `RegSetValueExW` | Registry | Set registry value (clock toggle) | Win10 20H2 |
| `RegQueryValueExW` | Registry | Query current state | Win10 20H2 |
| `RegCloseKey` | Registry | Close registry key | Win10 20H2 |
| `SendMessageTimeoutW` | Win32 | Broadcast WM_SETTINGCHANGE | Win10 20H2 |
| `RegisterHotKeyW` | Win32 | Register global hotkey | Win10 20H2 |
| `UnregisterHotKeyW` | Win32 | Unregister hotkey | Win10 20H2 |
| `CreateWindowExW` | Win32 | Create hidden message window | Win10 20H2 |
| `DestroyWindow` | Win32 | Clean up window | Win10 20H2 |
| `GetVersion` / `RtlGetVersion` | Win32 | Detect OS version (for method selection) | Win10 20H2 |
| `ShellExecuteW` | Win32 | Restart Explorer process (legacy path only) | Win10 20H2 |

### No External Dependencies

ClockTray uses **zero external libraries**:
- ✅ No NuGet packages
- ✅ No vcpkg dependencies
- ✅ Standard C++ 20 library only
- ✅ Windows SDK (already required by PowerToys)

This keeps the module lightweight, eliminates dependency management overhead, and reduces attack surface.

### Compiler & Build Environment

- **Language Standard**: C++20
- **Compiler**: MSVC (Visual Studio 2022 or later)
- **Architecture**: x64 (PowerToys standard)
- **CRT**: Dynamic CRT (matching PowerToys convention)
- **Build System**: CMake (consistent with PowerToys build)

---

## 6. Compatibility Matrix

### Supported Operating Systems

| OS | Build | Modern Path | Legacy Path | Status |
|---|----|----|----|----|
| Windows 10 | 20H2 (19042) | ❌ | ✅ | Supported |
| Windows 10 | 21H2 (19044) | ❌ | ✅ | Supported |
| Windows 11 | 21H2 (22000) | ❌ | ✅ | Supported |
| Windows 11 | 22H2 (22621) | ❌ | ✅ | Supported |
| Windows 11 | 23H2 (22635+) | ✅ | ✅ | **Preferred (modern)** |
| Windows 11 | Insider Fast Ring | ✅ | ✅ | Supported |

**Key Insight**: Win11 23H2 introduces the modern registry path. Older builds use the legacy path, which works reliably but requires Explorer restart. ClockTray auto-detects and uses the appropriate method per-system.

### Known Limitations & Considerations

1. **Explorer Restart (Win10/Win11 pre-23H2)**: When using the legacy path, ClockTray restarts `explorer.exe`. This is safe (Windows restarts Explorer automatically on crashes anyway), but may briefly:
   - Minimize all open windows (standard Explorer behavior)
   - Reset some taskbar state (icons may reorganize momentarily)
   
   **Mitigation**: Settings UI alerts users on older builds that the clock change requires a brief restart.

2. **Elevated Privileges**: ClockTray does NOT require administrator rights; it modifies `HKCU` (current user registry), which is always writable by the user.

3. **Multi-User Systems**: Each user's clock visibility is independent (stored in `HKCU`). One user toggling the clock does not affect other users.

4. **Future OS Versions**: Windows 12+ will inherit the modern path. Architecture future-proofs this by checking build number at runtime.

---

## 7. Implementation Timeline (5-Week Sprint)

### Sprint 1: Tech Spec & Design (Week 1)
**Owner:** Dutch (Lead)

**Deliverables:**
- ✅ This tech spec (comprehensive design document)
- WinUI 3 settings panel mockup (Blain)
- Architecture review checklist
- PowerToys compatibility audit (review existing modules)

**Success Criteria:**
- Spec >1500 words, covers all 10 sections
- Mockup aligns with PowerToys design language
- No architectural red flags identified

---

### Sprint 2: Core C++ Module (Weeks 2–3)
**Owner:** Mac (Backend Dev), Dillon (QA)

**Deliverables:**
- Compilable C++ module skeleton (PowerToys template)
- RegistryToggler class (both modern and legacy paths, fully tested)
- HotkeyHandler class (hotkey registration, message window)
- ConfigManager class (JSON serialization)
- ClockTray main class (Iface implementation)
- Unit tests (gtest, >80% code coverage)

**Daily Checkpoints:**
- Day 1–2: Project scaffold + RegistryToggler (no hotkeys yet)
- Day 3–4: HotkeyHandler + message window dispatch
- Day 5–6: ConfigManager + end-to-end plumbing
- Day 7–8: Unit tests + edge cases (rapid toggles, invalid JSON, Explorer restart race conditions)
- Day 9–10: Code review & fixes

**Success Criteria:**
- Compiles with `/W4` (warning level 4) and zero warnings
- All unit tests pass
- Code coverage >80% (reportable via OpenCppCoverage or similar)

---

### Sprint 3: Settings UI & Integration (Week 4)
**Owner:** Blain (UI Dev), Mac (config bridge)

**Deliverables:**
- WinUI 3 settings panel XAML + code-behind
- Two-way binding to module settings (enable/disable toggle, hotkey picker, status display)
- Integration tests (settings change → module state change → taskbar reflects)
- Settings storage (JSON persistence verified)

**Daily Checkpoints:**
- Day 1–2: XAML layout + basic binding (toggle enable/disable)
- Day 3: Hotkey picker control (map hotkey string to VirtualKey enum)
- Day 4–5: Integration testing (launch settings, change toggle, verify module re-evaluates)
- Day 6: Polish UI (match PowerToys design, accessibility review)

**Success Criteria:**
- Settings panel launches in PowerToys Settings app
- Toggle changes reflect in module state within 1 second
- Hotkey picker allows custom bindings (validated)
- All settings persist across app restart

---

### Sprint 4: Testing, Docs & Submission (Week 5)
**Owner:** Dutch (Orchestration), Dillon (QA), TechWriter (Docs)

**Deliverables:**
- Comprehensive test matrix (Windows versions × test scenarios)
- User documentation (README.md with usage guide)
- Developer documentation (DEVELOPMENT.md, contributing guidelines)
- Code review checklist (PowerToys standards compliance)
- PR submission to PowerToys/main

**Daily Checkpoints:**
- Day 1: OS compatibility testing (Win10 20H2, 21H2, Win11 21H2/22H2/23H2)
- Day 2: Edge case testing (rapid toggles, settings while toggled, Explorer restart during toggle, invalid hotkey configs)
- Day 3: Performance testing (startup time, memory footprint, resource handles)
- Day 4: Documentation writing (user guide, dev guide, PR description)
- Day 5: Final code review, sign-off, PR submission

**Success Criteria:**
- ✅ All OS versions passing
- ✅ Edge case results documented (known issues, if any, logged)
- ✅ User & dev docs complete + reviewed by team
- ✅ PR submitted; CI/CD green

---

## 8. Risk Mitigation

### Risk: PowerToys Team Rejects "Too Simple"

**Impact:** HIGH (blocks contribution)  
**Likelihood:** LOW (precedent exists: Awake, Text Extractor are single-purpose)

**Mitigation:**
- Tech spec emphasizes **real power-user pain** (buried settings, no quick toggle)
- Reference existing precedents (Awake prevents sleep with one click; ClockTray prevents clock visibility with one click)
- User feedback: This feature is frequently requested on Reddit, forums
- Positioning: "micro-utility for professionals (presentations, focus, minimal distraction)" not "consumer feature"

---

### Risk: WM_SETTINGCHANGE Broadcast Breaks on Future Win11 Builds

**Impact:** MEDIUM (fallback to legacy path, but requires restart)  
**Likelihood:** LOW (broadcast mechanism stable since Win11 21H2)

**Mitigation:**
- Legacy path (registry + Explorer restart) remains fallback
- Monitor Windows Insider updates; adjust if needed (low effort, <20 LOC change)
- Unit tests cover both paths; easy to verify with new OS builds
- Document in DEVELOPMENT.md that this is a known monitoring area

---

### Risk: C++ Module Complexity Too High for Small Team

**Impact:** MEDIUM (scope creep, missed sprint deadline)  
**Likelihood:** MEDIUM (Mac new to PowerToys module architecture)

**Mitigation:**
- Reference existing modules (Color Picker ~2000 LOC, Awake ~1500 LOC)
- Pair programming sessions: Dutch + Mac early in Sprint 2 for architecture validation
- Minimal feature scope (no multi-user, no roaming, no cloud sync)
- Reuse PowerToys utilities (Logger, JsonHelpers, Settings) rather than reimplementing

---

### Risk: Settings Panel UI Inconsistent with PowerToys Design Language

**Impact:** MEDIUM (may require redesign before acceptance)  
**Likelihood:** LOW (Blain experienced with PowerToys UI; design system clear)

**Mitigation:**
- Align early with PowerToys' WinUI 3 design tokens (spacing, colors, typography)
- Iterative design review (Blain shares mockups with Dutch in Sprint 1; incorporate feedback)
- Reference Color Picker & Awake settings panels as templates
- Accessibility review (WCAG 2.1 AA minimum per PowerToys standards)

---

### Risk: Hotkey Customization Edge Cases (Invalid Bindings, Modifier Conflicts)

**Impact:** LOW (user-facing UX issue, not blocker)  
**Likelihood:** MEDIUM (hotkey handling is error-prone)

**Mitigation:**
- Hotkey picker control validates binding (reject invalid combinations, warn on conflicts with OS shortcuts)
- Unit tests cover edge cases (Ctrl+Alt, Win key alone, function keys)
- Settings UI shows user-friendly error messages
- Default hotkey (Ctrl+Alt+T) rarely conflicts with built-in shortcuts
- Fallback: if hotkey registration fails, module still works via tray menu

---

## 9. Success Criteria

### Code Quality

- [ ] Compiles without warnings (MSVC `/W4` strict)
- [ ] Unit tests: >80% code coverage (RegistryToggler, HotkeyHandler, ConfigManager fully mocked)
- [ ] Static analysis clean (PREfast or similar PowerToys standard tool)
- [ ] Code review: 2+ PowerToys maintainers sign-off

### Functionality

- [ ] Tray icon context menu works (Hide/Show toggles clock visibility)
- [ ] Global hotkey works (Ctrl+Alt+T toggles clock)
- [ ] Settings panel appears in PowerToys settings
- [ ] Settings changes persist across app restart
- [ ] Hotkey customization functional (user can rebind to custom key)
- [ ] OS auto-detection works (Win10 uses legacy, Win11 23H2+ uses modern)

### Compatibility

- [ ] Windows 10 20H2 (legacy path): Clock toggles, Explorer restart observed
- [ ] Windows 11 21H2, 22H2 (legacy path): Clock toggles, restart observed
- [ ] Windows 11 23H2+ (modern path): Clock toggles instantly, no restart
- [ ] Multiple monitors: Clock toggles on all monitors simultaneously
- [ ] Multi-user: Each user's clock state independent

### Edge Cases

- [ ] Rapid toggles (10+ in 1 second): No race conditions, final state consistent
- [ ] Settings changed while module disabled: Settings persist; changes apply on re-enable
- [ ] Explorer restart (legacy path) during user interaction: Graceful handling; window restored
- [ ] Invalid hotkey config (e.g., corrupted JSON): Module disables gracefully; error logged
- [ ] Uninstall/reinstall: No registry artifacts left behind

### Documentation

- [ ] Tech spec: >1500 words, all 10 sections, approved by PowerToys team
- [ ] User guide: Clear, concise, screenshots, troubleshooting section
- [ ] Dev guide: Contribution guidelines, how to extend, code structure walkthrough
- [ ] PR description: Clear summary, links to spec, video demo if available

### Submission

- [ ] PR opened to Microsoft/PowerToys main branch
- [ ] CI/CD pipeline green (builds, tests, linting pass)
- [ ] PowerToys team review initiated (target: <1 week initial feedback)
- [ ] Follow-up commits addressing team feedback within <2 weeks

---

## 10. References & Precedents

### PowerToys Modules (Existing Examples)

1. **Color Picker** (`src/modules/ColorPicker/`)
   - Single-purpose utility (sample colors from screen)
   - ~2000 LOC, zero external deps
   - Global hotkey (Win+Shift+C)
   - Settings panel in PowerToys app
   - **Relevance:** Similar scope and architecture to ClockTray

2. **Awake** (`src/modules/Awake/`)
   - Prevents Windows from sleeping
   - ~1500 LOC, zero external deps
   - Timer-based (countdown or indefinite)
   - Settings panel (timer duration, theme)
   - **Relevance:** Single-function utility; good reference for settings UI

3. **FancyZones** (`src/modules/FancyZones/`)
   - Window snapping utility
   - Larger scope (~5000+ LOC), but modular
   - **Relevance:** Demonstrates scaling; not a 1:1 template for ClockTray

### PowerToys Architecture Documents

- **Module Interface:** PowerToys GitHub → `doc/devdocs/modules/module_interface.md`
- **Settings Architecture:** PowerToys GitHub → `doc/devdocs/settings_architecture.md`
- **Contributing Guidelines:** PowerToys GitHub → `CONTRIBUTING.md`

### Windows Development Resources

- **Registry Paths:**
  - Modern: `HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\ShowSystrayDateTimeValueName`
  - Legacy: `HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\HideClock`
  
- **Win32 API Documentation:**
  - Registry: https://learn.microsoft.com/en-us/windows/win32/api/winreg/
  - Hotkey: https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhoteyw
  - Messages: https://learn.microsoft.com/en-us/windows/win32/winmsg/wm-settingchange

- **Build System:**
  - PowerToys CMake structure: https://github.com/microsoft/PowerToys/blob/main/CMakeLists.txt
  - WinUI 3 docs: https://learn.microsoft.com/en-us/windows/apps/winui/

### Internal References

- **Phase 1 MVP (C# Proof-of-Concept):** `ElBruno.ClockTray` repository
  - Logic already proven (registry toggle, hotkey, tray integration all validated)
  - C++ rewrite reuses architectural decisions from MVP
  - See `.squad/decisions.md` for tech stack rationale

---

## 11. Appendix: Glossary & Terminology

| Term | Definition |
|------|-----------|
| **Modern Path** | Win11 23H2+ registry approach: `ShowSystrayDateTimeValueName` + `WM_SETTINGCHANGE` broadcast. Instant, no restart. |
| **Legacy Path** | Win10/Win11 pre-23H2 approach: `HideClock` under Policies + Explorer restart. Disruptive but reliable. |
| **PowertoyModuleIface** | Standard C++ interface all PowerToys modules implement. Defines lifecycle (enable/disable), config, metadata. |
| **Message-Only Window** | Win32 window class with `WS_POPUP` + `WM_SKIP_TASKBAR` flags. Receives messages (hotkey signals) but doesn't render. |
| **Registry Hive** | `HKCU` = current user registry (per-user settings, writable without admin). `HKLM` = local machine (system-wide, requires admin). ClockTray uses HKCU. |
| **WM_SETTINGCHANGE** | Win32 broadcast message. Apps listen to know when system settings changed. Used here to signal clock visibility change to taskbar. |
| **WinUI 3** | Modern UI framework for Windows apps. Used by PowerToys settings app. Replaces WPF/UWP. |
| **Hotkey** | Global keyboard shortcut (e.g., Ctrl+Alt+T) registered system-wide. Works even when PowerToys window unfocused. |

---

## Conclusion

ClockTray is a well-scoped, low-risk contribution to PowerToys. It solves a real power-user pain point (taskbar clock toggle), follows existing architectural patterns (Color Picker, Awake), and introduces zero external dependencies.

The 5-week sprint plan is realistic and achievable for a small team (Mac, Blain, Dillon, Dutch). The tech stack (C++20, Win32 APIs, WinUI 3) aligns perfectly with PowerToys' infrastructure. Success criteria are clear and measurable.

We are ready to proceed to Phase 2: C++ module implementation.

---

**Authored by:** Dutch (Technical Lead, ElBruno.ClockTray squad)  
**Date:** March 2026  
**Status:** Ready for PowerToys team review
