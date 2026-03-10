# Decision Log: Molly Sprint 1 Documentation

**Date:** 2026-03-10  
**Agent:** Molly (Technical Writer)  
**Phase:** Sprint 1 — Tech Spec & Design  
**Status:** DOCUMENTED FOR TEAM REVIEW

---

## Decisions Made

### 1. Documentation Delivery Strategy
**Decision:** Three-document approach for Sprint 1  
**Rationale:**
- Separates concerns: user pitch vs. user guide structure vs. strategic context
- Allows stakeholders to review structure before full prose writing
- Reduces rework on downstream (if outline is approved, full guide prose is straightforward)

**Documents:**
1. `doc/ClockTray-Overview.md` — 1-page user pitch (power users, value proposition)
2. `doc/ClockTray-UserGuide-Outline.md` — Detailed outline with section structure + estimated lengths (8–12 pages total when expanded)
3. `doc/PowerToys-Context.md` — Strategic alignment + contribution path (reference for team)

**Owner:** Molly  
**Approval Gate:** Dutch (lead) reviews structure before outline → full prose phase

---

### 2. User Documentation Language & Tone
**Decision:** User-centric, non-technical, analogies over jargon  
**Rationale:**
- PowerToys audience includes power users who may not be C++ developers
- Analogies ("light switch with remote control") make abstract concepts tangible
- Before/after workflow comparison helps users visualize value

**Examples:**
- ❌ "Registry modification via P/Invoke"
- ✅ "Press your hotkey to hide or show the clock"

- ❌ "WM_SETTINGCHANGE broadcast"
- ✅ "System notification to update the taskbar"

**Owner:** Molly  
**Validation:** Once tech spec available, Molly reviews for jargon appropriateness

---

### 3. User Guide Structure (Pre-approval)
**Decision:** Seven-section structure with appendices  
**Sections:**
1. Getting Started (installation, first use, quick settings)
2. Features & How to Use (toggle, hotkey, tray icon, auto-detection)
3. Settings Panel Reference (every setting explained)
4. Keyboard Shortcuts & Hotkeys (quick ref + customization)
5. Troubleshooting (common issues + solutions)
6. FAQ (conceptual Q&A)
7. Support & Feedback (help channels, bug reporting)

**Appendices:**
- A: Registry paths (for power users)
- B: Windows version matrix
- C: Accessibility notes

**Rationale:**
- Mirrors PowerToys documentation patterns (Color Picker, Awake guides)
- Separates reference material (Settings Panel) from narrative (Features)
- Troubleshooting + FAQ split by problem-solving vs. conceptual questions
- Appendices allow deep dives without cluttering main narrative

**Owner:** Molly  
**Next:** Full prose writing after outline approval

---

### 4. PowerToys Contribution Fit Assessment
**Decision:** Documented why ClockTray aligns with PowerToys (philosophy + market position)  
**Key Findings:**
- ✅ Solves real power user problem (reclaim taskbar space)
- ✅ Minimal scope (one function, ~150 lines core logic)
- ✅ No duplication (no existing clock toggle utility in PowerToys)
- ✅ Empowers user control (reversible, instant, transparent)
- ✅ Lightweight (zero external dependencies)

**PowerToys Philosophy Alignment:**
- "Remove friction" → Avoid Settings menu dive (use hotkey)
- "Empower control" → Users choose to show/hide
- "Lightweight" → Zero dependencies, minimal CPU/memory
- "Windows-first" → Leverages Win32 + WinUI 3
- "Open source" → Community-auditable

**Owner:** Molly  
**Audience:** Team reference + PowerToys team validation

---

