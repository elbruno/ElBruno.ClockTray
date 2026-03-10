# ClockTray User Guide — Table of Contents & Outline

## Document Structure

This outline defines the complete user guide for ClockTray. Each section includes purpose, scope, and estimated length.

---

## 1. Getting Started (2–3 pages)

**Purpose:** Help new users install, enable, and use ClockTray immediately  
**Audience:** First-time users, power users new to PowerToys

**Subsections:**
- **Installation** (~½ page)
  - Where to get ClockTray (PowerToys installer, direct download)
  - System requirements (Windows 10 20H2+, Windows 11)
  - Installation troubleshooting (common installer issues)

- **Enabling ClockTray** (~½ page)
  - Open PowerToys Settings
  - Locate ClockTray in the utility list
  - Toggle "Enable ClockTray" on
  - Quick visual confirmation (icon appears in system tray)

- **Your First Toggle** (~½ page)
  - Default hotkey: Ctrl+Alt+T
  - What happens when you press it (clock disappears/reappears)
  - Visual feedback (icon changes state in system tray)

- **Quick Settings Overview** (~1 page)
  - Where to find ClockTray settings in PowerToys
  - Main toggle (enable/disable entire utility)
  - Hotkey customization
  - What settings persist (user preferences saved automatically)

---

## 2. Features & How to Use Them (2–3 pages)

**Purpose:** Explore each capability of ClockTray in detail  
**Audience:** All users, reference material

**Subsections:**
- **Clock Visibility Toggle** (~1 page)
  - How the toggle works (registry modification + system notification)
  - What "visibility" means (taskbar date/time disappears/reappears)
  - Works with or without taskbar visible
  - Behavior on dual/multi-monitor setups (affects all monitors)

- **Hotkey Customization** (~¾ page)
  - Default hotkey: Ctrl+Alt+T
  - How to change your hotkey (hotkey picker in settings)
  - Supported key combinations (Ctrl, Shift, Alt modifiers + key)
  - Keyboard shortcuts guidelines (avoiding conflicts with Windows/apps)
  - Testing your new hotkey

- **System Tray Icon** (~½ page)
  - Icon location (system tray, bottom-right of taskbar)
  - Icon states (enabled/disabled, clock visible/hidden)
  - Right-click context menu (quick access to settings, exit)
  - Customizing icon appearance (future feature — noted as roadmap)

- **Automatic Clock Detection** (~½ page)
  - ClockTray auto-detects OS version (Win10 vs. Win11)
  - Chooses optimal registry method automatically
  - User doesn't need to worry about "Modern" vs. "Legacy" paths
  - Edge case: Building/rebuild triggers auto-detection

---

## 3. Settings Panel Reference (1–2 pages)

**Purpose:** Detailed walkthrough of every setting and option  
**Audience:** Users wanting to customize beyond defaults

**Subsections:**
- **Main Toggle (Enable/Disable)** (~¼ page)
  - Turns the entire utility on/off
  - Settings preserved when disabled (re-enable picks up same config)

- **Hotkey Picker** (~¾ page)
  - Visual hotkey selector in settings
  - How to select modifiers (Ctrl, Shift, Alt checkboxes)
  - How to select base key (dropdown or keyboard binding)
  - Conflict detection (warns if hotkey used by Windows or another app)
  - Testing button (press button → try new hotkey immediately)

- **Clock State Display** (~¼ page)
  - Info box showing current clock visibility (On/Off)
  - Shows whether modern or legacy registry path was used
  - Helpful for diagnostics if things aren't working as expected

- **Start with Windows** (~¼ page)
  - Option to auto-launch ClockTray on boot
  - Useful for power users who want it always active
  - Default: Off (user chooses whether to enable)

---

## 4. Keyboard Shortcuts & Hotkeys (1 page)

**Purpose:** Quick reference for all keyboard shortcuts  
**Audience:** Keyboard-first users, reference

**Content:**
- **ClockTray Global Hotkey**
  - Default: Ctrl+Alt+T
  - Function: Toggle taskbar clock visibility on/off
  - Works from any window or desktop

- **Changing Your Hotkey**
  - Open PowerToys Settings
  - Go to ClockTray
  - Click "Hotkey Picker"
  - Select modifiers + key
  - Click "Test" to verify immediately

- **Common Hotkey Combinations** (suggestions)
  - Ctrl+Alt+C (like "Clock")
  - Ctrl+Shift+T (like "Toggle Time")
  - Win+T (if not conflicting with built-in Ctrl+Alt+T behavior)

