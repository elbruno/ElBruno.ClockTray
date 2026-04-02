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

---

## Sprint 1: Specification & Design (2026-03-10)

### 4. Four-Class Module Architecture (Dutch)
**Date:** 2026-03-10  
**Agent:** Dutch (Architecture)  
**Status:** PROPOSED (awaiting team review)

**Decision:** Implement ClockTray as four focused C++ classes:
1. **ClockTray** — Module lifecycle, PowertoyModuleIface implementation
2. **RegistryToggler** — Registry manipulation (modern & legacy paths)
3. **HotkeyHandler** — Win32 global hotkey registration, message dispatch
4. **ConfigManager** — JSON settings serialization, state persistence

**Rationale:**
- **Separation of Concerns:** Each class has single responsibility (easier to test, maintain, extend)
- **Testability:** Mocking one layer doesn't require OS interaction
- **Reusability:** Patterns can be extracted for other PowerToys modules
- **Precedent:** Color Picker, Awake follow similar layered patterns

**Alternatives:** Monolithic class (simpler initially, hard to test); 6+ classes (over-engineered for 800 LOC)  
**Risk Level:** LOW (pattern proven in existing PowerToys modules)

---

### 5. Runtime OS Version Detection (Dutch)
**Date:** 2026-03-10  
**Agent:** Dutch (Architecture)  
**Status:** PROPOSED

**Decision:** Detect OS version at runtime via `RtlGetVersion`, then choose modern or legacy path dynamically (not compile-time branching).

**Rationale:**
- **Portability:** Single binary works on all OS versions (no "Win10 version" vs "Win11 version")
- **User Transparency:** Auto-selects best path; user doesn't configure
- **Future-Proof:** Windows 12+ can adopt modern path automatically
- **Maintenance:** No separate builds to maintain

**Risk Level:** LOW (build number queries are stable)

---

### 6. Zero External Dependencies (Dutch)
**Date:** 2026-03-10  
**Agent:** Dutch (Architecture)  
**Status:** PROPOSED

**Decision:** Use ONLY C++ Standard Library 20 + Windows SDK. No NuGet packages, no vcpkg dependencies.

**Files Allowed:** C++ standard (`<memory>`, `<string>`, `<vector>`, etc.), Windows SDK (`<windows.h>`, `<winreg.h>`), PowerToys utilities (`Logger.h`, `JsonHelpers.h`)

**Files NOT Allowed:** nlohmann/json, Boost, fmt library (use C++20 std::format)

**Rationale:**
- **Lightweight:** No dependency management
- **Security:** Fewer third-party vulnerabilities
- **Precedent:** Color Picker, Awake follow same principle
- **Build Simplicity:** CMake doesn't need vcpkg

**Risk Level:** LOW (constraint is well-understood and proven feasible)

---

### 7. Comprehensive Unit Tests with Mocked Win32 APIs (Dutch)
**Date:** 2026-03-10  
**Agent:** Dutch (Architecture)  
**Status:** PROPOSED

**Decision:** Unit tests do NOT call real Win32 APIs or modify real registry. Mock all OS interactions.

**Test Coverage Target:** >80% code paths  
**Manual Regression:** Sprint 4 (human-executed on real Win10/Win11 builds)

**Rationale:**
- **No Registry Pollution:** Tests don't leave artifacts
- **Speed:** Mocked tests run in milliseconds
- **Repeatability:** No OS state dependency; tests are deterministic
- **CI/CD Safe:** Automated pipelines can run without Admin rights
- **Precedent:** PowerToys unit tests follow this pattern

**Risk Level:** LOW (mocking is standard practice)

---

### 8. Message-Only Window for Hotkey Dispatch (Dutch)
**Date:** 2026-03-10  
**Agent:** Dutch (Architecture)  
**Status:** PROPOSED

**Decision:** Create hidden message-only window (HWND with `WS_POPUP | WM_SKIP_TASKBAR`) to receive WM_HOTKEY messages from OS.

**Rationale:**
- **Windows Standard Pattern:** All Windows apps use this for global hotkeys
- **Non-Intrusive:** Window is completely hidden; no taskbar/Alt+Tab appearance
- **Message Loop Integration:** Fits naturally with PowerToys' event loop

