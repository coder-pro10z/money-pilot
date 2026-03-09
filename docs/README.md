![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Angular](https://img.shields.io/badge/Angular-17-red)
![License](https://img.shields.io/badge/License-MIT-green)
![Release](https://img.shields.io/badge/Release-v0.0.0-blue)
![Status](https://img.shields.io/badge/Status-Active-success)
![API Docs](https://img.shields.io/badge/API-Swagger-green)
![Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-blue)
# 🧾 MoneyPilot Project Documentation

## 1. Project Overview

**MoneyPilot** is a full-stack finance and productivity web application that enables users to manage personal expenses, track budgets and investments, and build good financial habits. It is designed with a modular architecture using ASP.NET Core and Angular, following best practices for security, maintainability, and scalability.

## 🚀 Live Demo

| Service | Link |
|-------|------|
Frontend | https://money-pilot-opal.vercel.app
Backend API | https://money-pilot-webapi.onrender.com
Swagger API Docs | https://money-pilot-webapi.onrender.com/swagger

---

## 🏷 Release Version

Current Release: **v0.0.0**

Initial production-ready release of MoneyPilot.

---

## 📸 Application Screenshots

### Dashboard
![Dashboard Screenshot](docs/screenshots/dashboard.png)

### Expense Management
![Expenses Screenshot](docs/screenshots/expenses.png)

### Budget Tracking
![Budget Screenshot](docs/screenshots/budgets.png)

---

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

## 🛠 Tech Stack

### Backend

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-blue)
![Entity Framework](https://img.shields.io/badge/Entity_Framework-Core-green)
![C#](https://img.shields.io/badge/C%23-Language-blue)
![LINQ](https://img.shields.io/badge/LINQ-Queries-orange)
![FluentValidation](https://img.shields.io/badge/FluentValidation-Validation-red)

---

### Frontend

![Angular](https://img.shields.io/badge/Angular-17-red)
![TypeScript](https://img.shields.io/badge/TypeScript-Language-blue)
![Bootstrap](https://img.shields.io/badge/Bootstrap-UI-purple)
![Chart.js](https://img.shields.io/badge/Chart.js-Analytics-yellow)

---

### Database

![SQL Server](https://img.shields.io/badge/SQL_Server-Database-red)
![Entity Framework](https://img.shields.io/badge/EF_Core-ORM-green)

---

### Deployment & DevOps

![Render](https://img.shields.io/badge/Backend-Render-black)
![Vercel](https://img.shields.io/badge/Frontend-Vercel-black)
![GitHub](https://img.shields.io/badge/Source-GitHub-lightgrey)
![Swagger](https://img.shields.io/badge/API-Swagger-green)
---

---

# 🏗 System Architecture

MoneyPilot follows **Clean Architecture principles** with a layered backend and modular Angular frontend.


### 📐 Layered Design (Clean Architecture)

```
                    ┌──────────────────────┐
                    │      Frontend        │
                    │      Angular 17      │
                    │  (Hosted on Vercel)  │
                    └──────────┬───────────┘
                               │
                               │ HTTP + JWT
                               ▼
                    ┌──────────────────────┐
                    │   ASP.NET Core API   │
                    │   (Render Hosting)   │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │  Application Layer   │
                    │   Business Logic     │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │ Infrastructure Layer │
                    │ EF Core / Repos      │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │      Database        │
                    │  SQL Server/Postgres │
                    └──────────────────────┘
```
### Architecture Highlights

MoneyPilot implements **Clean Architecture** to ensure separation of concerns and maintainability.

Key principles used:

- **Domain Layer** → Core business entities
- **Application Layer** → Use cases and services
- **Infrastructure Layer** → Database and external integrations
- **API Layer** → Controllers and request handling

Benefits:

- Testable architecture
- Clear dependency flow
- Maintainable and scalable system

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

---

# 🔐 Authentication APIs

| Method | Endpoint | Description |
|------|------|------|
POST | `/api/auth/register` | Register a new user |
POST | `/api/auth/login` | Authenticate user and return JWT token |
POST | `/api/auth/refresh` | Refresh authentication token |
POST | `/api/auth/logout` | Logout user |

---

# 💰 Expense APIs

| Method | Endpoint | Description |
|------|------|------|
GET | `/api/expenses` | Get all expenses |
GET | `/api/expenses/{id}` | Get expense by ID |
POST | `/api/expenses` | Create new expense |
PUT | `/api/expenses/{id}` | Update expense |
DELETE | `/api/expenses/{id}` | Delete expense |
GET | `/api/expenses/summary` | Get expense summary for dashboard |

---

# 📊 Budget APIs

| Method | Endpoint | Description |
|------|------|------|
GET | `/api/budgets` | List all budgets |
GET | `/api/budgets/{id}` | Get budget details |
POST | `/api/budgets` | Create budget |
PUT | `/api/budgets/{id}` | Update budget |
DELETE | `/api/budgets/{id}` | Delete budget |
GET | `/api/budgets/current` | Get active budget |

---

# 🔁 Recurring Transaction APIs

| Method | Endpoint | Description |
|------|------|------|
GET | `/api/recurring-transactions` | Get all recurring transactions |
GET | `/api/recurring-transactions/{id}` | Get transaction by ID |
POST | `/api/recurring-transactions` | Create recurring transaction |
PUT | `/api/recurring-transactions/{id}` | Update recurring transaction |
DELETE | `/api/recurring-transactions/{id}` | Delete recurring transaction |
GET | `/api/recurring-transactions/due` | Get transactions due for execution |
POST | `/api/recurring-transactions/process-due` | Trigger processing of due transactions |

---

# 🏥 Monitoring APIs

| Method | Endpoint | Description |
|------|------|------|
GET | `/health` | Application health check |
GET | `/health/background-service` | Background service status |
GET | `/diagnostics/startup` | Startup diagnostics |
GET | `/api/logs/recent` | Retrieve recent logs |
POST | `/seed` | Seed test data |
GET | `/test-db` | Test database connection |

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

---

## 🧠 Architecture Principles

MoneyPilot follows enterprise-grade development principles:

- Clean Architecture
- SOLID Principles
- Repository Pattern
- Unit of Work
- DTO-based API design
- Modular Angular architecture
- Secure JWT authentication

---

## 📦 Release History

| Version | Description |
|------|------|
v0.0.0 | Initial portfolio release |

---

## ⭐ Project Purpose

This project demonstrates:

- Full-stack system design
- Secure API development
- Clean architecture in .NET
- Angular enterprise frontend
- Production-ready development practices

---

## 📌 Author

**Praveen Kashyap**

GitHub  
https://github.com/coder-pro10z

LinkedIn  
https://linkedin.com/in/coder-pro10z