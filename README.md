# WorkForceHub — Employee Management System

WorkForceHub is a modern, enterprise-grade Employee Management System (EMS) designed for HR professionals, managers, and employees. Built following the principles of **Clean Architecture** (Onion Architecture) with ASP.NET Core MVC and .NET 8.0, the codebase maintains loose coupling, strict layer independence, and high testability.

The user interface has been fully modernized with a responsive, developer-focused SaaS aesthetic using token-based custom CSS properties, avoiding heavy/flashy animations to prioritize clean typography, clear visual hierarchy, and robust accessibility (WCAG AA compliant).

---

## 🏗️ Clean Architecture Overview

The system is split into four distinct layers, with dependencies pointing strictly inward:

```text
EMS.Web  ──>  EMS.Infrastructure
  │                 │
  ▼                 ▼
EMS.Application ────> EMS.Domain
```

- **EMS.Domain**: Zero-dependency layer representing the core business domain. Contains entity models (`Employee`, `Department`, `Position`, `LeaveRequest`, `PayrollRecord`), enums, and common interfaces.
- **EMS.Application**: Cross-cutting service abstractions, DTO models (Data Transfer Objects), AutoMapper profiles, FluentValidation rules, and exception handlers. Depends only on the Domain layer.
- **EMS.Infrastructure**: Concrete implementations of interfaces defined in Application, including database operations (`ApplicationDbContext` via SQLite), repository patterns, identity account managers, file uploading storage, and email sending stubs.
- **EMS.Web**: The MVC presentation layer, Razor Views, ViewModels, and composition root configuration (`Program.cs`). ViewModels are mapped from DTOs in this layer to keep business logic isolated from presentation.

---

## 🛠️ Technology Stack

- **Framework**: .NET 8.0 (ASP.NET Core MVC)
- **Database Access**: Entity Framework Core 8.0 with SQLite provider
- **Security & Accounts**: ASP.NET Core Identity (Roles: Admin, HR, Manager, Employee)
- **Object Mapping**: AutoMapper 12.0.1
- **Validation**: FluentValidation 11.9.0 with AutoValidation feedback
- **Visual Analytics**: Chart.js 4.x integrated dynamically with theme-derived CSS variable colors
- **Styling UI**: Bootstrap 5 + Bootstrap Icons + Centralized Custom Token CSS Design System

---

## 🌟 Key Features & Polish

1. **Authentication & Authorization**: Registrations, session cookie logins, password show/hide scripts, forgot/reset password workflows, and strict role-based route/action security.
2. **Dynamic Dashboard**: Responsive metrics, live Chart.js visualization styled with theme colors, and a "Recent Hires" section.
3. **Employee Directory**: Paginated directory tables, multi-column sorting (name, department, employee number, date joined), and inline status badges. Includes dynamic terminate/resign date handlers and file upload previews.
4. **Leave Management**: Operational leave requests with date validation, approved request conflict overlap checks, and manager review tools (approve/reject/comment).
5. **Payroll Processing**: Automated net payout calculations ($Gross - Deductions$), real-time JS payment calculators, and payrun state transitions (Draft → Processed → Paid).
6. **Reusable Components**: Clean breadcrumbs, floating stackable Bootstrap Toasts, custom modal delete confirmations, and illustrated empty-state panels.
7. **Accessibility (A11y)**: Focus rings, standard `<main role="main">` markup, skip links, aria-labels for icon buttons, and color contrast ratios exceeding WCAG AA requirements.
8. **Performance**: preconnect CDNs, optimized font downloading sub-selections, and `loading="lazy"` attributes on directory avatar images.

---

## 📂 Project Directory Structure

```text
WorkForceHub/
├── src/
│   ├── EMS.Domain/           # Entities, Enums, BaseEntity class
│   ├── EMS.Application/      # DTOs, Service Interfaces, Validators, Mappings
│   ├── EMS.Infrastructure/   # DBContext, Repositories, Identity, Disk Storage
│   └── EMS.Web/              # Controllers, ViewModels, Razor Views, Program.cs
├── tests/
│   ├── EMS.UnitTests/        # xUnit unit tests (Service and Validation logic)
│   └── EMS.IntegrationTests/ # Controller & Repository tests (in-memory SQLite)
└── EmployeeManagementSystem.sln
```

---

## ⚙️ Quick Start & Setup Instructions

To build, test, and run the solution locally:

### Prerequisites
- Download and install the **.NET 8.0 SDK**: [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

### Installation Steps

1. **Clone or Extract the codebase** into your working directory.
2. **Build the Solution** in the root directory:
   ```bash
   dotnet build
   ```
3. **Start the Web Application**:
   ```bash
   dotnet run --project src/EMS.Web
   ```
4. **Access the application** at `https://localhost:7001` or `http://localhost:5000`. The SQLite database file (`workforcehub.db`) will be automatically created and seeded on startup.

### Default Seeding Account
The initializer seeds sample departments, employees, leave requests, payroll records, and an administrator account:
- **Email / Username**: `admin@workforcehub.com`
- **Password**: `Admin@123`

---

## 🧪 Testing

Execute the test suites containing 33 Unit Tests and 12 Integration Tests:
```bash
dotnet test
```

- **Unit Tests (`EMS.UnitTests`)**: Covers service business rules (Position, Dashboard), validation boundaries (Employee, Leave, Payroll), and AutoMapper profile mapping validity.
- **Integration Tests (`EMS.IntegrationTests`)**: Validates Web endpoints under realistic authenticated conditions using a custom `CustomWebApplicationFactory` powered by isolated in-memory SQLite instances.
