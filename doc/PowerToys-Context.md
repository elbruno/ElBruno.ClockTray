# PowerToys Contribution Context for ClockTray

## Understanding PowerToys: Philosophy & Mission

PowerToys is Microsoft's collection of utilities designed to help Windows power users accomplish more in less time. Rather than modifying Windows itself, PowerToys provides optional, lightweight utilities that respect user choice and system performance.

### Core Philosophy
- **"Remove friction"** — Solve real problems power users face daily
- **"Empower control"** — Give users agency over their system
- **"Lightweight"** — No bloat, no dependencies, minimal overhead
- **"Windows-first"** — Leverage native Windows APIs efficiently
- **"Open source"** — Community-driven, auditable, community-improved

### Why PowerToys Matters
PowerToys succeeds because it stays focused: each utility does one thing well. Features are discoverable, optional, and don't interfere with core Windows. Users control whether to install, enable, or uninstall without friction.

---

## Why ClockTray Fits PowerToys

ClockTray aligns with PowerToys' philosophy and market position in four ways:

### 1. **Solves a Real, Recurring Problem**
Power users frequently want to reclaim taskbar screen real estate or reduce on-screen distractions. Yet hiding the taskbar clock requires navigating Settings → Personalization → Taskbar (3 levels deep, buried). ClockTray removes that friction with a single hotkey.

**PowerToys precedent:** Color Picker (one-click color sampling), Awake (prevent sleep), FancyZones (window management)—all solve specific power user needs without reinventing the OS.

### 2. **Minimal Scope, Maximum Polish**
ClockTray does one thing: toggle the taskbar clock. No configuration complexity, no feature bloat. The entire core logic is ~150 lines of code across registry modification + hotkey binding. This makes it:
- Easy to maintain (low surface area for bugs)
- Fast to improve (quick iterations on edge cases)
- Trustworthy (auditable, transparent behavior)

**PowerToys precedent:** Utilities like Hosts (text editor) and ClipboardManager (clipboard history) exemplify "do one thing, do it well."

### 3. **No Duplication in Existing Utilities**
PowerToys has no existing utility that manages taskbar clock visibility. This fills a gap without stepping on other utilities' toes. Conversely, ClockTray doesn't conflict with FancyZones, Color Picker, or other modules.

### 4. **Empowers User Control**
The spirit of PowerToys is: "This is *your* computer. Take control." ClockTray embodies this by giving users instant, reversible control over a system element they can't easily toggle. No permanent changes, no damage if something goes wrong.

---

## PowerToys Contribution Path

The PowerToys team welcomes contributions in order of priority:

```
1. Tech Spec ← NEW UTILITIES START HERE
   ↓
2. Design Concept (UX mockups, recommendations)
   ↓
3. Documentation (user guides, contribution guides)
   ↓
4. Bug Fixes (existing utilities)
   ↓
5. Features (enhancements to existing utilities)
   ↓
6. New Utility (NEW POWER TOY) ← OUR TARGET
```

**Why this order?** The team prevents duplicated effort, validates ideas early, and ensures each contribution matches PowerToys standards before code is written. A well-reasoned tech spec can save weeks of rework.

---

## ClockTray's Contribution Journey

### Phase 1: C# MVP ✅ COMPLETE
- **Status:** Proof of concept delivered
- **Output:** Working Windows application (tray icon + hotkey toggle)
- **Purpose:** Validate the idea, test both Win10 (legacy) and Win11 (modern) registry paths
- **Timeline:** Week 1 of project

### Phase 2: C++ Module + PowerToys Integration 🎬 IN PROGRESS
**Goal:** Convert Phase 1 MVP into an official PowerToys utility module  
**Timeline:** 5 weeks (Sprints 1–4)

#### Sprint 1: Tech Spec & Design (Week 1)
- Write comprehensive tech spec (Microsoft template)
- Design WinUI 3 settings panel UI
- Research PowerToys module examples (Color Picker, FancyZones, Awake)
- Validate concept with PowerToys team (GitHub discussion)

**Deliverables:**
- `doc/ClockTray-TechSpec.md` — Technical specification (1500–2000 words)
- UI mockups/XAML layout
- Architecture diagram

#### Sprint 2: Core C++ Module (Weeks 2–3)
- Set up C++ project structure (PowerToys template)
- Implement `PowertoyModuleIface` interface (factory pattern)
- Port Win32 registry logic + hotkey handling
- Unit tests (Win10/Win11 paths)

