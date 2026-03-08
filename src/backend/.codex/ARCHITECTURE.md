# ARCHITECTURE.md — FlexTracker Domain Model

This document defines the core domain architecture used by FlexTracker. It describes the current bounded context, domain model, and invariants that must be preserved when implementing new features.

The purpose of this document is to ensure that automated agents and developers implement features consistently with Domain-Driven Design principles and with the architectural rules defined in AGENTS.md.

FlexTracker is a work-time tracking system. The domain model represents manually recorded work intervals and the rules used to calculate working time from those intervals. Correctness, explicit rules, and deterministic calculations are more important than convenience or implementation speed.

The system currently contains a single bounded context named TimeTracking. All domain concepts implemented so far belong to this bounded context. In the future the system may introduce additional contexts such as Reporting, Exports, or Payroll Integration, but those contexts do not yet exist.

Within the TimeTracking bounded context the primary aggregate is TimeEntry.

A TimeEntry represents a single recorded work interval for a specific date. The TimeEntry aggregate is responsible for enforcing all invariants that ensure the recorded data is valid and internally consistent.

A TimeEntry may contain the following information:

- Date
- StartTime
- EndTime
- LunchStartTime (optional)
- LunchEndTime (optional)

The TimeEntry aggregate must ensure that all invariants remain valid at all times. These invariants represent business rules and must not be bypassed.

The following invariants must always hold:

StartTime must be earlier than EndTime.

If a lunch interval exists, LunchStartTime must be earlier than LunchEndTime.

If a lunch interval exists, the lunch interval must occur within the work interval defined by StartTime and EndTime.

All calculated totals must be reproducible from stored data.

Invalid states must not be representable within the domain model.

The domain model should express these rules directly rather than relying on external validation or database constraints.

The TimeEntry aggregate should support domain behaviour that represents legitimate operations in the time-tracking domain. These behaviours may include:

CreateTimeEntry  
UpdateTimeEntryTimes  
AddLunchBreak  
RemoveLunchBreak  
CalculateWorkedMinutes

Operations that enforce invariants should be implemented within the aggregate root so that the TimeEntry always remains in a valid state.

The domain model should also use value objects to represent concepts that have value semantics rather than identity.

Examples of value objects in this domain include:

TimeRange, which represents a start and end time and ensures the interval is valid.

LunchBreak, which represents the lunch interval and ensures that lunch times form a valid range.

WorkedMinutes, which represents a calculated duration of work time.

Value objects must be immutable and should encapsulate their own validation logic where appropriate.

When domain invariants are violated the system should raise explicit domain errors. These errors represent violations of business rules rather than technical failures.

Examples of domain errors include:

StartAfterEndError  
InvalidLunchIntervalError  
LunchOutsideWorkIntervalError

These errors should clearly describe which invariant was violated and why.

Persistence concerns must remain separate from the domain model. Database representations of entities should exist only within the Infrastructure layer. For example, a persistence model such as TimeEntryRecord may represent how a work entry is stored in the database.

Persistence models are not domain entities. They exist purely to represent the database schema and should be mapped to domain models by infrastructure components such as repositories.

The Application layer implements use cases that orchestrate domain operations. These use cases represent the actions the system can perform from the perspective of users or external systems.

Typical use cases in the current system include:

CreateTimeEntry  
UpdateTimeEntry  
AddLunchBreak  
GetTimeEntries  
GetWeeklySummary

Application handlers coordinate domain operations and repository calls but should not implement domain invariants themselves.

Time calculations are a critical aspect of the system. All calculations must be deterministic, explicit, and reproducible. The system must never rely on hidden calculations, implicit assumptions, or behaviour that cannot be derived from stored data.

Worked time must always be derivable from the recorded timestamps and defined business rules.

The architecture of FlexTracker prioritises explicit domain modelling, clear invariants, testable business logic, and maintainable system structure. The domain model must remain independent of infrastructure technologies and frameworks so that the business rules remain stable even if persistence mechanisms or external interfaces change.

