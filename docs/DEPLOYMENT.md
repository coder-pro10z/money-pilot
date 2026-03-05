Perfect — this is a **very solid production stack** 👇

```
Frontend  →  Vercel (Angular)
Backend   →  Render (.NET API)
Database  →  Render PostgreSQL
```

This architecture is **clean, modern, and recruiter-friendly**.

I’ll give you the **exact deployment pipeline in the correct order** so nothing breaks.

---

# 🚀 DEPLOYMENT EXECUTION PLAN

We deploy in this order:

```
1️⃣ Database (Render PostgreSQL)
2️⃣ Backend API (Render)
3️⃣ Frontend Angular (Vercel)
4️⃣ Connect everything
5️⃣ Test end-to-end
```

---

# 🧱 STEP 1 — Create PostgreSQL Database (Render)

Go to:

```
https://dashboard.render.com
```

### Create Database

```
New → PostgreSQL
```

Fill:

```
Name: moneypilot-db
Plan: Free
Region: Closest to you
```

Render will generate credentials.

You will see something like:

```
Host
Database
User
Password
Port
```

Example connection string:

```
postgres://user:password@host:5432/dbname
```

---

# 🧱 STEP 2 — Convert Connection String for .NET

ASP.NET uses this format:

```
Host=HOST;
Port=5432;
Database=DBNAME;
Username=USER;
Password=PASSWORD;
```

Example:

```
Host=dpg-xxxx.render.com;
Port=5432;
Database=moneypilotdb;
Username=moneypilot_user;
Password=xxxx;
```

Save this.

You will use it in **Render environment variables**.

---

# 🧱 STEP 3 — Prepare Backend for Render

## Install PostgreSQL EF Provider

In backend project:

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

---

## Update `Program.cs`

Replace SQL Server with PostgreSQL:

```csharp
builder.Services.AddDbContext<MoneyPilotDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
```

---

## Remove SQL Server package

Optional but cleaner:

```
Microsoft.EntityFrameworkCore.SqlServer
```

---

# 🧱 STEP 4 — Reset Migrations (Important)

Because provider changed.

Delete:

```
/Migrations
```

Then run:

```bash
dotnet ef migrations add InitialPostgres
dotnet ef database update
```

Test locally first.

Verify:

```
Login works
Expense CRUD works
Budget CRUD works
Dashboard works
```

---

# 🧱 STEP 5 — Push Backend to GitHub

Your repo should look like:

```
MoneyPilot
 ├ backend
 │   ├ src
 │   └ MoneyPilot.API.csproj
```

Push to GitHub.

---

# 🧱 STEP 6 — Deploy Backend to Render

In Render:

```
New → Web Service
```

Connect GitHub repo.

Settings:

### Environment

```
Runtime: .NET
```

---

### Build Command

```
dotnet publish -c Release -o out
```

---

### Start Command

```
dotnet out/MoneyPilot.API.dll
```

---

# 🧱 STEP 7 — Add Environment Variables (Render)

Open **Environment tab**.

Add:

### Environment

```
ASPNETCORE_ENVIRONMENT = Production
```

---

### Connection String

```
ConnectionStrings__DefaultConnection = Host=...;Port=5432;Database=...;Username=...;Password=...
```

---

### JWT

```
Jwt__Key = YOUR_SECURE_KEY
Jwt__Issuer = MoneyPilotAPI
Jwt__Audience = MoneyPilotUsers
```

---

# 🧱 STEP 8 — Enable CORS for Vercel

In backend `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy",
        policy =>
        {
            policy.WithOrigins(
                "https://your-vercel-app.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
```

Then:

```
app.UseCors("ProductionPolicy");
```

---

# 🧱 STEP 9 — Test Backend

After deployment Render gives URL:

```
https://moneypilot-api.onrender.com
```

Test:

```
https://moneypilot-api.onrender.com/swagger
```

If swagger works → backend deployed.

---

# 🧱 STEP 10 — Deploy Angular to Vercel

Go to:

```
https://vercel.com
```

Import GitHub repo.

---

### Build Settings

Framework:

```
Other
```

Build command:

```
npm run build
```

Output directory:

```
dist/moneypilot-frontend
```

---

# 🧱 STEP 11 — Update Angular API URL

In:

```
environment.prod.ts
```

Replace:

```ts
apiBase: 'https://moneypilot-api.onrender.com/api'
```

Rebuild.

---

# 🧱 STEP 12 — Redeploy Frontend

Push commit.

Vercel auto deploys.

You get URL like:

```
https://moneypilot.vercel.app
```

---

# 🧱 STEP 13 — End-to-End Test

Test these flows:

### Login

```
Frontend → Render API
```

---

### Create Expense

```
Angular → API → PostgreSQL
```

---

### Budget CRUD

```
Angular → API → PostgreSQL
```

---

### Dashboard

```
Angular → API aggregates → DB
```

---

# ⚠️ Common Render Issue

Free tier sleeps after inactivity.

First request takes ~30 seconds.

Normal.

---

# 🏁 FINAL ARCHITECTURE

```
                 Internet
                     │
                     │
         https://moneypilot.vercel.app
                     │
                     ▼
        Angular Frontend (Vercel)
                     │
                     │ REST API
                     ▼
     ASP.NET Core API (Render Web Service)
                     │
                     │ EF Core
                     ▼
        PostgreSQL Database (Render)
```

---

# 💼 What You Now Have

A **production deployed SaaS app**:

* Angular 17
* ASP.NET Core API
* PostgreSQL
* Render Cloud
* Vercel Hosting
* JWT Auth
* Background services

This is **exactly the kind of architecture companies use**.

---

# 🚀 If you want, next I can also show you

* **How to fix Angular routing on Vercel (very important)**
* **How to add CI/CD pipeline**
* **How to add custom domain**
* **How to avoid Render cold starts**
* **How to monitor API logs**

Those 4 things turn this into a **senior-level deployment**.
