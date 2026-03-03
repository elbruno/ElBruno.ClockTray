# Decisions

## Team Decisions Log

### 1. Tech Stack Analysis & MVP Recommendation (2026-03-03)
**Agent:** Dutch  
**Status:** Approved & Implemented

ClockTray requires toggling taskbar clock. Windows lacks direct API; two registry approaches exist:
- **Method A (Legacy):** `HKCU\...Policies\Explorer\HideClock` + Explorer restart (works Win10/11, disruptive)
- **Method B (Modern):** `HKCU\...Explorer\Advanced\ShowSystrayDateTimeValueName` + `WM_SETTINGCHANGE` broadcast (Win11 23H2+ only, seamless)

**Framework Evaluation:**
| Framework | MVP Speed | Tray Support | PowerToys Path | Recommendation |
|---|---|---|---|---|
| C# WinForms | ⭐⭐⭐ (2-4h) | Native `NotifyIcon` | ❌ | ✅ **Phase 1** |
| C# WPF | ⭐⭐ (4-6h) | 3rd-party pkg | ❌ | Alternative |
| C++ Win32 | ⭐ (8-12h) | `Shell_NotifyIcon` | ✅ | **Phase 2** |
| C++ WinUI 3 | ⭐ (12-16h) | Manual interop | ✅ | Overkill now |

**Decision:** Two-phase approach
- **Phase 1:** C# WinForms MVP (fast, ships immediately)
- **Phase 2:** C++ rewrite as PowerToys module (future contribution)

---

### 2. MVP Implementation (2026-03-03)
**Agent:** Mac  
**Status:** Delivered ✅

Implemented Phase 1 per Dutch's recommendation:
- **Language:** C# / .NET 8
- **UI Pattern:** `ApplicationContext` (tray-only, no visible window)
- **Global Hotkey:** P/Invoke `RegisterHotKey` via `NativeWindow` wrapper
- **OS Detection:** Build number check (≥22631 for Win11 modern path, else legacy)
- **Registry Toggle:** P/Invoke `RegSetValueEx` + `SendMessageTimeout` broadcast
- **Hotkey:** Ctrl+Alt+T (per Dutch's recommendation)
- **Icon:** Fallback generated if clock.ico missing

**Files:** Program.cs, ClockTrayApplicationContext.cs, ClockToggler.cs, HotkeyWindow.cs, clock.ico, ClockTray.csproj, README.md  
**Build Status:** ✅ Compiles successfully

---

### 3. Next: PowerToys C++ Module (Future)
**Planned Phase 2**

Rewrite core toggle logic as C++ DLL:
- Implement `PowertoyModuleIface` in `src/modules/ClockTray/dll/`
- Export `powertoy_create()` factory
- Settings UI via WinUI 3 in PowerToys settings panel
- Reuse Registry + WM_SETTINGCHANGE logic (logic is trivial; effort is integration)
