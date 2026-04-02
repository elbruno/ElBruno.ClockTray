# Skill: PowerToys Documentation Patterns

**Author:** Molly (Technical Writer)  
**Date:** 2026-03-10  
**Domain:** PowerToys utility documentation (user guides + context)  
**Scope:** Patterns for documenting lightweight Windows utilities destined for PowerToys contribution

---

## Overview

This skill captures reusable patterns for documenting PowerToys utilities—specifically utilities designed to empower power users with single-purpose tools. These patterns are based on ClockTray's documentation strategy and can be applied to other PowerToys contributions.

---

## Core Patterns

### Pattern 1: Three-Tier Documentation Structure

**When to use:** Documenting a PowerToys utility destined for official contribution  
**Components:**
1. **Overview Document (1 page)** — User pitch + value proposition
   - Audience: Power users deciding whether to use
   - Tone: Clear, non-technical, problem-focused
   - Key sections: What is it? | Problem it solves | Key features | Before/after workflow | Who should use?
   - Deliverable: `doc/{UtilityName}-Overview.md`

2. **User Guide Outline (2–3 pages)** — Structure + section descriptions
   - Audience: Stakeholders approving guide structure before prose writing
   - Tone: Instructional, task-focused
   - Key sections: Each section includes purpose, subsections, estimated length, content notes
   - Deliverable: `doc/{UtilityName}-UserGuide-Outline.md`

3. **Strategic Context Document (1–2 pages)** — PowerToys fit + contribution narrative
   - Audience: Team + PowerToys review team
   - Tone: Strategic, validation-focused
   - Key sections: Why it fits PowerToys | Contribution path | Module architecture | Standards | Resources
   - Deliverable: `doc/PowerToys-Context.md` (shared across project)

**Rationale:** Separates user-facing pitch from structural planning from strategic validation. Allows early review gates without writing full prose.

**Anti-pattern:** Writing full user guide prose before outline approval → risk of scope creep and rework.

---

### Pattern 2: User Documentation Language (Non-Jargon)

**When to use:** Writing for power users (likely technical, but not developers)  
**Rules:**

1. **Avoid Windows API jargon**
   - ❌ "Registry modification via P/Invoke"
   - ✅ "Windows setting changes (automatic)"

2. **Replace technical terms with actions/outcomes**
   - ❌ "WM_SETTINGCHANGE broadcast"
   - ✅ "System notification to update the taskbar"
   - ❌ "Hotkey registration via NativeWindow wrapper"
   - ✅ "Keyboard shortcut that wakes the utility"

3. **Use analogies for abstract concepts**
   - ❌ "System tray application context"
   - ✅ "Utility runs in the background; icon appears in taskbar corner"

4. **Show outcomes, not mechanisms**
   - ❌ "Toggle modifies registry value HKEY_CURRENT_USER\...\ShowSystrayDateTime"
   - ✅ "Press hotkey → clock disappears/reappears"

**Example application (ClockTray):**
- ✅ "Press Ctrl+Alt+T to hide or show the taskbar clock"
- ✅ "ClockTray automatically detects your Windows version and chooses the right method"
- ✅ "Your hotkey preference is saved and restored when you restart"

**Validation:** If a non-technical user can understand the feature by reading it, the language is right.

---

### Pattern 3: User Guide Section Structure

**When to use:** Outlining a comprehensive user guide for a PowerToys utility  
**Recommended seven sections:**

1. **Getting Started** (2–3 pages)
   - Installation, first-use, quick settings overview
   - Goal: "I've heard about this; how do I get it running?"

2. **Features & How to Use** (2–3 pages)
   - Each capability explained with context
   - Goal: "What can this do for me? How do I use it?"

3. **Settings Panel Reference** (1–2 pages)
   - Every option explained in detail
   - Goal: "What does this setting mean? When should I change it?"

