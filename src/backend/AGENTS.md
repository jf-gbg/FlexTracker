# AGENTS.md — FlexTracker Backend (Codex)

This file describes how to work in the **backend** of FlexTracker: architecture boundaries, domain rules, and expected engineering practices.

## Collaboration style

- Act as a mentor/teacher/coach while collaborating.
- Explain why changes are made, not just what is being changed.

Source of truth for product/domain rules: `../../docs/product/project-brief.md`.

Current backend issue tracking: `skills/CURRENT_ISSUE.md`. Reference this at the start of each backend session. Update manually when requested.

---

## Goal of the backend (Phase 0–1)

Deliver a trustworthy, admin-style time tracking API that supports:
- Manual entry of work start/end
- Optional lunch interval (start + end)
- Deterministic worked-minutes calculations
- Weekly totals and flex balance (later epics)

Backend must enforce validation and calculation rules close to the domain.

---

## Domain rules (must be enforced)

### Time entry fields (Phase 1)
- `date` (workday date)
- `startTime` (required)
- `endTime` (required)
- `lunchStartTime` (optional)
- `lunchEndTime` (optional)
- `comment` (optional, planned; not yet implemented in the initial schema/model)
- Manual adjustments exist as a later epic; don’t design away history.

### Validation rules
Reject invalid ranges:
- `endTime` must be after `startTime`
- Lunch is optional but must be provided as a pair:
  - if `lunchStartTime` exists then `lunchEndTime` must exist (and vice versa)
- `lunchEndTime` must be after `lunchStartTime`
- Lunch must be within work interval:
  - `startTime ≤ lunchStartTime < lunchEndTime ≤ endTime`
- Overlapping entries are **rejected with validation errors**.

### Calculations
- `lunchMinutes = lunchEnd - lunchStart` when lunch provided, else `0`
- `workedMinutes = (end - start) - lunchMinutes`
- Weekly flex delta and balance carry-over are later epics but must remain deterministic when implemented.

---

## Architecture expectations

Keep domain logic framework-agnostic and testable.

Recommended layering (names may vary by solution layout):
- **Domain**: entities/value objects + invariants + calculation logic (no EF, no ASP.NET)
- **Application**: use-cases/services orchestrating domain + persistence abstractions
- **Infrastructure**: EF Core / SQLite, migrations, repository implementations
- **API**: Minimal API endpoints, DTOs, request/response mapping, ProblemDetails

Principles:
- Validation that expresses business invariants belongs in Domain (or Application when cross-aggregate).
- API layer should translate errors to HTTP responses; do not duplicate business rules there.

---

## API conventions

- Minimal API endpoints only.
- Use consistent routes and nouns (e.g., `/time-entries`).
- Prefer:
  - `POST /time-entries` to create
  - `GET /time-entries` to list (filters later)
  - `PUT/PATCH /time-entries/{id}` for edits (later)
- Use RFC7807 **ProblemDetails** for validation errors (400).
- Warnings (like overlaps) should be returned explicitly (e.g., `warnings: []`) rather than hidden in logs.

---

## Persistence conventions

- SQLite for local-first development.
- EF Core migrations used and committed.
- Store times in a way that preserves intent:
  - Prefer `DateOnly` for date and `TimeOnly` for times in domain.
  - Map to DB types explicitly in EF configuration.

---

## Testing expectations

At minimum:
- Unit tests for:
  - validation rules
  - worked-minutes and lunch-minutes calculations
- Tests should not depend on ASP.NET hosting or EF unless it’s an intentional integration test.
- Keep tests deterministic and fast.

---

## Changes Codex should NOT do without explicit request

- Don’t introduce complex frameworks/patterns (CQRS libraries, MediatR, event sourcing, etc.)
- Don’t add authentication/multi-user support (Phase 1 is single-user).
- Don’t assume fixed lunch window or fixed lunch minutes.
- Don’t reject overlapping entries; only warn.

---

## Implementation style

- Prefer explicit, readable code over clever abstractions.
- Keep domain methods side-effect free where possible (pure calculations).
- Use clear names that match the brief (workday/time entry/lunch interval/flex).

---

## Definition of done for a backend change

A change is “done” when:
- Validation rules are enforced consistently
- Calculation logic is covered by tests
- API returns clear error messages for invalid requests
- Behavior matches `../../docs/product/project-brief.md` rules.
