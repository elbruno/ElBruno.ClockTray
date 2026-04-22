# ClockTray CLI Reference

A quick reference for all ClockTray command-line commands and options.

## Installation

ClockTray is installed as a .NET global tool:

```bash
dotnet tool install --global ElBruno.ClockTray
```

After installation, the `clocktray` command is available from any terminal or command prompt.

### Update

To update to the latest version:

```bash
dotnet tool update --global ElBruno.ClockTray
```

### Uninstall

To remove ClockTray:

```bash
dotnet tool uninstall --global ElBruno.ClockTray
```

---

## Commands

### `clocktray` (no arguments)

Launches the ClockTray system tray application. The app minimizes to the system tray and remains active in the background.

**Usage:**
```bash
clocktray
```

**Behavior:**
- Starts the NotifyIcon in the Windows system tray
- Right-click menu provides Show/Hide and Exit options
- Global hotkey `Ctrl+Alt+T` activates the toggle
- App persists in tray until explicitly closed

---

### `clocktray show`

Shows the date and time in the Windows taskbar (if currently hidden).

**Usage:**
```bash
clocktray show
```

**Expected Output:**
```
Clock visibility changed to: Visible
```

**Exit Codes:**
- `0` — Success; clock is now visible
- `1` — Error (registry access denied or system API failure)

**Behavior:**
- On **Windows 11 23H2+**: Instantly applies via `WM_SETTINGCHANGE` broadcast (no Explorer restart)
- On **Windows 10 / older Windows 11**: Restarts Windows Explorer (brief screen flicker)
- If already visible, command succeeds silently

---

### `clocktray hide`

Hides the date and time in the Windows taskbar.

**Usage:**
```bash
clocktray hide
```

**Expected Output:**
```
Clock visibility changed to: Hidden
```

**Exit Codes:**
- `0` — Success; clock is now hidden
- `1` — Error (registry access denied or system API failure)

**Behavior:**
- On **Windows 11 23H2+**: Instantly applies via `WM_SETTINGCHANGE` broadcast (no Explorer restart)
- On **Windows 10 / older Windows 11**: Restarts Windows Explorer (brief screen flicker)
- If already hidden, command succeeds silently

---

### `clocktray status`

Reports the current visibility state of the taskbar clock.

**Usage:**
```bash
clocktray status
```

**Expected Output (if visible):**
```
Clock visibility: Visible
```

**Expected Output (if hidden):**
```
Clock visibility: Hidden
```

**Exit Codes:**
- `0` — Success; status reported
- `1` — Error (registry read failed)

**Behavior:**
- Reads current registry state without making changes
- Safe to run from scripts; provides machine-readable output

---

### `clocktray --help`

Displays help information and lists all available commands.

**Usage:**
```bash
clocktray --help
clocktray -h
```

**Expected Output:**
```
ClockTray - Windows Taskbar Clock Toggle

Usage:
  clocktray [command]

Commands:
  show      Show the taskbar date/time
  hide      Hide the taskbar date/time
  status    Display current clock visibility
  --help    Show this help message

Examples:
  clocktray               Launch the system tray app
  clocktray show          Make the clock visible
  clocktray hide          Hide the clock
  clocktray status        Check if clock is visible
```

---

## Usage Examples

### Example 1: Launch the Tray App

```bash
$ clocktray
```

The app starts minimized in the system tray. Press `Ctrl+Alt+T` anywhere to toggle, or right-click the tray icon.

### Example 2: Show the Clock

```bash
$ clocktray show
Clock visibility changed to: Visible
```

The taskbar clock is now visible. On modern Windows 11, this is instant. On Windows 10 or older Windows 11, Explorer briefly restarts.

### Example 3: Check Status and Then Hide

```bash
$ clocktray status
Clock visibility: Visible

$ clocktray hide
Clock visibility changed to: Hidden

$ clocktray status
Clock visibility: Hidden
```

### Example 4: Use in a Script

```batch
@echo off
REM Hide clock before presentation
clocktray hide
if %ERRORLEVEL% neq 0 (
    echo Failed to hide clock
    exit /b 1
)

REM Launch presentation
start PowerPoint.exe "my_presentation.pptx"

REM Show clock when done
clocktray show
```

### Example 5: Use in PowerShell