**Deliverables:**
- Compilable C++ module with >80% test coverage
- Core toggle functionality working

#### Sprint 3: Settings UI (Week 4)
- Implement WinUI 3 settings panel
- Hotkey customization widget
- Config persistence (JSON ↔ module state)
- Integration tests

**Deliverables:**
- Settings panel integrated into PowerToys settings app
- E2E hotkey customization working

#### Sprint 4: Testing & Submission (Week 5)
- Comprehensive OS/build matrix testing (Win10 20H2 → Win11 23H2)
- Edge case testing (Explorer restart, rapid toggles, etc.)
- User documentation + developer documentation
- PowerToys contributor checklist validation
- Submit PR to Microsoft/PowerToys

**Deliverables:**
- Full test matrix + results
- User guide + dev guide
- PR opened to PowerToys repository

---

## PowerToys Module Architecture: What It Means for ClockTray

PowerToys modules follow a plugin architecture: each utility is a DLL that implements a standard interface (`PowertoyModuleIface`). The PowerToys host application loads, configures, and manages these modules.

### Module Integration Points

**1. Core Interface (C++)**
Every PowerToys module exports these functions:
```cpp
// Factory: Create module instance
PowertoyModule* __stdcall powertoy_create();

// Lifecycle: Enable, disable, query state
void powertoy_destroy(PowertoyModule* module);
void enable(bool enabled);
bool is_enabled();
bool is_system_enabled();

// Configuration: Receive settings from UI, report current settings
void send_config_json(const wchar_t* json);
const wchar_t* get_config();

// Utilities: Name, icon, documentation
const wchar_t* get_name();
bool is_elevated();
intptr_t signal_event(const wchar_t* name, intptr_t data);
```

**For ClockTray:**
- `enable(true)` → Register hotkey listener
- `enable(false)` → Unregister hotkey listener
- `send_config_json()` → Parse hotkey from JSON, update registry method preference
- `get_config()` → Return current settings as JSON
- Hotkey press triggers internal toggle logic (Win32 message)

**2. Settings UI (WinUI 3)**
PowerToys hosts a centralized Settings application. Each module provides a settings page (XAML + code-behind). The page:
- Displays the module's current state
- Allows configuration (toggle, hotkey picker, options)
- Sends config changes back to the module via JSON

**For ClockTray:**
- Settings page shows: "ClockTray" toggle, hotkey picker, clock visibility status
- When user presses a different hotkey, page calls `send_config_json()` with new binding
- Status updates show current clock state (on/off) for diagnostics

**3. Registry & State Persistence**
The module manages internal state (clock visible/hidden) and persists it when appropriate. PowerToys also persists module configuration (hotkey preference) in its settings database.

**For ClockTray:**
- Internal state: Clock is visible/hidden (managed via registry toggle)
- Persistent config: User's hotkey preference (managed by PowerToys settings)
- On launch: Restore user's hotkey; detect clock's current state

### Module File Structure (PowerToys Template)
```
src/modules/ClockTray/
├── CMakeLists.txt              (build config)
├── dll/
│   ├── ClockTray.cpp           (module impl + interface)
│   ├── ClockTray.h
│   ├── ClockToggler.cpp        (registry + WM_SETTINGCHANGE logic)
│   ├── ClockToggler.h
│   ├── HotkeyWindow.cpp        (Win32 message window)
│   └── HotkeyWindow.h
├── WinUI3/
│   ├── ClockTraySettings.xaml  (settings panel UI)
│   ├── ClockTraySettings.xaml.cpp
│   └── ClockTraySettings.xaml.h
└── tests/
    ├── ClockTrayTests.cpp
    └── CMakeLists.txt
```

---

## PowerToys Standards & Review Criteria

When submitting ClockTray to PowerToys, the team expects:

### Code Quality
- **Language:** Modern C++ (C++17 minimum, C++20 preferred)
- **Compiler:** MSVC with `/W4` warnings (no warnings in final code)
- **Style:** Follow PowerToys C++ style guide (see repo)
- **Dependencies:** Zero external; use only Windows APIs + PowerToys framework
- **Documentation:** Comments explain *why*, not *what*; public APIs fully documented

### Architecture
- **Module pattern:** Implement `PowertoyModuleIface` correctly
- **State management:** Thread-safe where applicable
- **Error handling:** Robust logging, graceful degradation on failure
- **Performance:** No blocking I/O on UI thread; minimal CPU/memory overhead

