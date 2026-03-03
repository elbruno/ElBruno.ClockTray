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