```powershell
# Check current state
$status = & clocktray status
Write-Host "Current status: $status"

# Hide clock
& clocktray hide
if ($LASTEXITCODE -eq 0) {
    Write-Host "Clock hidden successfully"
} else {
    Write-Host "Failed to hide clock"
}
```

---

## Common Use Cases

### Presentations & Streaming

Hide the clock before going live to create a cleaner, distraction-free screen:

```bash
clocktray hide
# ... present or stream ...
clocktray show
```

Use in a startup script to hide the clock automatically when launching presentation software.

### System Monitoring & Automation

Query clock state in a monitoring script:

```bash
clocktray status
```

Combine with task schedulers to toggle the clock based on time of day or calendar events.

### Accessibility & Focus

Hide the taskbar clock to reduce visual clutter and distractions while working.

### Multi-Monitor Setups

Hide the clock on your primary monitor via CLI commands; the tray app continues to show the clock locally if needed.

---

## Exit Codes

| Code | Meaning | Typical Cause |
|------|---------|---------------|
| `0` | Success | Command completed as expected |
| `1` | Error | Registry access denied, API failure, or Windows version incompatibility |

---

## Troubleshooting

### Clock Changes Don't Persist

**Problem:** After running `clocktray show` or `clocktray hide`, the clock reverts to its previous state.

**Solution:**
1. Ensure you have administrator privileges (required to modify registry)
2. Check Windows version: `winver` to confirm Windows 10 or 11
3. If on Windows 10, a brief Explorer restart should occur; if you don't see it, manually restart Explorer via Task Manager

### Registry Access Denied

**Problem:** Running `clocktray show` / `clocktray hide` returns exit code `1`.

**Solution:**
1. Run the command in an elevated (administrator) command prompt
2. Check that your user account has write permissions to `HKEY_CURRENT_USER` registry hive
3. Verify that Group Policy does not prevent registry modifications

### Command Not Found

**Problem:** `clocktray` command is not recognized.

**Solution:**
1. Verify installation: `dotnet tool list --global | findstr clocktray`
2. If not listed, reinstall: `dotnet tool install --global ElBruno.ClockTray`
3. Close and reopen your terminal/command prompt for PATH changes to take effect
4. On Windows, verify that `.NET global tools` location is in your PATH (typically `%USERPROFILE%\.dotnet\tools`)

### Explorer Restart on Windows 10

**Problem:** Running `show`/`hide` causes Explorer to briefly restart and close open file explorer windows.

**Solution:**
1. This is expected behavior on Windows 10 and older Windows 11 builds (before 23H2)
2. Consider updating to Windows 11 23H2+ for seamless, instant changes (no Explorer restart)
3. Save work in open applications before running the command
4. Provide clear communication to users before automating in scripts

### Clock Changes Don't Apply Immediately

**Problem:** After running a command, the clock visibility doesn't update until I manually toggle something.

**Solution:**
1. On Windows 11 23H2+, changes should be instant
2. On older Windows, Explorer restart is required (may take 2-3 seconds)
3. If the clock still doesn't update after 5 seconds, try restarting Explorer manually via Task Manager

---

## Technical Details

### Registry Paths

ClockTray uses the following registry paths depending on Windows version:

**Windows 11 23H2+ (Modern Method):**
```
HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced
  ShowSystrayDateTimeValueName (DWORD): 1 = visible, 0 = hidden
```

Broadcasts `WM_SETTINGCHANGE` message to apply changes instantly.

**Windows 10 / Older Windows 11 (Legacy Method):**
```
HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer
  HideClock (DWORD): 1 = hidden, 0 = visible
```

Restarts Windows Explorer (`explorer.exe`) to apply changes.

### Global Hotkey

The system tray app (launched with `clocktray` with no arguments) registers a global hotkey:

- **Ctrl+Alt+T** — Toggles clock visibility from anywhere in Windows

To use the hotkey, the app must be running in the system tray. The CLI commands (`show`, `hide`, `status`) do not require the tray app to be active.

---

## More Information

- **GitHub:** [elbruno/ElBruno.ClockTray](https://github.com/elbruno/ElBruno.ClockTray)
- **NuGet Package:** [ElBruno.ClockTray](https://www.nuget.org/packages/ElBruno.ClockTray)
- **User Guide:** See [README.md](../README.md) for system tray app usage and Chinese calendar overlay features
