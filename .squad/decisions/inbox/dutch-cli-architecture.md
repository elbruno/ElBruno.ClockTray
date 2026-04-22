# CLI Architecture Plan for ClockTray

**Author:** Dutch (Lead)  
**Date:** 2026-07-14  
**Status:** Proposed  
**Requested by:** Bruno Capuano  
**Implementers:** Anna (CLI Specialist — CLI parsing), Mac (Backend Dev — integration)

---

## 1. Command Surface (Full CLI Contract)

ClockTray is already a .NET Global Tool (`dotnet tool install -g ElBruno.ClockTray`) with command name `clocktray`. The CLI must be dual-mode: headless CLI commands **and** GUI tray app from the same binary.

| Invocation | Behavior | Exit Code |
|---|---|---|
| `clocktray` | Launch tray app (existing behavior, unchanged) | N/A (long-running) |
| `clocktray show` | Show taskbar clock, print confirmation, exit | 0 success, 1 error |
| `clocktray hide` | Hide taskbar clock, print confirmation, exit | 0 success, 1 error |
| `clocktray status` | Print `Clock is visible` or `Clock is hidden`, exit | 0 |
| `clocktray toggle` | Toggle current state, print new state, exit | 0 success, 1 error |
| `clocktray --help` / `clocktray -h` | Print usage summary, exit | 0 |
| `clocktray --version` | Print version (from assembly), exit | 0 |
| `clocktray <unknown>` | Print error + usage hint, exit | 2 |

> **Note:** `toggle` is included because it maps directly to the existing `ClockToggler` logic and is useful for keybind scripts. Bruno's request focused on `show`/`hide`, but `toggle` and `status` round out the CLI for scripting scenarios.

### Output Format (CLI Mode)

```
$ clocktray show
ClockTray: Clock is now visible.

$ clocktray hide
ClockTray: Clock is now hidden.

$ clocktray status
ClockTray: Clock is visible.

$ clocktray toggle
ClockTray: Clock is now hidden.

$ clocktray --help
ClockTray — Toggle Windows taskbar clock visibility

Usage:
  clocktray              Launch system tray app (GUI mode)
  clocktray show         Show the taskbar clock
  clocktray hide         Hide the taskbar clock
  clocktray toggle       Toggle taskbar clock visibility
  clocktray status       Print current clock state
  clocktray --help       Show this help message
  clocktray --version    Show version
```

---

## 2. CLI Library Decision

### Options Evaluated

| Library | Pros | Cons |
|---|---|---|
| **Native `args[]`** | Zero deps, tiny surface, no NuGet additions | Manual help text, manual parsing, no built-in `--version` |
| **System.CommandLine** | Official MS, rich features, middleware | Heavy for 4 commands, still preview (2.x), adds dependency |
| **Spectre.Console** | Beautiful output, tables, progress | Overkill — we're printing single lines, adds large dependency |

### ✅ Recommendation: Native `args[]` parsing

**Justification:**
1. **Zero new dependencies.** ClockTray's csproj has zero NuGet packages today. This is a strength for a .NET Global Tool (small install, fast startup). Adding System.CommandLine or Spectre.Console would break this zero-dep property.
2. **Trivial surface area.** We have exactly 4 verbs (`show`, `hide`, `toggle`, `status`) plus 2 flags (`--help`, `--version`). A `switch` statement handles this in ~30 lines.
3. **Startup performance.** Global tools should feel instant. Native parsing avoids reflection and assembly loading overhead from third-party libraries.
4. **PowerToys compatibility.** Phase 2 C++ rewrite won't have System.CommandLine. Keeping parsing simple means the CLI contract stays portable.
5. **Future escape hatch.** If we later need subcommands with options (e.g., `clocktray config --hotkey Ctrl+Alt+T`), we can upgrade to System.CommandLine then. YAGNI for now.

> **Decision:** Use native `args[]` with a simple switch/case dispatcher. No new NuGet packages.

---

## 3. Architecture: Headless CLI vs GUI Tray Split

### The Core Constraint

`Program.cs` currently has `[STAThread]` and calls `Application.Run()` which starts a WinForms message loop. CLI commands must **never** enter this loop — they must execute, print, and exit immediately.

### Execution Flow

