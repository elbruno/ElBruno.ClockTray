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
- **PackAsTool + UseWindowsForms**: .NET SDK blocks this combo with NETSDK1146 error. Solution: explicit SDK imports (`<Import Project="Sdk.props/targets" Sdk="Microsoft.NET.Sdk" />`) + override `_PackToolValidation` target after SDK imports. Set `_ToolPackShortTargetFrameworkName` to simple TFM (e.g., `net10.0`) instead of platform TFM (`net10.0-windows`).
- **Exe vs WinExe for tools**: `PackAsTool` requires `OutputType=Exe`. WinForms works with `Exe` but shows console window briefly on startup.

## Session: 2026-03-03T15:50
**Batch:** Dutch + Mac spawn  
**Work:** Implemented ClockTray WinForms MVP  
**Outcome:** ✅ Build successful, ready for testing  
**Files:** Program.cs, ClockTrayApplicationContext.cs, ClockToggler.cs, HotkeyWindow.cs, clock.ico, ClockTray.csproj, README.md  
**Output:** `.squad/decisions/inbox/mac-mvp-implementation.md` → merged to `decisions.md`

## Session: 2026-03-03T16:27
**Work:** Configured ClockTray as dotnet global tool  
**Outcome:** ✅ Tool installs and runs via `clocktray` command  
**Challenge:** NETSDK1146 blocks PackAsTool + UseWindowsForms  
**Solution:** Explicit SDK imports + override `_PackToolValidation` target + set correct tool TFM  
**Files:** ClockTray.csproj (v0.5.5), README.md  
**Command:** `dotnet tool install --global ElBruno.ClockTray`
