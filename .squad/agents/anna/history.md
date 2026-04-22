# Anna — History

## Project Seed
- **Project:** ClockTray — Windows system tray app (.NET Global Tool)
- **Stack:** C# / .NET 10 / Win32 interop, net10.0-windows, PackageId: ElBruno.ClockTray
- **User:** Bruno Capuano
- **Goal:** Add CLI commands `clocktray show` and `clocktray hide` to control taskbar clock visibility headlessly, while keeping `clocktray` (no args) working as the tray app launcher.

## Day-1 Context
- Program.cs is minimal: `Application.Run(new ClockTrayApplicationContext())` — no arg parsing yet
- Already published as .NET Global Tool on NuGet
- ClockToggler.cs contains the Win32 logic for showing/hiding the clock
- Need headless mode: CLI commands must NOT open any UI, just act and exit

## Learnings

### 2026-03-10: CLI Parsing Implementation
**Pattern:** Dual-path Main() for GUI vs CLI modes
- **Challenge:** `[STAThread]` attribute cannot be applied conditionally at runtime
- **Solution:** Move `[STAThread]` from `Main()` to `RunGui()` helper method
  - CLI path (`args.Length > 0`) runs on default MTA thread, no WinForms dependencies
  - GUI path (`args.Length == 0`) calls `[STAThread] RunGui()` which launches ApplicationContext
- **Recommendation:** Native args[] parsing for simple CLIs (3-5 commands, no subcommands)
  - Zero dependencies, fast, easy to maintain
  - Manually implement --help/--version (only 10 extra LOC)
  - System.CommandLine adds 500KB for features we don't need
- **Exit Codes:** Always return int from Main() for proper shell integration (0 = success, 1 = error)
- **Console Output:** Use `Console.WriteLine()` for success, `Console.Error.WriteLine()` for errors
- **Backward Compat:** Check `args.Length` first to preserve no-args behavior (GUI launch)