```
Program.Main(args)
    │
    ├─ args.Length == 0 ───────────► GUI MODE
    │       [STAThread] ✓
    │       ApplicationConfiguration.Initialize()
    │       Application.Run(new ClockTrayApplicationContext())
    │
    └─ args.Length > 0 ────────────► CLI MODE
            Parse args[0]
            switch:
              "show"      → CliHandler.RunShow()
              "hide"      → CliHandler.RunHide()
              "toggle"    → CliHandler.RunToggle()
              "status"    → CliHandler.RunStatus()
              "--help"/"-h" → CliHandler.PrintHelp()
              "--version"   → CliHandler.PrintVersion()
              _           → CliHandler.PrintUnknown(args[0])
            Environment.Exit(exitCode)
```

### Key Design Points

1. **[STAThread] stays on Main.** It's harmless for CLI mode (STA apartment is set but never used if we don't create WinForms controls). No need for a separate entry point.

2. **Early exit.** CLI commands call `Environment.Exit(code)` after printing. This ensures no WinForms initialization happens. The branch occurs **before** `ApplicationConfiguration.Initialize()`.

3. **ClockToggler is already static and headless.** `SetClockVisibility()`, `IsClockVisible()`, and `IsWin11Modern()` have no WinForms dependency. They work perfectly in CLI mode. No changes needed to ClockToggler.cs.

4. **Console output in a WinForms app.** Since `OutputType` is `Exe` (not `WinExe`), `Console.WriteLine` works when launched from a terminal. When launched from Explorer (GUI mode, no args), console output is simply discarded. This is exactly the behavior we want.

### Why Not Two Separate Binaries?

Keeping one binary means:
- Single `dotnet tool install` command
- `clocktray` command works for both GUI and CLI
- No confusion about which binary to invoke
- Simpler packaging and CI/CD

---

## 4. File Changes Required

### Modified Files

| File | Change | Owner |
|---|---|---|
| **Program.cs** | Add args check at top of `Main()`. If `args.Length > 0`, dispatch to `CliHandler` and exit. Else, continue to GUI path. ~15 lines added. | Anna |
| **ClockTray.csproj** | No changes needed. `OutputType: Exe` already supports console output. Zero new NuGet packages. | — |
| **ClockToggler.cs** | No changes needed. Already has the static methods CLI needs. | — |

### New Files

| File | Purpose | Owner |
|---|---|---|
| **CliHandler.cs** | Static class with methods: `RunShow()`, `RunHide()`, `RunToggle()`, `RunStatus()`, `PrintHelp()`, `PrintVersion()`, `PrintUnknown(string)`. Each returns an `int` exit code. | Anna |

### Unchanged Files

- `ClockTrayApplicationContext.cs` — GUI-only, not touched
- `HotkeyWindow.cs` — GUI-only, not touched
- `LunarClockOverlay.cs` — GUI-only, not touched
- `ChineseCalendarHelper.cs` — utility, not touched
- `SolarTermCalculator.cs` — utility, not touched

### Test Files

| File | Purpose | Owner |
|---|---|---|
| **ClockTray.Tests/CliHandlerTests.cs** | Unit tests for CLI parsing logic and output. Mock-friendly since ClockToggler is static (can test help/version/unknown without registry access). | Anna |

---

## 5. Exit Code Conventions

| Code | Meaning | When |
|---|---|---|
| **0** | Success | Command executed successfully |
| **1** | Error | Registry write failed, permission denied, OS incompatibility |
| **2** | Unknown command | Unrecognized verb or flag |

### Error Output Convention

Errors go to `Console.Error.WriteLine()` so they can be separated from stdout in scripts:

```csharp
Console.Error.WriteLine("ClockTray: Error — failed to set clock visibility.");
Console.Error.WriteLine("  Ensure you have write access to HKCU registry.");
```

---

## 6. Backward Compatibility

### ✅ Confirmed: Zero Breaking Changes

| Scenario | Before | After | Status |
|---|---|---|---|
| `clocktray` (no args) | Launches tray app | Launches tray app (same path) | ✅ Identical |
| Double-click exe | Launches tray app | Launches tray app | ✅ Identical |
| `dotnet tool run clocktray` | Launches tray app | Launches tray app | ✅ Identical |
| Ctrl+Alt+T hotkey | Works in tray mode | Works in tray mode (unchanged) | ✅ Identical |

**The only behavioral change is additive:** `clocktray <verb>` now does something useful instead of being silently ignored (current `Main()` ignores args entirely).