**Alternatives:** Raw Win32 callback (not possible; Win32 requires window handle); Task Scheduler hooks (overly complex)

**Risk Level:** LOW (pattern well-established)

---

### 9. PowerToys Settings Integration (Dutch)
**Date:** 2026-03-10  
**Agent:** Dutch (Architecture)  
**Status:** PROPOSED

**Decision:** Settings stored in PowerToys' centralized settings JSON, accessed via PowerToys' `Settings` and `JsonHelpers` classes. No custom config UI.

**Config Location:** `C:\Users\<user>\AppData\Local\Microsoft\PowerToys\settings.json`  
**Config Section:** Added to JSON under `"ClockTray": { ... }`

**Rationale:**
- **Consistency:** Users see all PowerToys settings in one place
- **Maintenance Burden:** PowerToys core handles JSON lifecycle
- **Localization:** PowerToys handles translation; we provide `.resw` strings
- **Precedent:** All PowerToys modules follow this pattern

**Risk Level:** LOW (pattern is standard; integration examples available)

---

### 10. Timeline: 5 Weeks, 4 Sprints (Dutch)
**Date:** 2026-03-10  
**Agent:** Dutch (Architecture)  
**Status:** PROPOSED

**Decision:** Organize implementation into four focused sprints:
- **Sprint 1:** Week 1 — Tech spec + UI mockup (Dutch + Blain)
- **Sprint 2:** Weeks 2–3 — C++ core + unit tests (Mac + Dillon)
- **Sprint 3:** Week 4 — Settings UI + integration (Blain + Mac)
- **Sprint 4:** Week 5 — Testing + docs + submission (Dillon + Dutch)

**Rationale:**
- **Realistic Pace:** 2 weeks for C++ core achievable for experienced Win32 dev
- **Parallel Work:** Blain starts UI design while Dutch finalizes spec
- **Testing Last:** Concentrate testing in Sprint 4 for integration issues
- **Submission Ready:** PR ready by end of Sprint 4

**Alternatives:** 3-week sprint (too rushed); 8-week sprint (overkill for small module)

**Risk Level:** MEDIUM (depends on team availability; escalate if complexity exceeds expectations by end of Week 2)

---

### 11. Legacy Path: ShellExecute for Explorer Restart (Dutch)
**Date:** 2026-03-10  
**Agent:** Dutch (Architecture)  
**Status:** PROPOSED

**Decision:** Use `ShellExecuteW("explorer")` (not `CreateProcessW`) when restarting explorer.exe for legacy path.

**Rationale:**
- **Graceful Handling:** Respects user's shell preferences (Windows Terminal, etc.)
- **System Resilience:** More forgiving if explorer.exe is already starting
- **Precedent:** Windows updates use ShellExecute for explorer restart

**Risk Level:** LOW (approach proven in Windows utilities)

---

### 12. No Tray Icon State Indicator (Dutch)
**Date:** 2026-03-10  
**Agent:** Dutch (Architecture)  
**Status:** PROPOSED

**Decision:** Tray icon is static (clock icon always same appearance). Do NOT add visual indicator (e.g., "X" overlay) to show clock visible/hidden state.

**Rationale:**
- **Scope Control:** Icon design + multiple states = unnecessary MVP complexity
- **MVP Focus:** Feature works; visual polish can follow
- **Tooltip Fallback:** Can show state in tooltip if needed (future enhancement)

**Alternatives:** Icon + "X" overlay (2–3 days design/implementation); Animated icon (overkill, confusing)

**Risk Level:** LOW (can be added post-launch without breaking changes)

---

### 13. Hotkey Validation (UI-Level, Graceful Failure) (Dutch)
**Date:** 2026-03-10  
**Agent:** Dutch (Architecture)  
**Status:** PROPOSED

**Decision:** Settings UI validates hotkey binding against known-bad list (Ctrl+Alt+Del, Win+X, etc.), but does NOT prevent invalid bindings at OS level. Failures are graceful.

