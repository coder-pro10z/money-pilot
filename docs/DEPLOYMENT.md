# MoneyPilot – Full-Stack Deployment Guide

This document provides step‑by‑step instructions for deploying the **MoneyPilot** application to production.  
The stack consists of:

- **Frontend:** Angular 17+ hosted on [Vercel](https://vercel.com)
- **Backend:** ASP.NET Core 8 Web API hosted on [Render](https://render.com) (Dockerized)
- **Database:** PostgreSQL hosted on Render

---

## 📋 Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Prerequisites](#prerequisites)
3. [Local Development Setup](#local-development-setup)
4. [Deployment Order](#deployment-order)
5. [Database Setup (Render PostgreSQL)](#database-setup-render-postgresql)
6. [Backend Deployment (Render)](#backend-deployment-render)
   - [6.1 Prepare the Dockerfile](#61-prepare-the-dockerfile)
   - [6.2 Create a Render Web Service](#62-create-a-render-web-service)
   - [6.3 Environment Variables](#63-environment-variables)
   - [6.4 Deploy](#64-deploy)
7. [Frontend Deployment (Vercel)](#frontend-deployment-vercel)
   - [7.1 Environment Files](#71-environment-files)
   - [7.2 Configure angular.json](#72-configure-angularjson)
   - [7.3 npm Scripts (Optional)](#73-npm-scripts-optional)
   - [7.4 Create Vercel Project](#74-create-vercel-project)
   - [7.5 Redeploy](#75-redeploy)
8. [CORS Configuration](#cors-configuration)
9. [Database Migrations](#database-migrations)
10. [Post‑Deployment Verification](#post-deployment-verification)
11. [Troubleshooting](#troubleshooting)
12. [Environment Variables Summary](#environment-variables-summary)

---

## 🏗 Architecture Overview

```
Internet
    │
    ▼
Angular Frontend (Vercel)
    │  (REST API calls)
    ▼
ASP.NET Core API (Render – Docker)
    │
    ▼
PostgreSQL Database (Render)
```

- **Frontend** serves the Angular application and communicates with the backend via HTTPS.
- **Backend** exposes a REST API, handles business logic, and connects to the PostgreSQL database.
- **Database** stores all application data.

---

## ✅ Prerequisites

- [Render](https://render.com) account (for backend and database)
- [Vercel](https://vercel.com) account (for frontend)
- Git repository hosting (e.g., GitHub) with the project code
- [.NET 8 SDK](https://dotnet.microsoft.com/download) installed locally (for development/migrations)
- [Node.js](https://nodejs.org/) (v18 or later) and npm (for frontend development)
- PostgreSQL client (optional, for manual checks)

---

## 💻 Local Development Setup

Clone the repository and navigate to the project root.

### Backend
```bash
cd backend/src
dotnet restore
dotnet build
# Update database (if using local SQL Server or PostgreSQL)
dotnet ef database update
dotnet run --project MoneyPilot.API
```
The API will be available at `https://localhost:44391` (or `http://localhost:5000`).

### Frontend
```bash
cd frontend
npm install
ng serve
```
Navigate to `http://localhost:4200`. The development server proxies API requests to the local backend (see `proxy.conf.json`).

---

## 🧭 Deployment Order

1. **PostgreSQL Database** on Render  
2. **Backend API** on Render  
3. **Frontend** on Vercel  
4. **CORS Configuration** and final verification  

---

## 1️⃣ Database Setup (Render PostgreSQL)

1. Log in to [Render Dashboard](https://dashboard.render.com).  
2. Click **New +** → **PostgreSQL**.  
3. Fill in the details:
   - **Name:** `moneypilot-db` (or any unique name)
   - **Database:** leave as default or set your own
   - **User:** leave as default
   - **Region:** choose the closest to your users (e.g., Singapore)
   - **Plan:** Free (or upgrade as needed)
4. Click **Create Database**.  
5. After creation, note the **Internal Connection String** and the individual fields (Host, Port, Database, User, Password). You will need them for the backend.

---

## 2️⃣ Backend Deployment (Render)

### 2.1 Prepare the Dockerfile

Ensure a `Dockerfile` exists at the **root of your repository** (where `.git` is located). Use the following content (adjust project paths if your folder structure differs):

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and all project files
COPY backend/src/*.sln ./
COPY backend/src/MoneyPilot.API/*.csproj ./MoneyPilot.API/
COPY backend/src/MoneyPilot.Application/*.csproj ./MoneyPilot.Application/
COPY backend/src/MoneyPilot.Infrastructure/*.csproj ./MoneyPilot.Infrastructure/
COPY backend/src/MoneyPilot.Domain/*.csproj ./MoneyPilot.Domain/
COPY backend/src/MoneyPilot.Tests/*.csproj ./MoneyPilot.Tests/
COPY backend/src/MoneyPilot.SecurityHeaders/*.csproj ./MoneyPilot.SecurityHeaders/

# Restore dependencies
RUN dotnet restore

# Copy everything else and publish
COPY backend/src/ ./
RUN dotnet publish MoneyPilot.API -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "MoneyPilot.API.dll"]
```

> **Note:** If your projects are not under `backend/src`, adjust the `COPY` paths accordingly.

### 2.2 Create a Render Web Service

1. In the Render Dashboard, click **New +** → **Web Service**.  
2. Connect your GitHub repository.  
3. Configure the service:
   - **Name:** `money-pilot-api`
   - **Environment:** `Docker`
   - **Region:** same as your database (recommended for lower latency)
   - **Branch:** `release-v0.0.0` (or your production branch)
   - **Root Directory:** **leave empty** (the Dockerfile is at the repo root)
   - **Build Command:** leave empty (handled by Dockerfile)
   - **Start Command:** leave empty (handled by Dockerfile)
   - **Instance Type:** Free (or paid)
4. Click **Create Web Service**.

### 2.3 Environment Variables

After creation, go to your service **Settings** → **Environment** and add the following variables:

| Key | Value |
|-----|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | `Host=your-render-postgres-host;Port=5432;Database=your-db;Username=your-user;Password=your-password;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;Maximum Pool Size=20` |
| `Jwt__Key` | `your-secure-key-at-least-32-chars` |
| `Jwt__Issuer` | `MoneyPilotAPI` |
| `Jwt__Audience` | `MoneyPilotUsers` |

Replace the connection string with the actual values from your Render PostgreSQL instance. The double underscore `__` in the key represents the colon `:` in `IConfiguration` (e.g., `ConnectionStrings:DefaultConnection`).

### 2.4 Deploy

Render will automatically build and deploy the service. Once finished, note the service URL (e.g., `https://money-pilot-api.onrender.com`). You can test it by visiting `https://money-pilot-api.onrender.com/swagger` (if Swagger is enabled only in development, it won’t appear – instead visit the root `/` to see the welcome message).

---

## 3️⃣ Frontend Deployment (Vercel)

### 3.1 Environment Files

Make sure you have two environment files in `frontend/src/environments/`:

- `environment.ts` (development) – usually contains `apiBase: 'https://localhost:44391/api'`
- `environment.prod.ts` (production) – must contain your live backend URL.

**Example `environment.prod.ts`**:
```typescript
export const environment = {
  production: true,
  apiBase: 'https://money-pilot-api.onrender.com/api'   // No trailing slash!
};
```

### 3.2 Configure `angular.json`

Verify that the `production` configuration in `angular.json` includes the correct file replacement:

```json
"configurations": {
  "production": {
    "fileReplacements": [
      {
        "replace": "src/environments/environment.ts",
        "with": "src/environments/environment.prod.ts"
      }
    ],
    "budgets": [ ... ],
    "outputHashing": "all"
  }
}
```

### 3.3 npm Scripts (Optional)

In `frontend/package.json`, add these scripts for convenience:

```json
"scripts": {
  "build:prod": "ng build --configuration production",
  "serve:prod": "ng serve --configuration production"
}
```

### 3.4 Create Vercel Project

1. Go to [Vercel Dashboard](https://vercel.com) → **Add New** → **Project**.  
2. Import your GitHub repository.  
3. Configure the project:
   - **Root Directory:** If your Angular app is in a subfolder (e.g., `frontend`), set it to `frontend`. Otherwise leave empty.
   - **Build Command:** `npm run build:prod` (or `ng build --configuration production`)
   - **Output Directory:** `dist/frontend/browser` (this is where `index.html` is placed after build – verify the exact path from your `angular.json` `outputPath`)
   - **Install Command:** `npm install` (default)
4. Click **Deploy**.

Vercel will build and deploy your frontend. After deployment, you’ll get a URL like `https://money-pilot.vercel.app`.

### 3.5 Redeploy (if settings changed)

If you change any settings, push a new commit or use the **Redeploy** button in the Vercel dashboard.

---

## 🔒 CORS Configuration (Backend)

Edit `Program.cs` in the backend to allow requests from your Vercel frontend and any local test origins:

```csharp
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://your-vercel-app.vercel.app",   // Replace with your actual Vercel URL
            "http://localhost:3000",                 // For testing built app locally (serve)
            "http://localhost:4200"                   // For ng serve (development)
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();   // Include if you use cookies or authorization headers
    });
});

// After app building, before app.UseAuthorization() and app.MapControllers()
app.UseCors("AllowFrontend");
```

Redeploy the backend on Render after making this change.

---

## 🔄 Database Migrations

If you want the backend to automatically apply pending migrations on startup, add this to `Program.cs`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MoneyPilotDbContext>();
    dbContext.Database.Migrate();
}
```

Place this **after** `app` is built but before `app.Run()`. This ensures the database schema is up‑to‑date when the container starts.

---

## 🧪 Post‑Deployment Verification

1. **Backend Health Check**  
   Visit `https://money-pilot-api.onrender.com/` – you should see the message:  
   `🎉 MoneyPilot API is running!`

2. **Frontend**  
   Open your Vercel URL. Open browser Developer Tools (F12) → **Network** tab.  
   Perform a login or any action that calls the API.  
   Verify that requests go to `https://money-pilot-api.onrender.com/api/...` and return HTTP 200.

3. **Database**  
   You can connect to the Render PostgreSQL instance using a client (e.g., psql, pgAdmin) and check that tables have been created.

---

## ⚠️ Troubleshooting

| Issue | Likely Cause & Solution |
|-------|-------------------------|
| **Build fails with “The specified Root Directory does not exist”** | In Vercel, the **Root Directory** should point to the folder containing `package.json`, not the output folder. Set it to `frontend` (if applicable) or leave empty. |
| **API calls still go to `localhost`** | The production environment file (`environment.prod.ts`) is not being used. Ensure Vercel build command includes `--configuration production` and that file replacements are correct in `angular.json`. |
| **CORS errors (No 'Access-Control-Allow-Origin' header)** | Add your frontend origin to the backend CORS policy and redeploy. Check that the origin matches exactly (including https://). |
| **DateTime errors with PostgreSQL** | Add this line to `Program.cs` before building the app: `AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);` |
| **Swagger visible in production** | Wrap Swagger middleware in `if (app.Environment.IsDevelopment())`. |
| **Render service sleeps on free tier** | Free services spin down after inactivity. The first request after a period of inactivity may take 30+ seconds. This is normal. |

---

## 🌐 Environment Variables Summary

### Backend (Render)

| Variable | Purpose |
|----------|---------|
| `ASPNETCORE_ENVIRONMENT` | Set to `Production` |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `Jwt__Key` | Secret key for JWT tokens (min 32 chars) |
| `Jwt__Issuer` | JWT issuer (e.g., `MoneyPilotAPI`) |
| `Jwt__Audience` | JWT audience (e.g., `MoneyPilotUsers`) |

### Frontend (Vercel)

No environment variables are required if the `apiBase` is hardcoded in `environment.prod.ts`. If you prefer to set it via Vercel environment variables, you can create a custom build step that injects the value, but the simplest approach is to use the static file.

---

## ✅ Summary

- **Database:** Render PostgreSQL (free tier)
- **Backend:** Dockerized .NET 8 API on Render
- **Frontend:** Angular app on Vercel, built with production configuration
- **CORS:** Configured to allow communication between frontend and backend
- **Environment:** Production settings via environment variables and file replacements

Your MoneyPilot application is now live and accessible to users!