---

## 7. Implementation Skeleton for Anna

### Program.cs (modified)

```csharp
namespace ClockTray;

static class Program
{
    [STAThread]
    static int Main(string[] args)
    {
        // CLI mode: any arguments → headless command, no GUI
        if (args.Length > 0)
        {
            return CliHandler.Execute(args);
        }

        // GUI mode: no arguments → launch tray app (existing behavior)
        ApplicationConfiguration.Initialize();
        Application.Run(new ClockTrayApplicationContext());
        return 0;
    }
}
```

> **Note:** Return type changes from `void` to `int` so exit codes propagate to the shell. This is fully backward-compatible — the runtime handles both signatures.

### CliHandler.cs (new)

```csharp
namespace ClockTray;

/// <summary>
/// Handles CLI command dispatch. All methods are headless (no WinForms).
/// </summary>
public static class CliHandler
{
    public static int Execute(string[] args)
    {
        var command = args[0].ToLowerInvariant();

        return command switch
        {
            "show"      => RunShow(),
            "hide"      => RunHide(),
            "toggle"    => RunToggle(),
            "status"    => RunStatus(),
            "--help" or "-h" => PrintHelp(),
            "--version"      => PrintVersion(),
            _                => PrintUnknown(command),
        };
    }

    private static int RunShow()
    {
        try
        {
            ClockToggler.SetClockVisibility(true);
            Console.WriteLine("ClockTray: Clock is now visible.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ClockTray: Error — {ex.Message}");
            return 1;
        }
    }

    private static int RunHide()
    {
        try
        {
            ClockToggler.SetClockVisibility(false);
            Console.WriteLine("ClockTray: Clock is now hidden.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ClockTray: Error — {ex.Message}");
            return 1;
        }
    }

    private static int RunToggle()
    {
        try
        {
            bool wasVisible = ClockToggler.IsClockVisible();
            ClockToggler.SetClockVisibility(!wasVisible);
            var state = wasVisible ? "hidden" : "visible";
            Console.WriteLine($"ClockTray: Clock is now {state}.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ClockTray: Error — {ex.Message}");
            return 1;
        }
    }

    private static int RunStatus()
    {
        bool visible = ClockToggler.IsClockVisible();
        Console.WriteLine($"ClockTray: Clock is {(visible ? "visible" : "hidden")}.");
        return 0;
    }

    private static int PrintHelp()
    {
        Console.WriteLine("""
            ClockTray — Toggle Windows taskbar clock visibility

            Usage:
              clocktray              Launch system tray app (GUI mode)
              clocktray show         Show the taskbar clock
              clocktray hide         Hide the taskbar clock
              clocktray toggle       Toggle taskbar clock visibility
              clocktray status       Print current clock state
              clocktray --help       Show this help message
              clocktray --version    Show version
            """);
        return 0;
    }

    private static int PrintVersion()
    {
        var version = typeof(CliHandler).Assembly.GetName().Version;
        Console.WriteLine($"ClockTray {version?.ToString(3) ?? "unknown"}");
        return 0;
    }

    private static int PrintUnknown(string command)
    {
        Console.Error.WriteLine($"ClockTray: Unknown command '{command}'.");
        Console.Error.WriteLine("Run 'clocktray --help' for usage.");
        return 2;
    }
}
```

---

## 8. Open Questions (for team discussion)

1. **Should `clocktray show` be idempotent-silent?** Current plan: always prints confirmation even if clock was already visible. Alternative: print "Clock is already visible." — slightly more informative but adds a read-before-write check. **Recommendation:** Keep it simple, always execute + confirm.

2. **JSON output mode?** For scripting, `clocktray status --json` could output `{"visible": true}`. **Recommendation:** Defer. Not requested, easy to add later. YAGNI.

3. **Version bump?** Adding CLI capability is a minor feature. Suggest `0.7.0` (from current `0.6.0`). **Recommendation:** Mac or Bruno bumps version when merging.

---

## 9. Implementation Order

1. **Anna** creates `CliHandler.cs` with all methods + tests
2. **Anna** modifies `Program.cs` to add the args dispatch (as shown in skeleton)
3. **Mac** reviews integration, ensures GUI mode unaffected
4. **Dutch** final review before merge

Estimated effort: **2-3 hours** for Anna (including tests), **30 min** for Mac review.