**Validation:**
- **Level 1 (UI Warning):** Known conflicts shown as warning
- **Level 2 (OS Failure):** If user ignores warning, hotkey registration fails on next module enable
- **Fallback:** Module remains enabled and functional via tray menu

**Rationale:**
- **User Agency:** Let users try custom hotkeys; fail gracefully
- **Simplicity:** No need to monitor OS hotkey registry constantly
- **Fallback Path:** Tray menu always works; hotkey is convenience, not requirement

**Risk Level:** LOW (graceful degradation is acceptable)

---

### 14. ToggleSwitch vs Checkbox for Master Control (Blain)
**Date:** 2026-03-10  
**Agent:** Blain (UI Designer)  
**Status:** READY FOR TEAM REVIEW

**Decision:** Use `ToggleSwitch` (WinUI 3) for enable/disable control.

**Rationale:**
- **Immediate Feedback:** Visual state change with sliding animation
- **PowerToys Convention:** All modules use ToggleSwitch (Awake, Color Picker)
- **Accessibility:** Built-in ARIA support; screen readers announce clearly
- **Familiar Pattern:** Power users recognize as standard Windows 11 component
- **Keyboard Support:** Space/Enter toggles; Tab navigates

**Alternative:** Checkbox (rejected — less visually distinct, less modern)

**Impact:** Module enable/disable is primary user action; control choice sets tone for entire panel

---

### 15. ComboBox vs RadioButtons for Registry Method (Blain)
**Date:** 2026-03-10  
**Agent:** Blain (UI Designer)  
**Status:** READY FOR TEAM REVIEW

**Decision:** Use `ComboBox` (dropdown) for selecting registry method (Auto, Modern, Legacy).

**Rationale:**
- **Cleaner Layout:** Takes minimal space; three radio buttons would dominate
- **Discoverability:** Dropdown suggests "choose one"
- **PowerToys Pattern:** FancyZones, Color Picker use ComboBox
- **Help Text:** Dynamic text updates based on selection

**Alternatives:** Radio Button Group (rejected — takes space); Buttons with toggle state (rejected — unclear which is active)

**Impact:** Method selection is power-user setting; ComboBox keeps it accessible but not overwhelming

---

### 16. Record Button for Hotkey Entry (Blain)
**Date:** 2026-03-10  
**Agent:** Blain (UI Designer)  
**Status:** READY FOR TEAM REVIEW

**Decision:** Use dedicated "Record Hotkey" button + read-only TextBox display for hotkey entry.

**Rationale:**
- **Prevents Typos:** TextBox would allow invalid entries; Record mode validates on key presses
- **Familiar Pattern:** OBS, gaming apps use similar "record hotkey" UI
- **Clear Intent:** Record button makes action obvious
- **Real-Time Validation:** Recording mode validates immediately
- **Read-Only Display:** Shows current hotkey; prevents accidental manual edits

**Alternative:** Plain TextBox (rejected — user could type "hello", "ctrl", wasting time on validation)

**Impact:** Hotkey entry is critical; Record button prevents typos and support requests

---

### 17. Expander for Status Display (Blain)
**Date:** 2026-03-10  
**Agent:** Blain (UI Designer)  
**Status:** READY FOR TEAM REVIEW

**Decision:** Use `Expander` (collapsed by default) for status display (OS build, current method, clock visibility).

**Rationale:**
- **Reduces Clutter:** Status info is diagnostic; most users don't need it
- **Power-User Feature:** Advanced users can expand to troubleshoot
- **PowerToys Pattern:** Awake module uses Expander for advanced options
- **Performance:** Polling registry only when Expander is open (CPU optimization)
- **Cognitive Load:** Typical users see 5 sections (not 8 with expanded); cleaner

**Alternatives:** Always-visible status (rejected — noise); Separate page/tab (rejected — adds friction)

**Impact:** Keeps UI clean; respects user's primary goal (configuration)

---

### 18. Toast Notifications vs Modals (Blain)
**Date:** 2026-03-10  
**Agent:** Blain (UI Designer)  
**Status:** READY FOR TEAM REVIEW

**Decision:** Use Toast notifications for success/error messages; Modal only for destructive actions (Reset confirmation).

