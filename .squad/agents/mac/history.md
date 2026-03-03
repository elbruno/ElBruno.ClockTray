# Mac — History

## Project Context
- **Project:** ClockTray — Windows system tray app to show/hide date and time in the taskbar
- **Stack:** C# / .NET / WPF or WinUI, Win32 interop
- **User:** Bruno Capuano

## Learnings
- Built ClockTray MVP as .NET 8 WinForms app per Dutch's Phase 1 recommendation
- `NotifyIcon` + `ApplicationContext` pattern works great for tray-only apps (no visible Form needed)
- Win11 23H2+ (build 22631+) supports `ShowSystrayDateTimeValueName` registry key with `WM_SETTINGCHANGE` broadcast — no Explorer restart
- Older Windows needs `Policies\Explorer\HideClock` + Explorer restart (disruptive)
- Global hotkey via `RegisterHotKey` P/Invoke requires a hidden `NativeWindow` to receive `WM_HOTKEY` messages
- Generated a simple clock .ico programmatically with Python for the app icon

## Session: 2026-03-03T15:50
**Batch:** Dutch + Mac spawn  
**Work:** Implemented ClockTray WinForms MVP  
**Outcome:** ✅ Build successful, ready for testing  
**Files:** Program.cs, ClockTrayApplicationContext.cs, ClockToggler.cs, HotkeyWindow.cs, clock.ico, ClockTray.csproj, README.md  
**Output:** `.squad/decisions/inbox/mac-mvp-implementation.md` → merged to `decisions.md`
