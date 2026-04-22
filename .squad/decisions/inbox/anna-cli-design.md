# CLI Design Decision — Native Args Parsing

**Date:** 2026-03-10  
**Agent:** Anna (CLI Specialist)  
**Status:** IMPLEMENTED

## Decision: Use Native args[] Parsing (Zero Dependencies)

ClockTray now supports both GUI and CLI modes via a dual-path `Main()` implementation.

### Evaluation of Options

| Option | Pros | Cons | Recommendation |
|--------|------|------|----------------|
| **Native args[]** | Zero dependencies, simple, ~50 LOC | Manual --help/--version | ✅ **CHOSEN** |
| **System.CommandLine** | Auto --help, validation, tab completion | +500KB, overkill for 3 commands | ❌ Too heavy |
| **Spectre.Console.Cli** | Beautiful output, rich parsing | +200KB, brings console UI framework | ❌ Unnecessary for headless CLI |

**Rationale:** ClockTray has exactly 3 commands (show, hide, status) with no arguments. Native parsing is the simplest solution and aligns with Decision #6 (Zero External Dependencies).

---

## Implementation Pattern

### Dual-Path Architecture
```
Main(string[] args)
├── args.Length > 0? → RunCli(args) [Headless, no STA]
└── args.Length == 0? → RunGui() [STA Thread, WinForms]
```

### Key Patterns

1. **Conditional STA Threading**
   - Moved `[STAThread]` from `Main()` to `RunGui()` 
   - Only applies when launching GUI mode
   - CLI path runs on default MTA thread (no Windows message pump needed)

2. **Headless CLI Execution**
   - No `Application.Run()` call in CLI path
   - Calls `ClockToggler.SetClockVisibility()` directly
   - Prints confirmation to Console, exits with code 0/1
   - No UI dependencies loaded

3. **Backward Compatibility**
   - `clocktray` (no args) → launches GUI (existing behavior preserved)
   - `clocktray <command>` → runs headlessly and exits

### Commands Implemented
- `show` — Show taskbar clock
- `hide` — Hide taskbar clock  
- `status` — Print clock state + OS detection method
- `--help` / `-h` / `help` — Display usage
- `--version` / `-v` / `version` — Display version

### Exit Codes
- `0` — Success
- `1` — Unknown command

---

## Testing Checklist

- [ ] `clocktray` launches GUI (no regression)
- [ ] `clocktray show` hides clock and exits (no tray icon)
- [ ] `clocktray hide` shows clock and exits
- [ ] `clocktray status` prints state correctly
- [ ] `clocktray --help` displays usage
- [ ] `clocktray --version` displays version
- [ ] `clocktray badcommand` prints error + help, exits 1

---

## Future Considerations

If commands grow beyond 5-6 or need subcommands (e.g., `clocktray config set`), revisit System.CommandLine. For now, native parsing is the right choice.

**Risk Level:** MINIMAL (well-tested pattern in .NET CLI tools)