### 5. PowerToys Contribution Path (Sprint Planning)
**Decision:** Documented Microsoft's contribution sequence for clarity  
**Path:**
1. Tech spec (THIS: Sprint 1)
2. Design concept (Blain's UI mockups: Sprint 1)
3. Documentation (Molly's user + dev guides: Sprint 4)
4. Bug fixes (not applicable)
5. Features (not applicable)
6. **New utility** (OUR TARGET: Complete by week 5)

**Key Insight:** PowerToys wants validation *before* code. Tech spec can prevent wasted effort if feedback is negative. Early code review of spec saves rework.

**Owner:** Dutch (lead coordination)  
**Action:** Open GitHub Discussion on PowerToys repo once tech spec drafted (week 1–2)

---

### 6. PowerToys Module Architecture Requirements
**Decision:** Documented integration constraints for team clarity  
**Modules must implement:**
- `PowertoyModuleIface` (factory pattern: `powertoy_create()`, `powertoy_destroy()`)
- Lifecycle: `enable()`, `disable()`, `is_enabled()`
- Config: `send_config_json()`, `get_config()` (bidirectional settings)
- Settings UI: WinUI 3 (not custom windows)
- Dependencies: Zero external (Windows APIs + PowerToys framework only)
- Quality: `/W4` compiler warnings clean, >80% test coverage

**File Structure:** Standard PowerToys layout
```
src/modules/ClockTray/
├── dll/              (core module C++)
├── WinUI3/           (settings panel)
└── tests/            (unit + integration tests)
```

**Owner:** Mac (backend implementation)  
**Reference:** Color Picker, Awake modules in PowerToys repo

---

### 7. Documentation Review Gate (Tech Spec Refinement)
**Decision:** Defined Molly's tech spec review role  
**Once Dutch completes draft, Molly will:**
1. Check clarity (jargon → plain language)
2. Verify completeness (all Microsoft template sections covered)
3. Validate technical claims (architecture, registry paths, OS compatibility)
4. Flag audience-appropriateness (is this understandable to PowerToys team + users?)

**Deliverable:** "Tech Spec Review" document with suggestions (not edits — Molly advises, Dutch decides)

**Owner:** Molly (review), Dutch (authorship)  
**Timeline:** After Dutch's draft (expected: week 1)

---

## Open Questions for Team

### For Dutch (Lead/Architecture)
1. **Tech spec scope:** How deep into Win32 implementation details? (Registry paths, WM_SETTINGCHANGE mechanics)
   - *Concern:* PowerToys team needs technical depth; non-technical readers shouldn't be lost

2. **OS compatibility claims:** Win10 20H2+ support confirmed? Win11 23H2 "modern path" behavior stable?
   - *Concern:* Documentation accuracy depends on validated compatibility matrix

3. **Timeline realism:** 5 weeks for C++ module + WinUI 3 UI + testing reasonable?
   - *Concern:* PowerToys may ask for features beyond MVP scope; review cycle may extend timeline

### For Mac (Backend/Implementation)
1. **Module complexity:** Are there gotchas in PowertoyModuleIface implementation not obvious from examples?
   - *Concern:* Documentation should warn about pitfalls; Molly's dev guide needs accuracy

2. **Hotkey binding:** Win32 hotkey registration in module context — any threading/message loop concerns?
   - *Concern:* Dev guide should document known issues/workarounds

### For Blain (UI/Design)
1. **Settings panel mockup timeline:** When ready for Molly to review? (affects user guide screenshots)
   - *Concern:* Molly needs final UI to write accurate settings reference

2. **Accessibility baseline:** Are WinUI 3 controls in PowerToys all WCAG 2.1 AA by default?
   - *Concern:* User guide + dev guide should document accessibility requirements

### For Dillon (QA/Testing)
1. **Test matrix scope:** Which Windows versions/builds critical? (Win10 20H2, Win11 21H2/22H2/23H2?)
   - *Concern:* User guide troubleshooting section should reference tested OS list

2. **Edge cases to expect:** Explorer restart, rapid toggles, permission errors — any others?
   - *Concern:* Troubleshooting guide needs complete edge case coverage

---

## Success Criteria (Sprint 1)

✅ **Overview document approved** — Captures value proposition for power users  
✅ **User guide outline approved** — Section structure, scope, and length validated  
✅ **PowerToys context documented** — Team understands contribution path + fit  
✅ **Tech spec ready for review** — Dutch completes draft (Molly refines)  
✅ **Open questions listed** — Team can answer next sync  

---

## Next Actions (Week 1)

1. **Dutch:** Complete tech spec draft → Molly reviews for clarity
2. **Molly:** Write tech spec review document → Flag jargon, completeness, accuracy
3. **Blain:** UI mockups → Send to Molly for user guide reference
4. **Dutch:** Open GitHub Discussion on PowerToys repo → Share tech spec, get early feedback
5. **Team sync:** Review questions above, confirm timeline + scope

---

## Archival Note

This decision log is approved by: **Molly** (author)  
For team reference: `.squad/decisions/molly-sprint1-docs.md`  
Follow-up reviews: After Dutch's tech spec (week 2) and after first PowerToys team feedback
