# PowerToys Contribution Tracker

Track the status of the ClockTray PowerToys contribution process.

---

## 📦 Our Repository

| Resource | URL |
|----------|-----|
| **Feature branch** | https://github.com/elbruno/ElBruno.ClockTray/tree/feature/powertoys-contribution |
| **Tech Spec** | https://github.com/elbruno/ElBruno.ClockTray/blob/feature/powertoys-contribution/doc/ClockTray-TechSpec.md |
| **Submission Draft** | https://github.com/elbruno/ElBruno.ClockTray/blob/feature/powertoys-contribution/doc/powertoys-submission-draft.md |
| **Technical Review** | https://github.com/elbruno/ElBruno.ClockTray/blob/feature/powertoys-contribution/doc/powertoys-submission-review.md |

---

## 🎯 PowerToys Contribution Links

| Resource | URL | Status |
|----------|-----|--------|
| **PowerToys repo** | https://github.com/microsoft/PowerToys | — |
| **"Would you like to contribute?" thread** | https://github.com/microsoft/PowerToys/issues/28769 | ⏳ Post comment |
| **Feature request issue** | _(to be filed)_ | ⏳ File issue |
| **PowerToys PR** | _(to be opened after approval)_ | 🔒 Pending team approval |

> **Next action:** Post the comment on #28769 and file the feature request issue using the drafts in `doc/powertoys-submission-draft.md`.
> Once filed, update this file with the issue URL.

---

## 📋 Contribution Process

PowerToys requires: **issue first → community conversation → team agreement → code**

| Step | Action | Status |
|------|--------|--------|
| 1 | Comment on [#28769](https://github.com/microsoft/PowerToys/issues/28769) to announce intent | ⏳ Pending |
| 2 | File feature request issue at microsoft/PowerToys | ⏳ Pending |
| 3 | Engage with PowerToys team feedback | 🔒 Blocked on Step 2 |
| 4 | Get team agreement on approach | 🔒 Blocked on Step 3 |
| 5 | Sprint 2 — Mac writes C++ module skeleton | 🔒 Blocked on Step 4 |
| 6 | Sprint 3 — Blain builds WinUI 3 settings panel | 🔒 Blocked on Step 5 |
| 7 | Sprint 4 — Dillon tests, Molly finalizes docs | 🔒 Blocked on Step 6 |
| 8 | Open PowerToys PR | 🔒 Blocked on Step 7 |

---

## 🏗️ Sprint Status

| Sprint | Focus | Status |
|--------|-------|--------|
| **Sprint 1** | Tech spec, UI design, documentation | ✅ Complete |
| **Sprint 2** | C++ core module (Mac + Dillon) | ⏳ Awaiting PowerToys approval |
| **Sprint 3** | WinUI 3 settings panel (Blain) | ⏳ Awaiting Sprint 2 |
| **Sprint 4** | Testing, docs finalization, PR submission | ⏳ Awaiting Sprint 3 |

---

## 📝 How to Update This File

When Bruno files the issue at microsoft/PowerToys:
1. Replace `_(to be filed)_` with the actual issue URL
2. Update Step 2 status from ⏳ to ✅
3. Update Step 3 status from 🔒 to ⏳
4. Commit and push to `feature/powertoys-contribution`

---

*Last updated: 2026-03-10 | Branch: `feature/powertoys-contribution`*
