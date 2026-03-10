# Dutch Tech Spec Decisions — ClockTray PowerToys Module
**Date:** 2026-03-10  
**Lead:** Dutch (Architecture)  
**Status:** PROPOSED (awaiting team review)

---

## Overview

This document captures key architectural and design decisions made during tech spec authoring. These decisions guide the C++ implementation phase (Sprint 2).

---

## Decision 1: Four-Class Module Architecture

**Decision:** Implement ClockTray as four focused C++ classes:
1. **ClockTray** — Module lifecycle, PowertoyModuleIface implementation
2. **RegistryToggler** — Registry manipulation (modern & legacy paths), abstraction layer
3. **HotkeyHandler** — Win32 global hotkey registration, message window dispatch
4. **ConfigManager** — JSON settings serialization, state persistence

**Rationale:**
- **Separation of Concerns**: Each class has a single responsibility. Easier to test, maintain, extend.
- **Testability**: Mocking one layer (e.g., RegistryToggler) doesn't require OS interaction.
- **Reusability**: RegistryToggler and HotkeyHandler patterns can be extracted for other PowerToys modules.
- **Precedent**: Color Picker, Awake follow similar layered patterns.

**Alternatives Considered:**
- **Monolithic ClockTray class**: Simpler initially, but hard to test; Registry + Hotkey logic tightly coupled.
- **More granular classes (6+)**: Over-engineered for 800 LOC core; adds complexity without benefit.

**Risk Level:** LOW. Pattern proven in existing PowerToys modules.

---

## Decision 2: Runtime OS Version Detection (Not Compile-Time Branching)

**Decision:** Detect OS version at runtime via `RtlGetVersion` or `GetVersion`, then choose modern or legacy path dynamically.

**Implementation:**
```cpp
// Pseudocode
DWORD buildNumber = GetOSBuildNumber();  // Query at startup
if (buildNumber >= 22635) {
    // Win11 23H2+: Use modern path (ShowSystrayDateTimeValueName)
    UseModernPath();
} else {
    // Win10/Win11 pre-23H2: Use legacy path (HideClock + restart)
    UseLegacyPath();
}
```

**Rationale:**
- **Portability**: Single binary works on all supported OS versions. Users don't download "Win10 version" vs. "Win11 version."
- **User Transparency**: Auto-selects best path; user doesn't need to configure.
- **Future-Proof**: Windows 12+ can adopt modern path automatically.
- **Maintenance**: No need to maintain separate builds.

