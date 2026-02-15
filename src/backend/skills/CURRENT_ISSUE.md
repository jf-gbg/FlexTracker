# Current Backend Issue

Current issue:
- Implement API support for creating a manual time entry.
- Acceptance Criteria:
- POST /time-entries endpoint exists
- Accepts:
- date
- start time (HH:MM)
- end time (HH:MM)
- lunch start time (HH:MM, optional)
- lunch end time (HH:MM, optional)
- Validates input formats
- Business rules enforced:
- end time must be after start time
- if lunch start is provided, lunch end must also be provided (and vice versa)
- lunch interval must be within work interval
- lunch end must be after lunch start
- Returns structured validation errors (ProblemDetails / ValidationProblemDetails)
- Valid request persists entry and returns created resource with id

Last updated:
- 2026-02-14
