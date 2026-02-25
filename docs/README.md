# 🧾 MoneyPilot Project Documentation

## 1. Project Overview

**MoneyPilot** is a full-stack finance and productivity web application that enables users to manage personal expenses, track budgets and investments, and build good financial habits. It is designed with a modular architecture using ASP.NET Core and Angular, following best practices for security, maintainability, and scalability.

### 🎯 Target Users

* Individuals tracking their finances
* Students managing budgets
* Professionals working toward savings goals

### 🔑 Key Features

* Secure authentication (JWT / ASP.NET Identity)
* Expense and income tracking
* Budget planning and monitoring
* Investment tracking and simulation
* Habit-based productivity tracker
* Analytics dashboard with charts and summaries

---

## 2. Tech Stack

### Backend

* ASP.NET Core 8 (Web API)
* Entity Framework Core (Code-First, SQL Server)
* C#, LINQ, AutoMapper, FluentValidation

### Frontend

* Angular 17, TypeScript
* Angular Material or Tailwind CSS
* Chart.js or ngx-charts

### DevOps & Tooling

* Git + GitHub
* Swagger / OpenAPI
* Azure App Service (planned)
* Jenkins / GitHub Actions
* Octopus Deploy (CI/CD optional)

---

## 3. System Architecture

### 📐 Layered Design (Clean Architecture)

```
Frontend (Angular)
  ↓
API Controllers (MoneyPilot.API)
  ↓
Application Layer (MoneyPilot.Application)
  ↓
Infrastructure Layer (MoneyPilot.Infrastructure)
  ↓
Domain Layer (MoneyPilot.Domain)
  ↓
SQL Server (EF Core)
```

### 🔁 Flow of Request

1. Angular calls secure API endpoint
2. Controller validates and passes to Application Service
3. Application Service accesses Repository via Unit of Work
4. Repository queries SQL DB via DbContext
5. Response returns to frontend for display

---

## 4. Project Structure

### Backend (.NET Solution)

* `MoneyPilot.API`: Controllers, middleware, startup config
* `MoneyPilot.Application`: DTOs, interfaces, business logic
* `MoneyPilot.Infrastructure`: DbContext, EF config, repositories
* `MoneyPilot.Domain`: Entities, enums, value objects
* `MoneyPilot.Tests`: xUnit tests

### Frontend (Angular)

* `core/`: Auth, services, interceptors
* `features/`: expenses, dashboard, habits, investments
* `shared/`: components, pipes, directives
* `models/`: interfaces and types

---

## 5. Database Design

### 🧱 Core Entities

* `User`: Authenticated identity
* `Expense`: Amount, category, date, notes
* `Budget`: Limits and progress
* `Investment`: Asset name, value, ROI simulation
* `Habit`: Habit type, frequency, streak

### Migration Strategy

* EF Core Code-First
* Run `Add-Migration` and `Update-Database`

---

## 6. API Documentation (Sample)

### 🔐 Authentication

* `POST /api/auth/login`
* `POST /api/auth/register`

### 📊 Expenses

* `GET /api/expenses` (paginated, filtered)
* `POST /api/expenses`
* `PUT /api/expenses/{id}`
* `DELETE /api/expenses/{id}`

### 💰 Budget

* `GET /api/budgets`
* `POST /api/budgets`

---

## 7. Frontend Guide

### UI Modules

* `expenses`: Form + list view
* `dashboard`: Summary charts + alerts
* `auth`: Register/Login
* `habits`: Daily tracker

### State Handling

* Services using HttpClient + RxJS
* JWT token stored in localStorage

---

## 8. Authentication & Security

* JWT Authentication (Bearer Token)
* [Authorize] attributes on protected controllers
* Password hashing and token expiration
* Angular route guards

---

## 9. CI/CD & Deployment

* **CI:** GitHub Actions or Jenkins for build/test pipeline
* **CD:** Octopus Deploy or manual Azure deployment
* Angular frontend deploy via Netlify/Vercel/Azure Static Web Apps
* AppSettings for environment configs

---

## 10. Testing Strategy

* **xUnit**: Service logic, controller integration
* **Angular**: Jasmine/Karma for components and guards
* Mock repositories for isolated logic testing

---

## 11. Setup Instructions

### 🔧 Prerequisites

* .NET SDK 8.0
* Node.js 18+
* SQL Server Express or LocalDB

### ⚙️ Backend Setup

```bash
cd MoneyPilot.API
dotnet ef database update
dotnet run
```

### 🌐 Frontend Setup

```bash
cd money-pilot-frontend
npm install
ng serve
```

---

## 12. Contributions & Extensions

### 🚀 Future Enhancements

* Azure App deployment
* Email/SMS notifications
* PWA support for offline access
* Multi-tenant organization support


---

## 13. Current Backend Capabilities (As Implemented)

### 🔐 Authentication

- JWT-based authentication (HS256)
- Role-based access control (`User`, `Admin`)
- Protected endpoints using `[Authorize]`
- Development test user seeding
- Token expiration configured via `appsettings.json`

### 💳 Finance Modules (Implemented)

- Categories (Full CRUD)
- Expenses (Full CRUD)
- Budgets (Full CRUD)
- Recurring Transactions (CRUD + Background processing)
- Dashboard Summary endpoint

---

## 14. Security Architecture

- JWT Bearer Authentication
- ASP.NET Identity password hashing
- Configured CORS policy for Angular development server
- HTTPS redirection enabled
- Security headers configured:
  - Content-Security-Policy
  - X-Frame-Options
  - X-Content-Type-Options
  - Strict-Transport-Security

---

## 15. API Response Pattern

All API responses follow a consistent wrapper:

```json
{
  "success": true,
  "message": "Optional message",
  "data": {}
}
```

### 🤝 Contribution Guidelines

* Fork + PRs welcome
* Follow folder conventions
* Write unit tests for features

---

## 📌 Author
Praveen Kashyap(coder-pro10z)