**Rationale:**
- **Non-Blocking:** Toasts don't interrupt flow; modals demand attention
- **PowerToys Convention:** All modules use toasts for routine feedback
- **Accessibility:** Toast messages read by screen readers
- **Dismissal:** Toasts auto-dismiss after 3–5 seconds (or manually)

**Exception:** Reset to Defaults uses modal (destructive action; confirmation prevents accidents)

**Impact:** UI feels responsive and non-intrusive

---

### 19. Two-Column Grid Layout (Blain)
**Date:** 2026-03-10  
**Agent:** Blain (UI Designer)  
**Status:** READY FOR TEAM REVIEW

**Decision:** Use Grid with two columns: Label/Help (70%, left) + Control (30%, right, MinWidth 300px).

**Responsive:** On small screens (<600px), switches to single column (control below label)

**Rationale:**
- **Label Proximity:** Help text sits immediately below label; control nearby
- **Alignment:** Left-aligned labels, right-aligned controls (visually organized)
- **PowerToys Standard:** FancyZones, Awake use similar layout
- **Readability:** Ample space for text; controls breathe

**Alternatives:** Single-column (unnecessarily tall); Label-above-control (creates visual "towers")

**Impact:** Layout strongly influences how users read the panel

---

### 20. WinUI 3 Theme Resources (Blain)
**Date:** 2026-03-10  
**Agent:** Blain (UI Designer)  
**Status:** READY FOR TEAM REVIEW

**Decision:** Use `{ThemeResource}` bindings for all colors, fonts, spacing (no hardcoded colors).

**Rationale:**
- **Light/Dark Mode:** Automatic inversion; colors correct in both themes
- **DPI Scaling:** Auto-scales at 100%, 125%, 150%, 200%+ without adjustments
- **High Contrast Mode:** Respects user's accessibility settings
- **Consistency:** All PowerToys modules use same theme resources
- **Maintainability:** Updates automatically if design system changes

**Alternative:** Manual color adjustments per theme (rejected — error-prone, violates DRY)

**Impact:** Ensures correct appearance on all OS versions, themes, accessibility settings

---

### 21. Hotkey Validation Rules (Blain)
**Date:** 2026-03-10  
**Agent:** Blain (UI Designer)  
**Status:** READY FOR TEAM REVIEW

**Decision:** Multi-level validation:
1. **Level 1 (Hard Block):** Modifier required (Ctrl, Alt, Shift, Windows); single primary key only; PowerToys conflicts
2. **Level 2 (Soft Warn):** Windows reserved shortcuts (Win+E, etc.); common third-party hotkeys

**Rationale:**
- **User Experience:** Prevent invalid input (typos, incomplete combos)
- **System Stability:** Block hotkeys that conflict with OS or other apps
- **Real-Time:** Validation happens during recording (instant feedback)

**Impact:** Critical to module reliability; prevents support requests from "hotkey doesn't work"

---

### 22. Dynamic Help Text (Blain)
**Date:** 2026-03-10  
**Agent:** Blain (UI Designer)  
**Status:** READY FOR TEAM REVIEW

**Decision:** MethodDescription property updates automatically when user changes Registry Method selection.

**Examples:**
- Auto-detect: "This will use Modern on Win11 23H2+, Legacy on Win10."
- Modern: "Seamless toggle. Recommended for Windows 11 23H2+."
- Legacy: "May require Explorer restart. Use on Windows 10 or older Windows 11."

**Rationale:**
- **Context-Aware Guidance:** Each method has nuanced pros/cons
- **Reduces Documentation Burden:** Information is inline
- **Space Efficient:** Single help text area (not three separate labels)
- **PowerToys Pattern:** Similar to Awake module

**Impact:** Improves user education; reduces friction when choosing method

---

### 23. Default Module State = OFF (Blain)
**Date:** 2026-03-10  
**Agent:** Blain (UI Designer)  
**Status:** READY FOR TEAM REVIEW

**Decision:** Enable/Disable toggle defaults to OFF on first install (opt-in).

