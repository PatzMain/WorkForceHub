# WorkForceHub — Architecture & Design

## 1. Architectural Approach

The system follows a pragmatic implementation of **Clean Architecture** (Onion Architecture), structured into four distinct projects with dependencies pointing strictly inward:

```text
EMS.Web  ──>  EMS.Infrastructure
  │                 │
  ▼                 ▼
EMS.Application ────> EMS.Domain
```

- **EMS.Domain**: Contains core entity models (`Employee`, `Department`, `Position`, `LeaveRequest`, `PayrollRecord`), enums, and common abstractions. Zero external dependencies.
- **EMS.Application**: Houses data transfer structures (DTOs), service boundaries, FluentValidation rules, AutoMapper configuration profiles, and core exceptions. Has no dependency on web, database, or infrastructure frameworks.
- **EMS.Infrastructure**: Implements persistence via EF Core (targeting SQLite), user identity (`UserManager`/`SignInManager` with custom properties), repository configurations, file uploads, and mail stubs.
- **EMS.Web**: An ASP.NET Core MVC presentation layers, view schemas, controllers, client scripts, custom CSS styles, and application startup definitions (`Program.cs`).

### Key Design Decoupling Patterns
- **ViewModels vs DTOs**: DTOs are used across the Application boundary. ViewModels are specific to the presentation layer (Razor views) to support localized validation, display flags, and dropdown lists, preventing leaking MVC concerns into business logic services.
- **Service-Repository Abstraction**: Repositories function as thin, generic transactional wrappers (`IRepository<T>`). Business operations (validation, transaction checks, leave request overlap handling, and delete constraints) are handled inside service implementations to maintain a clean separation of concerns.

---

## 2. Entity Database Schema

| Entity | Purpose |
|---|---|
| `Employee` | Personal, contact details, profile picture URL, base salary, FKs to Department, Position, and ApplicationUser |
| `Department` | Org unit mapping. Includes an optional `ManagerId` referencing `Employee`. |
| `Position` | Role designation, base salary metrics, and salary grade levels. |
| `LeaveRequest` | Leave logging (Vacation, Sick, etc.) driven by status checks (`Pending`, `Approved`, `Rejected`, `Cancelled`). |
| `PayrollRecord` | Monthly pay run statement containing Gross Pay, Deductions, and Net Pay ($Gross - Deductions$). |
| `ApplicationUser` | Identity user extended with an optional `EmployeeId` for self-service portal binding. |

### Soft Delete & Global Filters
`Employee`, `Department`, and `Position` implement soft deletion through the `ISoftDelete` contract and an EF Core global query filter (`IsDeleted == false`). This preserves payroll records, leave records, and reporting history when a reference record is removed.

---

## 3. UI/UX Design System

The presentation layer uses a custom CSS architecture that avoids framework overhead and ensures a sleek developer-focused SaaS aesthetic.

### CSS Token System (`wwwroot/css/tokens.css`)
Specifies core theme colors (Brand Indigo, Info Cyan, Success Emerald, Danger Rose), sizing scales, border radii, shadows, and default transition times in CSS custom variables:
- Neutral palette: custom grays optimized for dark texts on light backgrounds.
- Spacing scale: structured layout margins (4px to 64px).
- Styling components: global card scales, input fields, and custom buttons.

### Layout Styles (`wwwroot/css/site.css`)
Binds layout and UI styles to the tokens:
- **Two-Zone Container**: Sidebar nav (fixed, 240px wide) + main content flex container.
- **Responsive Offcanvas Navigation**: Automatically collapses sidebar into a slide-out drawer on devices below 992px wide, triggered by a hamburger button in the top header.
- **Toast Notifications**: Stackable Bootstrap toast bubbles for success/error cues.
- **Badge Indicators**: Clean status tags using contrasting pastel backgrounds.
- **Reduced Motion**: Disables animation/transitions under the `@media (prefers-reduced-motion: reduce)` rule.

---

## 4. Folder Structure

```text
WorkForceHub/
├── src/
│   ├── EMS.Domain/           # Entities, Enums, ISoftDelete, BaseEntity
│   ├── EMS.Application/
│   │   ├── DTOs/             # Request/Response data models per entity
│   │   ├── Interfaces/       # Abstraction boundary (Services & Repositories)
│   │   ├── Services/         # Business services implementing interfaces
│   │   ├── Validators/       # FluentValidation rulesets (MemberList.Source mapped)
│   │   ├── Mappings/         # AutoMapper configuration maps
│   │   └── Common/           # Generic collections, Custom exceptions
│   ├── EMS.Infrastructure/
│   │   ├── Data/             # DbContext, Seeding, Entity configurations
│   │   ├── Repositories/     # Entity repositories implementing IRepository
│   │   ├── Identity/         # Identity account models
│   │   └── Services/         # Storage and mail client implementations
│   └── EMS.Web/
│       ├── Controllers/      # Presentation route handlers
│       ├── Views/            # Razor markup views structured by controller
│       ├── ViewModels/       # View-specific DTO projections
│       └── wwwroot/          # Global assets, JS helpers, and token-based CSS
└── tests/
    ├── EMS.UnitTests/        # Mocked service tests, validations, AutoMapper
    └── EMS.IntegrationTests/ # End-to-end endpoints using WebApplicationFactory
```

---

## 5. Testing Framework

The testing framework includes extensive unit and integration tests to ensure system stability.

### Unit Tests (`EMS.UnitTests`)
- **Position & Dashboard Services**: Verifies retrieval boundaries, correct calculations, and deletion blocker logic (e.g., preventing position deletions when employees are assigned).
- **Validators**: FluentValidation rules for Employee, Leave, and Payroll input structures.
- **AutoMapper Profiles**: Strict configuration assertion tests (`AssertConfigurationIsValid`) ensuring all DTO properties are mapped correctly, utilizing `MemberList.Source` mapping configurations.

### Integration Tests (`EMS.IntegrationTests`)
- **CustomWebApplicationFactory**: Configures a test environment bypassing CSRF anti-forgery filters for testing POST calls, and sets up isolated, thread-safe SQLite in-memory databases per test class connection.
- **Controllers**: Simulates authentications and route navigation for Employee, Department, LeaveRequest, and Payroll modules.
- **Repositories**: Validates EF Core interactions, transactional behavior, and soft deletion query filters.