**Alternatives Considered:**
- **Compile-time branching (#ifdef WIN11)**: Requires multiple binaries; distribution headache.
- **Configuration UI toggle**: Users shouldn't choose; error-prone. Auto-detection is better.

**Risk Level:** LOW. Build number queries are stable; no edge cases known.

---

## Decision 3: Zero External Dependencies (C++20 Std Only)

**Decision:** Use ONLY C++ Standard Library 20 + Windows SDK. No NuGet packages, no vcpkg dependencies.

**Files Allowed:**
- C++ standard headers (`<memory>`, `<string>`, `<vector>`, `<json>` - if in std lib)
- Windows SDK headers (`<windows.h>`, `<winreg.h>`, `<winuser.h>`)
- PowerToys utility headers (`Logger.h`, `JsonHelpers.h`, `Settings.h` from PowerToys core)

**Files NOT Allowed:**
- External JSON library (nlohmann/json, etc.)
- Boost
- fmt library (use std::format from C++20)

**Rationale:**
- **Lightweight**: No dependency management. Easier for maintainers.
- **Security**: Fewer third-party vulnerabilities.
- **Precedent**: Color Picker, Awake follow same principle.
- **Build Simplicity**: CMake doesn't need vcpkg; reduces CI/CD complexity.

**Alternatives Considered:**
- **nlohmann/json**: Convenient, but unnecessary. C++20 has good string/JSON handling.
- **Boost.Interprocess**: Overkill for single-user registry changes.

**Risk Level:** LOW. Constraint is well-understood and proven feasible.

---

## Decision 4: Comprehensive Unit Tests with Mocked Win32 APIs

**Decision:** Unit tests do NOT call real Win32 APIs or modify real registry. Instead, mock all OS interactions.

**Test Structure:**
```cpp
// Example: MockRegistry.h
class MockRegistry {
    std::map<std::string, DWORD> state_;
public:
    LONG RegSetValue(const wchar_t* path, DWORD value) {
        state_[path] = value;
        return ERROR_SUCCESS;  // Simulated success
    }
};

// RegistryToggler.cpp (production)
RegistryToggler::Toggle() {
    if (modern_) {
        registry_->SetValue(kShowSystrayPath, newValue);
        SendMessageTimeout(...);  // Mock-able via dependency injection
    }
}
```

**Test Coverage Target:** >80% code paths covered.

**Rationale:**
- **No Registry Pollution**: Unit tests don't leave artifacts on test machines.
- **Speed**: Mocked tests run in milliseconds; registry I/O is slow.
- **Repeatability**: No OS state dependency; tests are deterministic.
- **CI/CD Safe**: Automated pipelines can run without Admin rights.
- **Precedent**: PowerToys unit tests follow this pattern (see Color Picker gtest suite).

**Alternatives Considered:**
- **Integration tests on real OS**: Too slow, requires Admin, causes test interference.
- **No mocking**: Registry pollution, flaky tests, CI/CD failures.

**Risk Level:** LOW. Mocking is standard practice.

**Note:** Manual regression testing on real Win10/Win11 builds happens in Sprint 4 (human-executed).

---

## Decision 5: Message-Only Window for Hotkey Dispatch

**Decision:** Create a hidden message-only window (`HWND` with `WS_POPUP | WM_SKIP_TASKBAR`) to receive WM_HOTKEY messages from the OS.

**Implementation:**
```cpp
// HotkeyHandler.cpp
HWND hWnd = CreateWindowExW(
    0,                          // Extended style
    L"STATIC",                  // Window class
    L"ClockTray Hotkey Listener",
    0,                          // Style = message-only (no WS_VISIBLE)
    0, 0, 0, 0,                 // Hidden dimensions
    HWND_MESSAGE,               // Message-only window
    nullptr,                    // No menu
    hInstance,
    nullptr
);

// Window procedure
LRESULT CALLBACK HotkeyWindowProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam) {
    if (msg == WM_HOTKEY) {
        // Hotkey pressed: trigger toggle
        ClockTray::GetInstance()->Toggle();
    }
    return DefWindowProc(hWnd, msg, wParam, lParam);
}
```

**Rationale:**
- **Windows Standard Pattern**: All Windows apps use this for global hotkeys.
- **Non-Intrusive**: Window is completely hidden; doesn't appear in taskbar or Alt+Tab.
- **Message Loop Integration**: Fits naturally with PowerToys' event loop.

**Alternatives Considered:**
- **Raw Win32 callback (without window)**: Not possible; Win32 requires a window handle.
- **Task Scheduler hooks**: Overly complex for simple hotkey.

**Risk Level:** LOW. Pattern is well-established, used by PowerToys globally.

---

## Decision 6: PowerToys Settings Integration (Not Custom Config UI)

**Decision:** Settings stored in PowerToys' centralized settings JSON, accessed via PowerToys' `Settings` and `JsonHelpers` classes. Settings UI is a WinUI 3 panel hosted in PowerToys Settings app (not a standalone dialog).

**Files Involved:**
- PowerToys settings JSON: `C:\Users\<user>\AppData\Local\Microsoft\PowerToys\settings.json`
- ClockTray config section: Added to JSON under `"ClockTray": { ... }`
- Settings panel XAML: `src/modules/ClockTray/settings/ClockTraySettings.xaml`

**Rationale:**
- **Consistency**: Users see all PowerToys settings in one place.
- **Maintenance Burden**: PowerToys core handles JSON lifecycle; we just read/write.
- **Localization**: PowerToys handles translation; we provide strings in `.resw` format.
- **Precedent**: All PowerToys modules (Color Picker, Awake, etc.) follow this pattern.

**Alternatives Considered:**
- **Custom config file** (e.g., `ClockTraySettings.json` in AppData): Creates orphaned files, confuses users.
- **Registry config**: Mixes concerns; PowerToys uses JSON for new modules.

**Risk Level:** LOW. Pattern is standard; integration examples available.

---

## Decision 7: Timeline: 5 Weeks, 4 Sprints

**Decision:** Organize implementation into four focused sprints:

| Sprint | Duration | Owner | Focus |
|--------|----------|-------|-------|
| 1 | Week 1 | Dutch + Blain | Tech spec + UI mockup |
| 2 | Weeks 2–3 | Mac + Dillon | C++ core + unit tests |
| 3 | Week 4 | Blain + Mac | Settings UI + integration |
| 4 | Week 5 | Dillon + Dutch | Testing + docs + submission |

**Rationale:**
- **Realistic Pace**: 2 weeks for C++ core is achievable for experienced Win32 dev (Mac).
- **Parallel Work**: Blain starts UI design in Sprint 1 while Dutch finalizes spec.
- **Testing Last**: Concentrate testing in Sprint 4 to catch integration issues.
- **Submission Readiness**: PR ready by end of Sprint 4.

**Alternatives Considered:**
- **3-week sprint**: Too rushed; insufficient testing time.
- **8-week sprint**: Overkill for small, focused module.

**Risk Level:** MEDIUM. Timeline depends on team availability and no major blockers. If C++ module complexity exceeds expectations, escalate to Dutch by end of Week 2.

---

## Decision 8: Legacy Path Uses ShellExecute (Not CreateProcess) for Explorer Restart

**Decision:** When restarting explorer.exe (legacy path, Win10/Win11 pre-23H2), use `ShellExecuteW("explorer")` rather than `CreateProcessW`.

**Rationale:**
- **Graceful Handling**: ShellExecute respects user's shell preferences (Windows Terminal, etc.), not forced cmd.exe.
- **System Resilience**: If explorer.exe is already starting, ShellExecute is more forgiving.
- **Precedent**: Windows updates use ShellExecute for explorer restart.

**Alternatives Considered:**
- **CreateProcessW**: More direct control, but requires harder-coded paths; less flexible.
- **taskkill /IM explorer.exe && explorer**: Risky; brief period with no taskbar.

**Risk Level:** LOW. Approach proven in Windows utilities.

---

## Decision 9: No Tray Icon State Indicator (MVP)

**Decision:** Tray icon is static (clock icon always same appearance). Do NOT add visual indicator (e.g., "X" overlay) to show clock visible/hidden state.

**Rationale:**
- **Scope Control**: Icon design, multiple states = unnecessary complexity for MVP.
- **MVP Focus**: Feature works; visual polish can follow.
- **Tooltip Fallback**: Tooltip can show state if needed: "Clock: visible" / "Clock: hidden" (future enhancement).

**Alternatives Considered:**
- **Icon + "X" overlay**: Nice-to-have, but adds 2–3 days design/implementation work.
- **Animated icon**: Overkill; confusing.

**Risk Level:** LOW. Can be added post-launch without breaking changes.

---

## Decision 10: Hotkey Validation (UI-Level, Not Kernel-Level)

**Decision:** Settings UI validates hotkey binding against a known-bad list (Ctrl+Alt+Del, Win+X, etc.), but does NOT prevent invalid bindings at OS level. If user binds to a conflicting hotkey, registration will fail gracefully; module continues via tray menu.

**Validation:**
- UI shows warning: "Hotkey already in use" (if detected)
- If user ignores warning, hotkey registration fails on next module enable
- Error logged; module remains enabled and functional via tray menu

**Rationale:**
- **User Agency**: Let users try custom hotkeys; fail gracefully, not block.
- **Simplicity**: No need to monitor OS hotkey registry constantly.
- **Fallback Path**: Tray menu always works; hotkey is convenience, not requirement.

**Alternatives Considered:**
- **Kernel-level monitoring**: Overly complex; maintenance burden.
- **Strict blocking**: Frustrates power users who know what they're doing.

**Risk Level:** LOW. Graceful degradation is acceptable.

---

## Open Questions for Team Discussion

**Q1: Should hotkey picker validate against OS shortcuts?**
- **Current Decision:** UI warns only; no kernel-level blocking.
- **Team Input Needed:** Is warning enough, or should we block known conflicts?

**Q2: Should tray icon show state (visible/hidden)?**
- **Current Decision:** Static icon; optional tooltip (MVP post-feature).
- **Team Input Needed:** Priority for launch, or defer to post-launch?

**Q3: What is acceptable auto-restart behavior (legacy path)?**
- **Current Decision:** ShellExecute explorer (graceful, system-managed).
- **Team Input Needed:** Any concerns with brief taskbar absence? Known compatibility issues?

**Q4: For CI/CD testing, how do we test registry changes safely?**
- **Current Decision:** Unit tests with mocks; Sprint 4 manual regression on real OS.
- **Team Input Needed:** Do we need a regression test matrix (spreadsheet) or just documentation?

---

## Sign-Off

| Role | Name | Approval | Comments |
|------|------|----------|----------|
| Lead | Dutch | ✅ APPROVED | As author; subject to team review |
| Backend | Mac | ⏳ PENDING | Review for implementation feasibility |
| UI | Blain | ⏳ PENDING | Review for Settings UI alignment |
| QA | Dillon | ⏳ PENDING | Review for test strategy |
| User | Bruno | ⏳ PENDING | Review for requirements alignment |

---

## Implementation Checklist (For Sprint 2)

**Mac + Dillon to verify before starting code:**
- [ ] Review this decisions document
- [ ] Confirm C++20 standard library capabilities (string, containers, format)
- [ ] Confirm mocking strategy (gtest + GoogleMock available in PowerToys build)
- [ ] Identify PowerToys utility headers (Logger, JsonHelpers, Settings) locations
- [ ] Schedule pair programming session (Dutch + Mac) for architecture kickoff

---

**Status:** Ready for team discussion.  
**Next Step:** Team review + sign-off, then begin Sprint 2 (Mac + Dillon).