4. **Keyboard Shortcuts** (1 page)
   - Global hotkeys, customization, common combinations
   - Goal: "How do I use this faster?"

5. **Troubleshooting** (2–3 pages)
   - Common issues + solutions, organized by symptom
   - Goal: "Something's not working. How do I fix it?"

6. **FAQ** (1–2 pages)
   - Conceptual Q&A, not troubleshooting
   - Goal: "I'm curious about X. Why is it that way?"

7. **Support & Feedback** (½ page)
   - Where to report bugs, request features, contribute
   - Goal: "How do I help? Who do I contact?"

**Optional appendices:**
- Appendix A: Technical deep-dive (registry paths, advanced settings)
- Appendix B: Compatibility matrix (OS versions, known limitations)
- Appendix C: Accessibility (keyboard-only, screen reader, high-contrast)

**Rationale:** Separates task-focused (Getting Started, Features) from reference (Settings, Shortcuts) from problem-solving (Troubleshooting). Mirrors PowerToys docs structure (Color Picker, Awake).

**Page estimate:** 8–12 pages total (varies by feature complexity)

---

### Pattern 4: Before/After Workflow Comparison

**When to use:** Explaining the value proposition of a utility that simplifies a workflow  
**Structure:**

| Aspect | Before (Without Utility) | After (With Utility) |
|--------|-------------------------|---------------------|
| **Workflow** | [Steps to accomplish task the hard way] | [Steps with utility] |
| **Time** | [Estimated time] | [Estimated time] |
| **Friction** | [Obstacles, menu dives, etc.] | [Streamlined] |

**Example (ClockTray):**

| Aspect | Before | After |
|--------|--------|-------|
| **Workflow** | Settings → Personalization → Taskbar → Toggle clock | Press Ctrl+Alt+T |
| **Friction** | 3-level deep menu, hard to remember path | One hotkey, always available |
| **Reversibility** | Same steps to undo | Same hotkey to toggle back |

**Rationale:** Concrete comparison helps power users decide immediately if utility solves *their* problem.

---

### Pattern 5: PowerToys Contribution Context Document

**When to use:** Documenting why a utility fits PowerToys and how the contribution process works  
**Sections:**

1. **PowerToys Philosophy** (½ page)
   - Core values: Remove friction, empower control, lightweight, Windows-first, open source
   - Why PowerToys matters (trusted, auditable, community-driven)

2. **Why Your Utility Fits PowerToys** (½–1 page)
   - Solves real power user problem
   - Minimal scope (avoids bloat)
   - Doesn't duplicate existing utilities
   - Aligns with philosophy

3. **PowerToys Contribution Path** (½ page)
   - Sequence: Tech spec → Design → Docs → Code → New utility (ours)
   - Why order matters (prevent waste, validate early)

4. **Module Architecture** (1 page)
   - Interface requirements (PowertoyModuleIface)
   - Settings UI pattern (WinUI 3)
   - File structure (src/modules/UtilityName/)

5. **PowerToys Standards** (½–1 page)
   - Code quality (C++, modern practices, /W4 warnings clean)
   - Testing (>80% coverage, multi-OS matrix)
   - Documentation (tech spec, user guide, dev guide)
   - UX/UI (consistent with PowerToys design system)

6. **Contribution Process** (½ page)
   - Discuss early (GitHub Discussions)
   - Submit tech spec for feedback
   - Submit code PR with tests + docs
   - Iterate based on review

7. **Resources** (½ page)
   - PowerToys repo links
   - Module examples (Color Picker, Awake, FancyZones)
   - Design system references

**Rationale:** Clarifies for team why this contribution path is right and manages expectations around PowerToys review cycle (1–3 weeks PR review, 2–5 feedback rounds).

---

### Pattern 6: Tech Spec Review Template (Molly's Role)

**When to use:** Reviewing a tech spec before submission to PowerToys  
**Review dimensions:**