**Rationale:**
- **Consent:** User explicitly enables module (respects principle of least surprise)
- **Safety:** Hotkey not registered until user confirms intent
- **Adoption:** Users who don't need module won't incur overhead
- **Onboarding:** First-time user sees clear "Enable ClockTray" step

**Alternative:** Default to ON (rejected — forces "opt-out"; less respectful)

**Impact:** Sets tone for module as respectful, non-intrusive

---

### 24. Config Format = JSON (Blain)
**Date:** 2026-03-10  
**Agent:** Blain (UI Designer)  
**Status:** READY FOR TEAM REVIEW

**Decision:** Settings stored in `%APPDATA%\PowerToys\modules\ClockTray\config.json`

**Schema:**
```json
{
  "enabled": false,
  "method": "Auto",
  "hotkey": "Ctrl+Alt+T"
}
```

**Rationale:**
- **PowerToys Standard:** All modules use JSON (ensures consistency)
- **Human-Readable:** Easy to inspect/debug
- **Portable:** Platform-agnostic; future implementations can reuse format
- **Serialization:** WinUI ViewModel can serialize via `JsonSerializer`

**Alternatives:** Windows Registry (rejected — violates PowerToys convention); XML (rejected — verbose)

**Impact:** Config format is bridge between UI and C++ module; JSON ensures seamless handoff

---

### 25. Three-Tier Documentation Strategy (Molly)
**Date:** 2026-03-10  
**Agent:** Molly (Technical Writer)  
**Status:** DOCUMENTED FOR TEAM REVIEW

**Decision:** Three-document approach for Sprint 1:
1. `doc/ClockTray-Overview.md` — 1-page user pitch
2. `doc/ClockTray-UserGuide-Outline.md` — Detailed outline (8–12 pages when expanded)
3. `doc/PowerToys-Context.md` — Strategic alignment

**Rationale:**
- **Separates Concerns:** User pitch vs. structure vs. strategic validation
- **Stakeholder Review:** Allows approval of structure before prose writing
- **Reduces Rework:** If outline approved, full prose is straightforward

**Impact:** Outline-first approach prevents scope creep and rework

---

### 26. User Documentation Language (Molly)
**Date:** 2026-03-10  
**Agent:** Molly (Technical Writer)  
**Status:** DOCUMENTED FOR TEAM REVIEW

**Decision:** User-centric, non-technical language; analogies over jargon.

**Rules:**
- Avoid Windows API jargon ("Registry modification via P/Invoke" → "Windows setting changes")
- Replace technical terms with actions ("WM_SETTINGCHANGE broadcast" → "System notification to update taskbar")
- Use analogies ("System tray application context" → "Utility runs in background; icon in taskbar corner")
- Show outcomes, not mechanisms ("Press hotkey → clock disappears/reappears")

**Validation:** If non-technical user understands feature, language is right

**Impact:** Accessible to power users (likely technical, but not developers)

---

### 27. User Guide Section Structure (Molly)
**Date:** 2026-03-10  
**Agent:** Molly (Technical Writer)  
**Status:** DOCUMENTED FOR TEAM REVIEW

**Decision:** Seven-section user guide structure:
1. **Getting Started** (2–3 pages) — Installation, first use, quick settings
2. **Features & How to Use** (2–3 pages) — Each capability with context
3. **Settings Panel Reference** (1–2 pages) — Every option explained
4. **Keyboard Shortcuts** (1 page) — Hotkeys, customization, common combos
5. **Troubleshooting** (2–3 pages) — Issues organized by symptom
6. **FAQ** (1–2 pages) — Conceptual questions
7. **Support & Feedback** (½ page) — Bug reporting, feature requests, contribution

**Appendices:** A (Registry paths), B (OS version matrix), C (Accessibility)

**Total Estimate:** 8–12 pages

**Rationale:**
- Mirrors PowerToys documentation patterns (Color Picker, Awake guides)
- Separates reference from narrative
- Troubleshooting + FAQ split by problem-solving vs. conceptual

**Impact:** Stakeholders can review structure before full prose writing

---

### 28. PowerToys Contribution Path & Fit (Molly)
**Date:** 2026-03-10  
**Agent:** Molly (Technical Writer)  
**Status:** DOCUMENTED FOR TEAM REVIEW

