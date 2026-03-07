# 🚀 MoneyPilot Deployment Guide

This document describes how to deploy **MoneyPilot** to a
production-ready cloud stack.

The application is deployed using a modern full-stack architecture:

Frontend → Vercel (Angular)\
Backend → Render (ASP.NET Core API)\
Database → Render PostgreSQL

------------------------------------------------------------------------

# 🧭 Deployment Order

1.  PostgreSQL Database (Render)\
2.  Backend API (Render)\
3.  Frontend Application (Vercel)\
4.  Connect services\
5.  End-to-end verification

------------------------------------------------------------------------

# 1️⃣ PostgreSQL Database (Render)

1.  Go to: https://dashboard.render.com\
2.  Click **New → PostgreSQL**\
3.  Configure:

Name: moneypilot-db\
Plan: Free\
Region: Closest available

Render will generate credentials:

Host\
Database\
User\
Password\
Port

------------------------------------------------------------------------

# 2️⃣ Connection String for ASP.NET

Format:

Host=HOST; Port=5432; Database=DBNAME; Username=USER; Password=PASSWORD;
SSL Mode=Require

Example:

Host=dpg-xxxx.render.com; Port=5432; Database=moneypilot_db;
Username=moneypilot_user; Password=xxxx; SSL Mode=Require

------------------------------------------------------------------------

# 3️⃣ Backend Preparation (.NET)

Install PostgreSQL provider:

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

Update Program.cs:

builder.Services.AddDbContext`<MoneyPilotDbContext>`{=html}(options =\>
options.UseNpgsql(
builder.Configuration.GetConnectionString("DefaultConnection") ) );

------------------------------------------------------------------------

# 4️⃣ Reset Migrations (If switching DB providers)

Delete:

Infrastructure/Migrations

Then recreate:

dotnet ef migrations add InitialPostgres\
dotnet ef database update

Verify locally:

-   Login
-   Expense CRUD
-   Budget CRUD
-   Dashboard

------------------------------------------------------------------------

# 5️⃣ Deploy Backend to Render

Render → New → Web Service

Runtime: .NET

Root Directory:

backend/src/MoneyPilot.API

Build Command:

dotnet publish -c Release -o out

Start Command:

dotnet out/MoneyPilot.API.dll

------------------------------------------------------------------------

# 6️⃣ Render Environment Variables

ASPNETCORE_ENVIRONMENT=Production

ConnectionStrings\_\_DefaultConnection=
Host=...;Port=5432;Database=...;Username=...;Password=...;SSL
Mode=Require

Jwt\_\_Key=YOUR_SECURE_KEY\
Jwt\_\_Issuer=MoneyPilotAPI\
Jwt\_\_Audience=MoneyPilotUsers

------------------------------------------------------------------------

# 7️⃣ Configure CORS

Program.cs:

builder.Services.AddCors(options =\> {
options.AddPolicy("ProductionPolicy", policy =\> { policy.WithOrigins(
"https://moneypilot.vercel.app" ) .AllowAnyHeader() .AllowAnyMethod();
}); });

app.UseCors("ProductionPolicy");

------------------------------------------------------------------------

# 8️⃣ Test Backend

Example:

https://moneypilot-api.onrender.com/swagger

------------------------------------------------------------------------

# 9️⃣ Deploy Angular Frontend to Vercel

Import GitHub repo in:

https://vercel.com

Root Directory:

frontend

Build Command:

npm run build

Output Directory:

dist/money-pilot

------------------------------------------------------------------------

# 🔟 Configure Angular API URL

environment.prod.ts

export const environment = { production: true, apiBase:
"https://moneypilot-api.onrender.com/api" };

Push commit → Vercel auto deploys.

------------------------------------------------------------------------

# 🧪 End-to-End Tests

Login\
Expense creation\
Budget CRUD\
Dashboard analytics

------------------------------------------------------------------------

# ⚠️ Render Free Tier

Services may sleep after inactivity.\
First request can take \~30 seconds.

------------------------------------------------------------------------

# 🏗 Final Architecture

Internet\
↓\
Angular Frontend (Vercel)\
↓ REST API\
ASP.NET Core API (Render)\
↓ EF Core\
PostgreSQL Database (Render)

------------------------------------------------------------------------

# 💼 Production Stack

Angular 17\
ASP.NET Core Web API\
PostgreSQL\
JWT Authentication\
Render Cloud Infrastructure\
Vercel Hosting\
Entity Framework Core
