# Project Brief v1 — Work Time & Flex Tracking App

## 1. Vision

### Problem statement

I work with flex time but do not log my actual working hours in my jobs time tracker. This makes it easy to unintentionally work more than expected over time. 

### Vision

Build a **simple, trustworthy admin-style web application** that allows manual entry of working hours and clearly shows weekly totals and flex balance. The application should be usable quickly, then improved incrementally.

### Primary success criterion

> I can use this app every workday to log my hours and trust the weekly flex number.

If this is true, the project is successful.

---

## 2. Target user

* Primary user: me
* Secondary (future): others with flex-based work hours

Single-user only for Phase 1.

---

## 3. Domain rules (Phase 1)

### Working time

* Standard working week: **40 hours**
* Expected weekly hours are configurable (default 40)

### Lunch

* Lunch is **unpaid**
* Lunch is entered as an **optional interval**: lunch start time (HH:MM) and lunch end time (HH:MM)
* The system derives lunch minutes as **(lunch end − lunch start)** when lunch is provided
* No fixed lunch window is assumed

### Flex calculation

* Worked time = (end time − start time) − lunch duration, where lunch duration = (lunch end − lunch start) when provided, otherwise 0
* Weekly flex delta = worked time − expected time
* Flex balance carries over week to week

### Time entry rules

* Start time and end time are manually entered
* End time must be after start time
* Lunch is optional but must be provided as a pair (both lunch start and lunch end)
* Lunch end must be after lunch start
* Lunch interval must be within work interval (start ≤ lunch start < lunch end ≤ end)
* Overlapping entries are allowed but flagged with a warning

### Adjustments

* Manual adjustments (+/− minutes) are allowed
* Each adjustment must include a reason
* Adjustments never delete or hide history

### Explicit non-goals (Phase 1)

* Automatic public holiday calendars
* Multiple users or teams
* Native mobile apps

---

## 4. Scope and phases

### Phase 0 — Walking skeleton

**Goal:** Prove end-to-end flow works.

* Create one time entry
* Persist it
* Display it in the UI

Deliverable: one working vertical slice (UI → API → DB → UI).

---

### Phase 1 — MVP (usable)

**Goal:** Daily usable application.

#### Epic A — Manual time entry

* Enter start time
* Enter end time
* Enter lunch start time (optional)
* Enter lunch end time (optional)
* System shows derived lunch minutes and worked time for the entry
* Optional comment
* Validation with clear error messages
* System shows derived lunch minutes and worked time for the entry

Acceptance criteria:

* Invalid time ranges are rejected
* Entries are saved and visible immediately

---

#### Epic B — Weekly calculation

* View weekly total worked hours
* Configure expected weekly hours
* See weekly flex delta (+/−)
* See running flex balance

Acceptance criteria:

* Totals are deterministic
* Editing past entries recalculates affected weeks

---

#### Epic C — Corrections and trust

* Edit existing entries
* Add manual adjustments with reason
* Adjustments included in totals

Acceptance criteria:

* History is never silently lost
* User can explain how a total was calculated

---

### Phase 2 — Reliability and quality

**Goal:** Trust the system long-term.

* Tests for calculation logic
* Clear separation of domain logic
* Improved validation and warnings

---

### Phase 3 — Portfolio polish (optional)

**Goal:** Interview-ready project.

* CI pipeline (build + tests)
* Structured logging
* Clear README and documentation

---

## 5. Non-functional goals

* Backend-first, admin-style architecture
* React frontend with enough UX to be pleasant
* Local-first development
* Low or zero running cost

---

## 6. Definition of “done” (Phase 1)

Phase 1 is complete when:

* I have used the app for at least **one full work week**
* Weekly flex numbers match manual calculations
* Mistakes can be corrected without breaking trust

---

## 7. Constraints

* No code generation from templates beyond standard tooling
* Avoid overengineering
* Prefer clarity over abstraction

---

## 9. Long-term optional ideas (explicitly deferred)

* Public holiday handling
* Multiple users
* Reports and charts
* Mobile-first UX

