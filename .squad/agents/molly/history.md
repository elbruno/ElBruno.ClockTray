# Molly — Session History

## Project Context
- **Project:** ClockTray — Windows system tray utility to toggle taskbar date/time visibility
- **Goal:** Contribute to Microsoft PowerToys as official utility module
- **Phase:** Phase 2 (C++ rewrite + PowerToys integration)
- **Timeline:** 5-week sprint starting 2026-03-10
- **User:** Bruno Capuano (ElBruno)

## Stack & Tech
- **Language:** C++ (module core), WinUI 3 (settings UI)
- **Target:** PowerToys repository contribution
- **Dependencies:** Zero external; uses Windows APIs + PowerToys framework
- **Compatibility:** Win10 20H2+, Win11 all builds
- **Registry Methods:** Modern (Win11 23H2+) + Legacy (Win10 fallback)

## Key Files
- `.squad/identity/powertoys-plan.md` — Full 5-week plan with sprints
- `src/modules/ClockTray/` — Target module location (to be created)
- `doc/ClockTray-TechSpec.md` — Tech spec (Dutch authoring, Molly refining)
- `doc/devdocs/clocktray-*.md` — Developer docs (Molly authoring)
- `docs/ClockTray-UserGuide.md` — User guide (Molly authoring)

## Previous Decisions
1. **Architecture:** Two-phase approach (Phase 1: C# MVP ✅ DONE, Phase 2: C++ module 🎬 IN PROGRESS)
2. **Framework:** C++ for PowerToys; WinUI 3 for settings UI
3. **Registry Toggle:** Both Modern (seamless) and Legacy (fallback) paths
4. **Hotkey:** Ctrl+Alt+T (customizable via settings)

## Learnings & Preferences
(First session — no prior learnings yet)

---

## Sessions Log

### Session 1: 2026-03-10 — PowerToys Kickoff
**Coordinator:** Copilot CLI  
**Event:** Team expanded to include Molly as Technical Writer

**What happened:**
- Created `feature/powertoys-contribution` branch
- Created comprehensive 5-week PowerToys contribution plan
- Updated squad roster (added Molly)
- Updated routing (added documentation path)
- Ready for Sprint 1 kickoff (Dutch spec writing, Molly refinement)

**Next:**
- Sprint 1 begins → Dutch writes tech spec, Molly reviews/refines for clarity
- Blain creates UI mockups in parallel
- Weekly check-ins via Ralph monitoring