- **Troubleshooting Hotkey Issues**
  - Hotkey not working? See [Troubleshooting](#troubleshooting)

---

## 5. Troubleshooting (2–3 pages)

**Purpose:** Solve common problems users encounter  
**Audience:** Users experiencing issues

**Subsections:**
- **ClockTray Won't Enable**
  - Check Windows version (requires Win10 20H2 or later)
  - Restart PowerToys application
  - Verify admin privileges (may require elevation on some systems)
  - Check Event Viewer for errors (advanced)

- **Hotkey Not Working**
  - Is ClockTray enabled in PowerToys Settings?
  - Is the hotkey bound to another application? (check conflict in settings)
  - Restart PowerToys or Windows Explorer
  - Try a different hotkey to isolate the problem

- **Clock Won't Toggle (Stays Visible/Hidden)**
  - Close and reopen ClockTray
  - Restart Windows Explorer: `Ctrl+Shift+Esc` → right-click "Windows Explorer" → "Restart"
  - Check Event Viewer → Windows Logs → System for errors
  - Verify user has permission to modify registry (HKEY_CURRENT_USER paths)

- **System Tray Icon Missing**
  - Icon may be hidden in system tray overflow
  - Drag icon out of overflow onto visible taskbar
  - Restart PowerToys application
  - Check Settings > System > Notification area → "Select which icons appear on the taskbar"

- **ClockTray Stops Working After Windows Update**
  - Microsoft may change registry paths or notification mechanisms
  - Check if Windows version jumped (e.g., 22H2 → 23H2)
  - ClockTray attempts auto-detection; restart PowerToys to re-detect
  - Report issue if problem persists (see [Support](#support))

- **Unexpected Clock Behavior on Dual Monitors**
  - ClockTray toggles clock on *all* displays (system-wide behavior)
  - Toggling clock on main monitor affects all monitors
  - This is expected; individual monitor control not currently supported

- **Performance Issues**
  - ClockTray is lightweight and has no known performance impact
  - If slowdowns occur, disable ClockTray and monitor system
  - Report if issue persists

---

## 6. Frequently Asked Questions (FAQ) (1–2 pages)

**Purpose:** Answer common questions not covered in other sections  
**Audience:** All users

**Q&A Format:**

**Q: Why would I need to hide the taskbar clock?**  
A: Different use case for different people. Some prefer full-screen apps without distractions. Others use high-res displays where taskbar space is premium. Power users love one-click control instead of buried Settings menus.

**Q: Does ClockTray affect system performance?**  
A: No. ClockTray is lightweight, runs in the background, and only activates when you press the hotkey. Zero noticeable impact.

**Q: Will ClockTray work if I use a custom taskbar position or theme?**  
A: Yes. ClockTray controls the system clock via Windows registry and notifications—not visual customization. Works with all taskbar positions and themes.

**Q: Can I customize the hotkey?**  
A: Absolutely. Go to PowerToys Settings → ClockTray → Hotkey Picker and choose your combination.

**Q: What if my hotkey conflicts with another application?**  
A: ClockTray warns you about conflicts. Choose a different hotkey. [Settings Panel Reference](#3-settings-panel-reference) has suggestions.

**Q: Does ClockTray work on Windows 10?**  
A: Yes, Windows 10 20H2 and later. Earlier builds are not supported.

**Q: What happens to my clock when I hide it?**  
A: It's hidden from the taskbar display only. Windows still tracks time internally; your system clock isn't affected. Press the hotkey again to show it.

**Q: Can I hide the ClockTray system tray icon?**  
A: Not currently, but it's on the roadmap. For now, you can minimize it to the system tray overflow.

**Q: Is ClockTray open source?**  
A: Yes. ClockTray is part of Microsoft PowerToys (open-source). Source code available on GitHub.

**Q: How do I report a bug?**  
A: See [Support & Feedback](#support--feedback) section.

---

## 7. Support & Feedback (½ page)

**Purpose:** Direct users to help resources and feedback channels  
**Audience:** Users needing help or wanting to contribute

**Content:**
- **Getting Help**
  - PowerToys GitHub Issues: https://github.com/microsoft/PowerToys/issues
  - PowerToys Discussions: https://github.com/microsoft/PowerToys/discussions
  - ClockTray-specific: Tag issues with `clocktray`

- **Reporting Bugs**
  - Include: Windows version, build number, steps to reproduce
  - Attach: Event Viewer logs if available
  - Example: "Windows 11 23H2, ClockTray v1.0, toggle not working after reboot"

- **Requesting Features**
  - Use GitHub Discussions for feature requests
  - Vote on existing requests
  - Example: "Custom per-monitor clock toggling" or "Clock visibility schedule"

- **Contributing to ClockTray**
  - Development guide: [see devdocs/clocktray-dev.md](../../doc/devdocs/clocktray-dev.md)
  - Pull requests welcome (follow PowerToys contribution guidelines)

---

## Appendix (Optional, ½–1 page each)

- **Appendix A: Registry Paths Reference**
  - Technical details of registry modification (for power users / advanced debugging)
  - Modern path: `HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced`
  - Legacy path: `HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer`
  - Note: ClockTray handles these automatically; listed for reference only

- **Appendix B: Windows Version Matrix**
  - Compatibility table (Windows 10 20H2, Win11 21H2, 22H2, 23H2, etc.)
  - Behavior differences (if any)

- **Appendix C: Accessibility Notes**
  - Keyboard-only operation (ClockTray is fully keyboard-accessible)
  - Screen reader compatibility (system tray icons are accessible)
  - High-contrast mode support

---

## Metadata

| Property | Value |
|----------|-------|
| **Document Type** | User Guide (Outline) |
| **Target Audience** | Windows power users, daily users, accessibility-focused users |
| **Estimated Total Length** | 8–12 pages (prose + screenshots) |
| **Screenshots Required** | ~8–10 (settings panel, hotkey picker, system tray, toggles) |
| **Review Checklist** | Tech accuracy, clarity for non-technical audience, completeness |
| **Maintenance** | Update on major releases or registry changes (Win11 updates) |

---

**Next:** Once this outline is approved, each section will be expanded into full prose with screenshots and examples.
