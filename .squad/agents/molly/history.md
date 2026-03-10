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

### Documentation Structure Decisions
1. **Three-tier approach:** Overview → UserGuide (outline) → Context (contribution)
   - **ClockTray-Overview.md** — 1-page power user pitch (solves the "why use this?" question)
   - **ClockTray-UserGuide-Outline.md** — Detailed outline with section structure + estimated lengths
   - **PowerToys-Context.md** — Strategic alignment document explaining PowerToys fit & contribution path

2. **User-centric language:** Avoided jargon, used analogies ("light switch with a remote control"), before/after workflow comparison
   - Non-technical users should understand value immediately
   - Power users get the "removes friction" benefit statement

3. **Outline-first strategy:** Rather than writing full guide prose, structured the outline with:
   - Purpose statement for each section (who is this for?)
   - Subsection breakdown with estimated page counts
   - Content scaffolding (what will be covered)
   - This allows stakeholders to review structure before prose writing begins

### PowerToys Contribution Context Findings
1. **PowerToys philosophy identified:** Remove friction, empower control, lightweight, Windows-first, open source
   - ClockTray aligns on all five dimensions
   - Key insight: PowerToys values "do one thing well" (ClockTray's scope is right)

2. **Contribution path clarified:** Tech spec → Design → Documentation → Code submission
   - Early tech spec review prevents wasted effort
   - PowerToys team expects clear problem statement + architecture rationale
   - Documentation is *third*, not last (shows commitment before code)

3. **Module architecture requirements documented:**
   - Must implement PowertoyModuleIface (factory pattern, lifecycle, config JSON)
   - Settings UI via WinUI 3 (not custom windows)
   - Zero external dependencies (Windows APIs + PowerToys framework only)
   - >80% test coverage + /W4 compiler warnings clean

4. **PowerToys review cycle realistic:** 1–3 weeks for PR review, 2–5 feedback rounds common for new modules
   - Set team expectations for iteration time
   - Code quality + documentation completeness critical for approval

### Open Questions for Next Phase
1. **Tech spec clarity:** Dutch's draft will need review for:
   - Jargon simplification (registry paths, Win32 APIs not clear to all audiences)
   - Claims accuracy (Win11 23H2+ requirements, compatibility)
   - Completeness (does it address all Microsoft template sections?)

2. **User guide screenshots:** Need to identify:
   - Which sections require screenshots vs. prose-only
   - Screenshot size/quality standards for PowerToys docs
   - Localization readiness (placeholder text for future translation?)

3. **PowerToys team contact:** Should reach out early (GitHub Discussion) to:
   - Validate tech spec direction before finalizing
   - Clarify module integration points (any breaking changes in PowerToys recently?)
   - Confirm WinUI 3 version target (PowerToys may have constraints)

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

### Session 2: 2026-03-10 — Sprint 1 Kickoff
**Coordinator:** Copilot CLI (Scribe)  
**Event:** Three agents dispatched in parallel to establish Phase 2 foundation

**What happened:**
- **Dutch:** Authored 1,400+ word tech spec with 10 architectural decisions
- **Blain:** Designed WinUI 3 settings panel (XAML, mockups, behavior specs) + created 9-pattern skill document
- **Molly:** Created documentation framework (overview, outline, context) + created 7-pattern skill document
- **Scribe:** Wrote orchestration logs, session log, merged decision inbox, updated agent histories

**Key Outputs:**
- Tech spec + 10 decisions (Dutch)
- UI design docs + 11 decisions (Blain)
- Documentation framework + 7 decisions (Molly)
- Reusable skill documents: WinUI 3 patterns (652 lines) + PowerToys documentation patterns (417 lines)
- Total: 28 decisions, 2,500+ words, 2 reusable skills

**Team Readiness:**
- Mac: Can begin C++ implementation with clear architecture
- Blain: Can begin XAML coding with production-ready structure
- Dillon: Can begin test planning with comprehensive behavior specs
- Bruno: Can review overview + outline for scope approval

**Next:**
- Week 2: Team review of all artifacts
- Dutch opens GitHub Discussion on PowerToys repo (tech spec)
- Molly writes tech spec review document (clarity, completeness, accuracy)
- Sprint 2: Mac + Dillon begin C++ core implementation