1. **Clarity**
   - [ ] Jargon appropriately explained for mixed audience (developers + product team)
   - [ ] Acronyms defined on first use
   - [ ] Complex mechanisms explained with analogy or diagram

2. **Completeness**
   - [ ] All Microsoft tech spec template sections covered
   - [ ] Architecture diagram or pseudocode provided
   - [ ] Rationale for key decisions explained
   - [ ] Dependencies clearly listed
   - [ ] Timeline and resource estimates realistic

3. **Accuracy**
   - [ ] Technical claims validated (OS compatibility, API behavior)
   - [ ] No contradictions between sections
   - [ ] Examples are correct (registry paths, hotkey combinations)
   - [ ] Known limitations acknowledged

4. **Audience Fit**
   - [ ] PowerToys team can understand scope + architecture
   - [ ] Non-technical readers understand the problem being solved
   - [ ] Decision-makers see why this is worth effort

5. **Tone**
   - [ ] Professional but accessible
   - [ ] Problem-focused (what user pain point exists?)
   - [ ] Solution-confident (clear how your approach solves it)

**Deliverable:** "Tech Spec Review" document with suggestions, organized by dimension. Molly advises; spec author (e.g., Dutch) decides what to change.

---

### Pattern 7: User Guide Section Patterns

**Getting Started section:**
- Subsections: Installation → Enable → First use → Quick settings
- Tone: Action-oriented (user just wants it working)
- Example: "Press your hotkey to toggle the clock" (no jargon)

**Features section:**
- Each subsection: Feature name → What it is → How to use it → When you'd want it
- Tone: Exploratory (user discovering capabilities)
- Example: "Clock Visibility Toggle — Press your hotkey to hide or show the taskbar clock. Useful when..." 

**Settings Reference section:**
- Each subsection: Setting name → What it does → When to change it → Valid values/options
- Tone: Reference (user looking up a specific setting)
- Example: "Hotkey Picker — Choose which keyboard combination activates ClockTray. Default: Ctrl+Alt+T. You can change to any combination that doesn't conflict with Windows or your apps."

**Troubleshooting section:**
- Organization by symptom (not root cause)
- Format: "Problem: X | Possible causes: A, B, C | Solutions: Try A, then B, then C"
- Example: "Problem: Hotkey not working | Causes: Wrong key bound, app conflict, ClockTray disabled | Solutions: Check PowerToys settings, test different key, restart PowerToys"

**FAQ section:**
- Conceptual, not troubleshooting (vs. Troubleshooting which is symptom-focused)
- Example: "Q: Why would I hide the clock? A: Different reasons — full-screen work, space on small monitors, focus without distractions."

---

## Checklist: Documenting a PowerToys Utility

### Pre-Writing
- [ ] Read PowerToys Contribution Guide + existing module examples
- [ ] Confirm utility fits PowerToys philosophy (solve power user problem, minimal scope, no duplication)
- [ ] Identify target user (power users? all users? specific workflow?)
- [ ] Map utility's features to user workflows (avoid feature-focused language)

### Overview Document
- [ ] Problem statement clear and relatable
- [ ] Value proposition obvious in first paragraph
- [ ] Before/after comparison shows workflow improvement
- [ ] No jargon; analogies used where helpful
- [ ] Audience can decide immediately if it's for them

### User Guide Outline
- [ ] Seven sections present (Getting Started, Features, Settings, Shortcuts, Troubleshooting, FAQ, Support)
- [ ] Each section has purpose statement + audience + subsections
- [ ] Estimated lengths are realistic (8–12 pages total)
- [ ] Tone consistent across sections
- [ ] Screenshots/visual aids identified where needed

### PowerToys Context Document
- [ ] PowerToys philosophy clearly explained
- [ ] Utility's fit justified on 4+ dimensions
- [ ] Contribution path and review expectations set
- [ ] Module architecture requirements listed
- [ ] Standards (code quality, testing, documentation) explicit
- [ ] Open questions for team identified

