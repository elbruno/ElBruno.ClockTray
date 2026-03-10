# Dutch — History

## Project Context
- **Project:** ClockTray — Windows system tray app to show/hide date and time in the taskbar
- **Stack:** C# / .NET / WPF or WinUI, Win32 interop
- **User:** Bruno Capuano
- **Goal:** Tray icon with context menu + optional hotkey. Potential PowerToys contribution.

## Learnings
- **Taskbar clock toggle**: No direct Win32 API exists. Two registry approaches: (1) `HideClock` under Policies\Explorer (requires Explorer restart), (2) `ShowSystrayDateTimeValueName` under Explorer\Advanced (Win11 23H2+, no restart needed via `WM_SETTINGCHANGE` broadcast with `"TraySettings"`).
- **PowerToys module architecture**: C++ DLL implementing `PowertoyModuleIface`, exported via `powertoy_create()` factory. Modules live under `src/modules/<name>/`. Settings UIs use WinUI 3.
- **System tray**: WinForms `NotifyIcon` is the fastest path. C++ uses `Shell_NotifyIcon` + `NOTIFYICONDATA`. WinUI 3 has no built-in tray support.
- **Global hotkeys**: All stacks use Win32 `RegisterHotKey`/`UnregisterHotKey` (P/Invoke from C#).
- **Recommended approach**: Two-phase — WinForms MVP first, then C++ rewrite for PowerToys contribution.

## Session: 2026-03-03T15:50
**Batch:** Dutch + Mac spawn  
**Work:** Tech stack analysis & recommendation  
**Outcome:** ✅ Phase 1 approved & implemented by Mac  
**Output:** `.squad/decisions/inbox/dutch-tech-stack-analysis.md` → merged to `decisions.md`

## Session: 2026-03-10 (Tech Spec Authoring)
**Work:** Authored comprehensive PowerToys tech spec (10 sections, 1500+ words)  
**Outcome:** ✅ ClockTray-TechSpec.md complete  
**Key Decisions:**
- **Architecture**: Four-class structure (ClockTray, RegistryToggler, HotkeyHandler, ConfigManager) with clear separation of concerns
- **Compatibility Strategy**: Runtime build-number detection (modern vs. legacy path) rather than compile-time branching
- **External Dependencies**: Zero (uses Win32 APIs only, standard C++20, PowerToys utilities for logging/settings)
- **Test Strategy**: Comprehensive unit test suite with mocked Win32 APIs; no OS-specific integration tests (registry changes risky in CI/CD)
- **Risk Profile**: LOW overall. Existing Color Picker + Awake precedents prove similar single-purpose utilities are accepted. Legacy fallback path mitigates modern path instability
- **Team Readiness**: Mac + Blain + Dillon capable; no new skill gaps identified. Pair programming recommended for C++20 PowerToys module patterns

**Learnings from PowerToys Spec Review:**
1. **Contribution Workflow**: PowerToys accepts new utilities AFTER tech spec approval. Tech spec + design mockup → discussion → approval → implementation. This spec is the gate.
2. **Module Patterns**: Color Picker & Awake show single-function utilities (with >80% test coverage) are valued. ClockTray aligns perfectly with this pattern.
3. **Settings Architecture**: PowerToys uses centralized JSON config + WinUI 3 XAML panels. No custom dialogs. ConfigManager must integrate with PowerToys' `Settings` class (not reinvent).
4. **Win32 Interop**: Registry + WM_SETTINGCHANGE + hotkeys are low-level, but well-understood by team from Phase 1 MVP. C++ rewrite is straightforward port.
5. **Localization Consideration**: PowerToys community handles localization (not us). Prepare resource strings in `.resw` format from day one (easy to extract later).

**Spec Structure Decisions:**
- 10 sections (Problem, Solution, Architecture, Features, Dependencies, Compatibility, Timeline, Risk, Success, References) match Microsoft PowerToys standards
- Included glossary & appendix for clarity (non-technical readers + PowerToys maintainers)
- Emphasized low maintenance burden (0 external deps, 800 LOC core, proven C# MVP = lower risk)
- Referenced existing modules (precedent argument) + Microsoft standards (credibility with maintainers)
- Timeline realistic: 5 weeks, clearly staged (spec → core → UI → testing)

**Open Questions for Team Discussion:**
1. Should hotkey customization UI validate against OS shortcuts (Ctrl+Alt+Del, Win+X, etc.)? Or warn only?
2. On legacy path, should we use `ShellExecute("explorer")` or `CreateProcess("explorer.exe")`? (ShellExecute is safer, lets system handle restart.)
3. Should ClockTray tray icon indicate current state (e.g., tooltip "Clock: visible" or visual indicator)? (Nice-to-have; not in MVP.)
4. For CI/CD testing, how do we test registry changes safely in automated environment? (Mock-only in unit tests; manual regression on target OS versions.)

**Artifacts Created:**
- `doc/ClockTray-TechSpec.md` (this spec, ready for PowerToys team)

**Cross-Agent Updates (Sprint 1 Kickoff):**
- **Blain (UI Designer):** Completed WinUI 3 settings panel design (XAML structure, mockups, behavior specs) + extracted 9 reusable WinUI 3 patterns into skill document (652 lines)
- **Molly (Technical Writer):** Completed documentation framework (overview, guide outline, PowerToys context) + extracted 7 reusable documentation patterns into skill document (417 lines)

**Next Steps for Squad:**
1. ✅ Spec complete. Ready for Bruno approval + PowerToys community discussion.
2. ✅ Blain's UI design complete (unblocks Mac for backend implementation)
3. ✅ Molly's documentation framework complete (unblocks full prose writing after outline approval)
4. 🎯 Sprint 1 Week 2: Team review of all artifacts; Dutch opens GitHub Discussion on PowerToys repo
5. 🎯 Sprint 2: Mac + Dillon begin C++ core implementation
6. 🎯 Sprint 3: Blain implements XAML settings panel (based on production-ready design)
7. 🎯 Week 5: Dutch prepares PR submission to PowerToys/main