**Decision:** Documented why ClockTray aligns with PowerToys and contribution sequence.

**PowerToys Philosophy Alignment:**
- ✅ "Remove friction" → Avoid Settings menu dive (use hotkey)
- ✅ "Empower control" → Users choose to show/hide
- ✅ "Lightweight" → Zero dependencies, minimal CPU/memory
- ✅ "Windows-first" → Leverages Win32 + WinUI 3
- ✅ "Open source" → Community-auditable

**Contribution Path:** Tech spec → Design → Docs → Code → New utility (our target)

**Module Requirements:**
- Implement `PowertoyModuleIface` (factory pattern: `powertoy_create()`, `powertoy_destroy()`)
- Lifecycle: `enable()`, `disable()`, `is_enabled()`
- Config: `send_config_json()`, `get_config()` (bidirectional)
- Settings UI: WinUI 3 (not custom windows)
- Dependencies: Zero external (Windows APIs + PowerToys framework only)
- Quality: `/W4` warnings clean, >80% test coverage

**Key Insight:** PowerToys wants validation BEFORE code (tech spec can prevent wasted effort)

**Impact:** Early engagement with PowerToys team prevents rework

---

### 29. PowerToys Submission Strategy (Molly)
**Date:** 2026-03-10  
**Agent:** Molly (Technical Writer)  
**Status:** DECISION MADE

PowerToys contribution process requires contributors to follow a specific sequence before code is written. The process validates ideas early and prevents wasted effort on concepts that don't align with PowerToys philosophy.

**Decision:** PowerToys submission = File issue first + comment on #28769, BEFORE any C++ code

**Sequence:**
1. **Comment on Issue #28769** (Standing "Would you like to contribute?" thread)
   - Short intro (3–5 sentences)
   - State intent to submit ClockTray
   - Link to full feature request issue
   - Mention working C# prototype validates approach

2. **File GitHub Feature Request Issue** (Main proposal to microsoft/PowerToys)
   - Title: "ClockTray: Global Hotkey to Toggle Taskbar Clock Visibility"
   - Sections: Problem → Solution → Why Fits → Technical Approach → OS Compatibility → Prototype → Call to Action
   - Length: ~500 words (punchy, not a wall of text)
   - Links: Reference tech spec (`doc/ClockTray-TechSpec.md`), prototype repo (`feature/powertoys-contribution`)
   - Tone: Professional, collaborative, enthusiastic but technical (match PowerToys community voice)

3. **Await Feedback** (1–2 weeks)
   - PowerToys team assesses fit and architecture
   - May ask clarifying questions or suggest modifications
   - Ideally approves concept before C++ implementation begins

4. **Begin C++ Implementation** (Sprint 2, after feedback)
   - Only after PowerToys team confirms alignment
   - Reduces risk of rework or rejection
   - Ensures architectural decisions match expectations

**Rationale:**
- **Prevents duplicated effort:** PowerToys team may reject or suggest significant changes before you write 1000+ lines of C++ code
- **Validates philosophy fit:** Early feedback confirms ClockTray aligns with PowerToys' "remove friction" + "lightweight" philosophy
- **Clarifies constraints:** PowerToys may have architectural requirements, WinUI 3 version constraints, or breaking changes the team should know about upfront
- **Respects team's time:** A well-written issue + prototype demo signals maturity and commitment, making review more efficient

**Artifacts:**
- Location: `doc/powertoys-submission-draft.md`
- Contents:
  - Section 1: Comment for issue #28769
  - Section 2: Full GitHub feature request issue (suggested title + issue body)

Both in Markdown, ready for copy-paste once approved.