### Tech Spec Review (After Author Draft)
- [ ] Clarity ✅ (jargon, acronyms, complexity)
- [ ] Completeness ✅ (template sections, architecture, timeline)
- [ ] Accuracy ✅ (technical claims, OS compatibility, examples)
- [ ] Audience fit ✅ (PowerToys team understands; non-technical understand problem)
- [ ] Review document written with suggestions (not edits)

### Full User Guide (After Outline Approval)
- [ ] Each section has introduction + body + conclusion
- [ ] Screenshots provided for Settings, UI-heavy sections
- [ ] Tone consistent (non-jargon, action-focused)
- [ ] Troubleshooting organized by symptom
- [ ] FAQ covers conceptual questions (not problem-solving)
- [ ] Cross-references between sections (Troubleshooting → Getting Started if needed)
- [ ] Appendices available for advanced users

---

## Anti-Patterns to Avoid

❌ **Jargon-first documentation**  
→ Use outcome language instead ("the clock disappears" not "registry value modified")

❌ **Features-focused instead of workflow-focused**  
→ Describe how the feature helps with a real task, not just what it does

❌ **Full prose before outline approval**  
→ Outline-first allows stakeholder review before effort investment

❌ **Troubleshooting organized by root cause**  
→ Users don't know root cause; organize by symptom users experience

❌ **Settings documentation that mirrors code structure**  
→ Organize by user goal (when would I change this?) not internal design

❌ **Ignoring PowerToys contribution path**  
→ Failing to engage PowerToys team early can kill the contribution later (wasted effort)

❌ **Technical spec without non-technical summary**  
→ PowerToys team includes product managers, not just engineers

---

## Quick Reference: Document Templates

### Overview Document (1 page)
```
# {UtilityName}: {Value Proposition}

## What is {Utility}?
[One sentence pitch]

## The Problem It Solves
[Pain point power users experience]

## Key Features
[3–5 bullet points]

## Before & After
[Workflow comparison table]

## Who Should Use It?
[Target audience personas]

## Quick Start
[3–5 steps to get running]
```

### User Guide Outline (2–3 pages)
```
# {Utility} User Guide — Table of Contents

## 1. Getting Started (~X pages)
**Purpose:** [Who, what goal]
**Subsections:**
- X (~¼ page): What will be covered
- Y (~½ page): What will be covered

## 2. Features (~X pages)
...

[Repeat for remaining 5 sections]

## Metadata
| Estimated length | Screenshots needed | Review checkpoint |
```

### PowerToys Context (1–2 pages)
```
# PowerToys Contribution Context for {Utility}

## Understanding PowerToys
[Philosophy: remove friction, empower control, lightweight, Windows-first, open source]

## Why {Utility} Fits PowerToys
[1. Solves real problem, 2. Minimal scope, 3. No duplication, 4. Empowers user]

## Contribution Path
[Tech spec → Design → Docs → Code → Submission]

## Module Architecture
[Interface requirements, settings UI pattern, file structure]

## PowerToys Standards
[Code quality, testing, documentation, UX/UI]

## Resources & Next Steps
```

---

## Usage Notes

- **Scale:** These patterns work for utilities in 1–3 person-weeks of development scope (like ClockTray)
- **Adapt:** For larger utilities (FancyZones, Color Picker), expand each section but keep overall structure
- **Language:** Patterns assume English; translate or localize only after PowerToys approval
- **Audience:** Patterns assume power user + developer audience (PowerToys' demographic)

---

## Related Skills

- PowerToys module architecture (Mac's domain)
- WinUI 3 settings UI patterns (Blain's domain)
- Tech spec authoring per Microsoft template (Dutch's domain)
- PowerToys contribution process (Dutch's domain)

---

**Maintained by:** Molly  
**Last updated:** 2026-03-10  
**Status:** Active (applied to ClockTray Sprint 1 docs)
