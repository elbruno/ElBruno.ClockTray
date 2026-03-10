# PowerToys Contribution Plan — ClockTray Module

**Date:** 2026-03-10  
**Branch:** `feature/powertoys-contribution`  
**Goal:** Submit ClockTray as a PowerToys utility module  
**Target:** Microsoft/PowerToys repository contribution

---

## Executive Summary

ClockTray is a proof-of-concept Windows system tray utility that provides one-click toggling of the taskbar date/time display. It has completed Phase 1 (C# MVP) and is ready to transition to Phase 2: a PowerToys-compatible C++ module.

This plan outlines the path to contribute ClockTray to PowerToys following Microsoft's contribution guidelines, including:
1. **Tech spec authoring** (per PowerToys contribution requirements)
2. **C++ module implementation** aligned with PowerToys architecture
3. **PowerToys settings UI integration** (WinUI 3)
4. **Testing & compatibility validation** (Win10/Win11)
5. **Submission to PowerToys repository**

---

## Contribution Path (per Microsoft Docs)

PowerToys welcomes contributions in this order:
1. ✅ **Tech spec** — Design proposal document (THIS PHASE)
2. ✅ **Design concept** — UI/UX mockups and recommendations
3. ✅ **Documentation** — Usage and contribution docs
4. ⏳ **Bug fixes** — Issues in existing utilities
5. ⏳ **New features** — Code contributions to existing utilities
6. 🎯 **New utility** — Entirely new PowerToy (OUR TARGET)

### Why ClockTray Fits PowerToys

| Criterion | Status | Notes |
|-----------|--------|-------|
| **Solves real power user need** | ✅ | Quick toggle for taskbar clock visibility |
| **Minimal scope** | ✅ | Single function, easy to maintain |
| **Windows-first** | ✅ | Leverages Win32/WinUI ecosystem |
| **Doesn't duplicate existing utilities** | ✅ | No PowerToys utility currently manages taskbar clock |
| **Aligns with PowerToys philosophy** | ✅ | Removes friction, increases control |

---

## Phase 2: C++ Module Architecture

### Current State (Phase 1)
- **Language:** C#/.NET 8 (fast MVP)
- **Status:** Feature-complete, tested on Win10/Win11
- **Code:** ~150 lines + UI
- **Limitation:** Not suitable for PowerToys (requires C++ modules)

### Target State (Phase 2)
- **Language:** C++ (20 standard, modern practices)
- **Location:** `src/modules/ClockTray/` (PowerToys repo structure)
- **Pattern:** Implement `PowertoyModuleIface` (mandatory interface)
- **UI:** WinUI 3 settings panel in PowerToys settings app
- **Distribution:** Bundled with PowerToys installer

### Key Integration Points

**1. Module Interface (C++)**
```
✅ powertoy_create()          — Factory function
✅ powertoy_destroy()         — Cleanup
✅ enable()                   — Activate module
✅ disable()                  — Deactivate module
✅ is_enabled()               — State check
✅ send_config_json()         — Settings from UI
✅ get_config()               — Report current settings
```

**2. Settings UI (WinUI 3)**
```
🎨 Toggle: Enable/disable module
🎨 Option: Choose registry method (Modern/Legacy)
🎨 Hotkey picker: Customize activation shortcut
🎨 Status: Show current clock visibility state
```

**3. Registry & Messaging (reuse from Phase 1)**
```
📝 Modern path (Win11 23H2+): ShowSystrayDateTimeValueName
📝 Legacy path (Win10):       HideClock + Explorer restart
📝 Broadcast:                 WM_SETTINGCHANGE
```

---

## Work Breakdown — Sprint Structure

### Sprint 1: Tech Spec & Design (Week 1)
**Lead:** Dutch (Architecture)  
**Parallel support:** Blain (UI design)

| Task | Owner | Output |
|------|-------|--------|
| Write tech spec (Microsoft template) | Dutch | `doc/specs/ClockTray-TechSpec.md` |
| Design WinUI 3 settings panel mockup | Blain | Figma/PNG mockups + XAML layout |
| Research PowerToys module examples (FancyZones, etc.) | Dutch | Reference implementation notes |
| Validate with PowerToys team (GitHub discussion) | Dutch | Feedback & approval |

**Deliverables:**
- Tech spec (1500-2000 words, includes: problem statement, solution, architecture, dependencies, timeline)
- UI mockup (settings panel design)
- Module architecture diagram

---

### Sprint 2: Core C++ Module (Weeks 2–3)
**Lead:** Mac (Backend/Win32)  
**QA:** Dillon (Testing)

| Task | Owner | Output |
|------|-------|--------|
| Set up C++ project structure (PowerToys template) | Mac | `src/modules/ClockTray/` skeleton |
| Implement `PowertoyModuleIface` interface | Mac | Module boilerplate + interface stubs |
| Port registry/WM_SETTINGCHANGE logic from C# | Mac | Native C++ implementation |
| Implement hotkey handler (Win32 API) | Mac | Hotkey registration & dispatch |
| Implement enable/disable state management | Mac | Module lifecycle |
| Unit tests (Win10/Win11 paths) | Dillon | Test suite covering both code paths |

**Deliverables:**
- Compilable C++ module
- Core functionality working (toggle on/off)
- Test coverage >80%

---

### Sprint 3: Settings UI & Integration (Week 4)
**Lead:** Blain (UI/WinUI 3)  
**Support:** Mac (config JSON bridge)

| Task | Owner | Output |
|------|-------|--------|
| Create WinUI 3 settings panel XAML | Blain | Settings page in PowerToys settings app |
| Implement settings persistence (JSON) | Blain | Config load/save |
| Bind toggle/hotkey picker to settings | Blain | Two-way binding to module |
| Integration testing (settings → module) | Dillon | E2E test scenarios |

**Deliverables:**
- Settings panel UI working in PowerToys settings app
- Config updates reflected in module state
- UI tests passing

---

### Sprint 4: Testing, Docs & Submission (Week 5)
**Lead:** Dutch (Orchestration)  
**QA:** Dillon (Comprehensive testing)  
**Docs:** New role needed — Technical Writer

| Task | Owner | Output |
|------|-------|--------|
| Comprehensive OS/build validation (Win10 20H2, Win11 all builds) | Dillon | Test matrix + results |
| Edge case testing (explorer restart, rapid toggles, etc.) | Dillon | Edge case report |
| User documentation (README + usage guide) | NEW: TechWriter | `docs/ClockTray-User-Guide.md` |
| Contribution guide (for other developers) | NEW: TechWriter | `doc/devdocs/clocktray-dev.md` |
| Code review for PowerToys standards | Dutch | Checklist sign-off |
| Prepare PR submission + change summary | Dutch | PR template + description |
| Submit to PowerToys/main | Dutch | GitHub PR #XXXX |

**Deliverables:**
- Full test matrix (OS × build versions)
- User docs + dev docs
- PowerToys contributor checklist complete
- PR submitted to Microsoft/PowerToys

---

## Team Composition — New Members Needed

**Current team:**
- 🏗️ Dutch — Lead (Architecture, decisions, code review) ✅
- 🔧 Mac — Backend Dev (Win32/C++) ✅
- ⚛️ Blain — UI Dev (WinUI 3) ✅
- 🧪 Dillon — Tester (QA, edge cases) ✅
- 📋 Scribe — Session logs ✅
- 🔄 Ralph — Work monitor ✅

**New roles needed:**
- 📝 **Technical Writer** — User docs, contribution guides, PowerToys submission narrative
  - Writes user-facing documentation (how to use ClockTray)
  - Writes developer docs (how to contribute/maintain)
  - Supports submission narrative to PowerToys team

---

## Constraints & Assumptions

### Must Haves
- ✅ C++ implementation (PowerToys requirement)
- ✅ WinUI 3 settings panel (consistent with PowerToys UX)
- ✅ Support Win10 + Win11 (broad compatibility)
- ✅ Zero dependencies (lightweight, minimal bloat)
- ✅ Follow PowerToys code standards & architecture

### Nice to Have
- 🎯 Hotkey customization UI (power users appreciate it)
- 🎯 Auto-detect OS version (seamless Modern/Legacy switch)
- 🎯 Optional keyboard shortcut (Ctrl+Alt+T default, customizable)

### Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|-----------|
| PowerToys team rejects utility as "too simple" | BLOCKER | Tech spec emphasizes productivity value + market need |
| WM_SETTINGCHANGE broadcast breaks on future Win11 updates | HIGH | Legacy registry path as fallback; monitor Win11 Insider updates |
| C++ module complexity too high for small team | MEDIUM | Reference existing modules (Color Picker, Awake), pair programming with Mac |
| Settings panel UI inconsistent with PowerToys design | MEDIUM | Align with PowerToys design system early + iterative review |

---

## Success Criteria

✅ **Phase 2 Complete When:**

1. **Code:**
   - [ ] C++ module compiles without warnings (MSVC /W4)
   - [ ] All 12+ unit tests passing
   - [ ] Code coverage >80%

2. **Integration:**
   - [ ] Settings panel appears in PowerToys settings app
   - [ ] Toggle changes → module state changes → taskbar clock toggles
   - [ ] Hotkey customization works end-to-end

3. **Testing:**
   - [ ] Win10 20H2, Win11 21H2, Win11 22H2, Win11 23H2 all passing
   - [ ] Edge cases documented + passing
   - [ ] Known issues (if any) logged + prioritized

4. **Documentation:**
   - [ ] Tech spec approved by PowerToys maintainers
   - [ ] User guide complete + reviewed
   - [ ] Dev guide complete + reviewed
   - [ ] Contributor checklist 100% complete

5. **Submission:**
   - [ ] PR opened to PowerToys/main
   - [ ] CI/CD green
   - [ ] PowerToys team review initiated
   - [ ] Follow-up commits addressing feedback

---

## Timeline Estimate

| Phase | Duration | Owner | Status |
|-------|----------|-------|--------|
| Sprint 1: Spec & Design | 1 week | Dutch + Blain | 📅 Starting now |
| Sprint 2: C++ Core | 2 weeks | Mac + Dillon | 📅 Week 2–3 |
| Sprint 3: Settings UI | 1 week | Blain + Mac | 📅 Week 4 |
| Sprint 4: Testing & Submission | 1 week | Dillon + TechWriter + Dutch | 📅 Week 5 |
| **TOTAL** | **5 weeks** | Squad | **Target: Early April 2026** |

---

## Next Steps (Immediate)

1. ✅ **Create `feature/powertoys-contribution` branch** → DONE (see: git branch output)
2. 📋 **Update Squad team** → Add Technical Writer agent
3. 🎬 **Sprint 1 kickoff** → Dutch writes tech spec, Blain designs UI
4. 🔄 **Ralph monitoring** → Track sprint milestones daily

---

## PowerToys Resources

- **Contribution Guide:** https://github.com/microsoft/PowerToys/blob/main/CONTRIBUTING.md
- **Developer Docs:** https://github.com/microsoft/PowerToys/tree/main/doc/devdocs
- **Module Examples:**
  - Color Picker: `src/modules/ColorPicker/`
  - Awake: `src/modules/Awake/`
  - FancyZones: `src/modules/FancyZones/`
- **Tech Spec Template:** https://codeburst.io/on-writing-tech-specs-6404c9791159
- **PowerToys Design System:** (PowerToys settings UI patterns)

---

## Approval & Sign-Off

| Role | Name | Approval | Date |
|------|------|----------|------|
| Lead | Dutch | ⏳ Pending | — |
| Backend | Mac | ⏳ Pending | — |
| UI | Blain | ⏳ Pending | — |
| QA | Dillon | ⏳ Pending | — |
| User | Bruno Capuano | ⏳ Pending | — |

Once the squad reviews this plan, we'll spawn agents to begin Sprint 1.