**Related Decisions:**
- **Tech Stack Analysis & MVP Recommendation** — Two-phase approach (Phase 1: C# MVP ✅, Phase 2: C++ rewrite 🎬)
- **MVP Implementation** — C# prototype delivered on `feature/powertoys-contribution`
- **PowerToys Contribution Path** (from PowerToys-Context.md) — Confirms sequence: Tech Spec → Design → Documentation → Code

---

### 30. Packaging ClockTray as a .NET Global Tool (Mac)
**Date:** 2026-03-03  
**Agent:** Mac  
**Status:** Implemented ✅

Bruno requested ClockTray be installable as a dotnet global tool so users can run `dotnet tool install --global ElBruno.ClockTray` and then launch it with `clocktray` from any terminal.

**Problem:**
The .NET SDK throws **NETSDK1146** error when `PackAsTool=true` is combined with `UseWindowsForms=true`:

```
error NETSDK1146: PackAsTool does not support TargetPlatformIdentifier being set. 
For example, TargetFramework cannot be net5.0-windows, only net5.0. 
PackAsTool also does not support UseWPF or UseWindowsForms when targeting .NET 5 and higher.
```

This is a hard error thrown from `Microsoft.NET.PackTool.targets` line 294 in the `_PackToolValidation` target. The SDK explicitly blocks packaging WinForms/WPF apps as tools.

**Solution:**
Override the SDK validation using explicit SDK imports and target redefinition:

1. **Change from implicit to explicit SDK imports** in `.csproj`:
   ```xml
   <Project>
     <Import Project="Sdk.props" Sdk="Microsoft.NET.Sdk" />
     <!-- properties and items here -->
     <Import Project="Sdk.targets" Sdk="Microsoft.NET.Sdk" />
     <!-- override targets AFTER SDK targets -->
   </Project>
   ```

2. **Override `_PackToolValidation` target** after SDK imports:
   ```xml
   <Target Name="_PackToolValidation" Condition=" '$(PackAsTool)' == 'true' ">
     <PropertyGroup>
       <_ToolPackShortTargetFrameworkName>net10.0</_ToolPackShortTargetFrameworkName>
       <_ToolPackShortTargetFrameworkName Condition="'$(SelfContained)' == 'true'">any</_ToolPackShortTargetFrameworkName>
     </PropertyGroup>
     <!-- Validation errors removed to allow PackAsTool + UseWindowsForms -->
   </Target>
   ```

3. **Change OutputType** from `WinExe` to `Exe` (required for dotnet tools):
   ```xml
   <OutputType>Exe</OutputType>
   ```

4. **Set tool properties**:
   ```xml
   <PackAsTool>true</PackAsTool>
   <ToolCommandName>clocktray</ToolCommandName>
   ```

**Key Technical Details:**
- **Why explicit imports work**: The SDK imports happen implicitly at the start/end of `<Project Sdk="...">`. By using explicit `<Import>`, we control the order and can define targets AFTER the SDK's targets, allowing our override to win.
- **Why set `_ToolPackShortTargetFrameworkName` directly**: The SDK calculates this via `GetNuGetShortFolderName` task which returns `net10.0-windows7.0` for `net10.0-windows` TFM. Tools require a simple TFM folder structure (`tools/net10.0/any/`) not a platform-specific one. Setting it to `net10.0` directly ensures correct package layout.
- **Console window**: Changing from `WinExe` to `Exe` means a console window briefly appears on startup. This is a trade-off for tool packaging. Could be suppressed with additional Win32 calls if needed.

**Verification:**
- ✅ `dotnet pack -c Release` succeeds
- ✅ Package created: `ElBruno.ClockTray.0.5.5.nupkg`
- ✅ Tool installs: `dotnet tool install --global ElBruno.ClockTray`
- ✅ Tool runs: `clocktray` command launches the system tray app
- ✅ Build still works: `dotnet build` succeeds

**Files Modified:**
- `ClockTray.csproj` — explicit SDK imports, PackAsTool config, target override, version 0.5.5
- `README.md` — installation instructions updated to use `dotnet tool install`

**Future Considerations:**
- Monitor .NET SDK changes — the validation might be relaxed in future versions
- Consider hiding console window with `FreeConsole()` P/Invoke if the brief flash is problematic
- Alternative: create a separate console wrapper project that references the WinForms app, but this approach is simpler

**References:**
- SDK source: `Microsoft.NET.PackTool.targets` (line 273-294 in .NET SDK 10.0.200)
- Related issue: https://github.com/dotnet/sdk/issues/10346
