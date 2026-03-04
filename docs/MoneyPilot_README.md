# 🚀 MoneyPilot

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Angular](https://img.shields.io/badge/Angular-17-red)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![Status](https://img.shields.io/badge/status-active-success)

MoneyPilot is a full‑stack personal finance management application built
with **ASP.NET Core 8** and **Angular 17**.

It demonstrates real-world engineering practices including: - Clean
Architecture - JWT authentication - Background services - Modular
Angular frontend - Secure API design

------------------------------------------------------------------------

## ✨ Features

### Authentication

-   JWT login and registration
-   ASP.NET Identity password hashing
-   Angular route guards

### Expense Management

-   Add / Edit / Delete expenses
-   Category-based tracking
-   Monthly summaries

### Budget Tracking

-   Monthly budget limits
-   Category budgets
-   Spending progress

### Recurring Transactions

-   Automated recurring expenses
-   Background processing service

### Dashboard

-   Financial overview
-   Category summaries
-   Monthly trends

------------------------------------------------------------------------

## 🏗 Architecture
```
Angular Frontend
    ↓
API Controllers
    ↓
Application Layer
    ↓
Infrastructure Layer
    ↓
Domain Layer
    ↓
SQL Server
```
Clean Architecture ensures maintainable, testable, and scalable code.

------------------------------------------------------------------------

## 🛠 Tech Stack

### Backend

-   ASP.NET Core 8
-   C#
-   Entity Framework Core
-   SQL Server
-   JWT Authentication
-   Serilog
-   Swagger

### Frontend

-   Angular 17
-   TypeScript
-   RxJS
-   Reactive Forms

------------------------------------------------------------------------

## 📂 Project Structure
```
MoneyPilot
 ├── backend 
 │        │ 
 │        ├── MoneyPilot.API 
 │        │ 
 │        ├── MoneyPilot.Application
 │        │ 
 │        ├── MoneyPilot.Domain 
 │        │ 
 │        └── MoneyPilot.Infrastructure 
 ├── frontend 
 │        │
 │        └── Angular app 
 └── docs
```
------------------------------------------------------------------------

## 🔌 API Endpoints

### Authentication

- POST /api/auth/register
- POST /api/auth/login

### Expenses

- GET /api/expense
- POST /api/expense
- PUT /api/expense/{id}
- DELETE /api/expense/{id}

### Budgets

- GET /api/budget
- POST /api/budget

### Recurring Transactions

- GET /api/recurring-transactions
- POST /api/recurring-transactions
- PUT /api/recurring-transactions/{id}
- DELETE /api/recurring-transactions/{id}

------------------------------------------------------------------------

## 🔐 Security

-   JWT Bearer authentication
-   ASP.NET Identity password hashing
-   Role-based authorization
-   HTTPS enforcement
-   Security headers

------------------------------------------------------------------------

## ⚙️ Local Setup

### Prerequisites

-   .NET 8 SDK
-   Node.js 18+
-   SQL Server

### Backend
```
cd backend/src/MoneyPilot.API
dotnet restore
dotnet ef database update
dotnet run
```
API runs at: https://localhost:44391

Swagger: https://localhost:44391/swagger

### Frontend
```
cd frontend
npm install
ng serve
```
App runs at: http://localhost:4200

------------------------------------------------------------------------

## 📊 Project Status
```
Authentication --- Complete
Expenses --- Complete
Budgets --- Complete
Recurring Transactions --- Complete
Dashboard --- In Progress
```
------------------------------------------------------------------------

## 👨‍💻 Author

Praveen Kashyap
GitHub: https://github.com/coder-pro10z

MoneyPilot demonstrates production‑style architecture used in modern
SaaS applications.