### Testing
- **Coverage:** >80% line coverage recommended
- **Scope:** Unit tests for core logic (registry, hotkey)
- **OS Matrix:** Test on Win10 20H2, Win11 21H2/22H2/23H2
- **Edge cases:** Rapid toggles, Explorer restarts, registry permission errors

### UX/UI
- **Settings panel:** Consistent with PowerToys design system
- **Clarity:** Settings terminology matches Windows conventions
- **Accessibility:** Keyboard navigation, screen reader support, high-contrast mode
- **Feedback:** User knows when toggle succeeded/failed (status messages)

### Documentation
- **Tech spec:** Problem statement, solution, architecture, rationale (1500–2000 words)
- **User guide:** Getting started, feature walkthrough, troubleshooting (for average users)
- **Dev guide:** Architecture, build instructions, testing, contribution guidelines
- **PR description:** Clear problem statement, scope, testing, compatibility notes

### Contribution Process
1. **Open discussion** (GitHub Discussions) to validate idea early
2. **Submit tech spec** for review + feedback
3. **Submit design concept** (UI mockups) for review
4. **Submit PR** with working code + tests + documentation
5. **Iterate** based on team feedback
6. **Merge** once approved

---

## Why PowerToys Over Standalone App?

PowerToys offers advantages for ClockTray:

| Aspect | Standalone App | PowerToys Module |
|--------|---|---|
| **Installation** | Separate installer, updates | Bundled with PowerToys; managed updates |
| **Discoverability** | User must find it | Featured in PowerToys settings |
| **Trust** | Unknown publisher | Microsoft-published, open source |
| **Integration** | Isolated | Shares settings UI, follows standards |
| **Audience** | General users | Power users (PowerToys' core demographic) |
| **Maintenance** | Individual burden | Community + Microsoft stewardship |

**For Bruno & the team:** PowerToys submission validates the idea with Microsoft's backing. It reaches the exact target audience (power users) and ensures long-term maintenance.

---

## Team Expectations & Handoff

### PowerToys Team Review Cycle
Expect:
- **Tech spec review:** 1–2 weeks (feedback on architecture, scope)
- **PR review:** 1–3 weeks (code review, testing feedback)
- **Iteration cycle:** 2–5 rounds of feedback common for new modules
- **Approval criteria:** Code quality, documentation completeness, test coverage, design fit

### Success Indicators
✅ PR approved and merged to PowerToys/main  
✅ ClockTray appears in PowerToys 1.x+ release  
✅ Community can install via standard PowerToys installer  
✅ Documentation accessible in PowerToys official docs  

---

## Next Steps for ClockTray

1. **Immediate (Sprint 1, this week):**
   - Dutch writes tech spec (validation + architecture)
   - Molly reviews spec for clarity + completeness
   - Blain designs WinUI 3 settings panel

2. **Week 2:**
   - Open GitHub discussion on PowerToys repo (share tech spec, get early feedback)
   - PowerToys team provides guidance on module integration points

3. **Weeks 2–5:**
   - Mac & Dillon implement C++ module (core logic + tests)
   - Blain implements settings UI
   - Molly writes user guide + dev guide in parallel
   - Dutch coordinates with PowerToys team on any blockers

4. **Week 5 (Submission):**
   - Final testing + documentation review
   - Open PR to PowerToys/main
   - Respond to feedback in real-time

---

## Resources & References

**PowerToys Official**
- Contribution Guide: https://github.com/microsoft/PowerToys/blob/main/CONTRIBUTING.md
- Developer Docs: https://github.com/microsoft/PowerToys/tree/main/doc/devdocs
- Module Examples:
  - Color Picker: `src/modules/ColorPicker/`
  - Awake: `src/modules/Awake/`
  - FancyZones: `src/modules/FancyZones/`

**Reference Architecture**
- PowertoyModuleIface: `src/common/interop/PowertoyModuleIface.h`
- Settings pattern: Study Color Picker's WinUI 3 settings (example of modern module)

**PowerToys Design System**
- WinUI 3 patterns (follow existing settings pages)
- Accessibility guidelines (WCAG 2.1 AA minimum)
- Localization (prepare for multi-language support)

---

## Final Thought

ClockTray represents exactly what PowerToys is designed for: solving a power user problem with minimal complexity, maximum reliability, and respect for system resources. By contributing to PowerToys rather than distributing standalone, we're validating that philosophy and building something the community can trust and maintain together.

**ClockTray → PowerToys is a win for:** Bruno (official validation), power users (trusted official utility), and the Windows ecosystem (more useful tools).
