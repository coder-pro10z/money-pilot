using HealthChecks.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoneyPilot.Application.Configs;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;
using MoneyPilot.Domain.Enums;
using MoneyPilot.Infrastructure.Data;
using MoneyPilot.Infrastructure.Repositories;
using MoneyPilot.Infrastructure.Services;
using MoneyPilot.SecurityHeaders.Extensions;
using Serilog;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
//using MoneyPilot.Application.Configs;

// At the VERY TOP of Program.cs


//start of try Block
try
{

    // ====================== LOGGER ======================
    //Configure Serilog
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}")
        .WriteTo.File("Logs/moneypilot_api_log.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{exception}"
            )
        .CreateLogger();


    //First thing to run - before anything else
    
    
    Log.Information("Starting MoneyPilot API...");
    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog (ADD THIS LINE)
    builder.Host.UseSerilog();

    //
    // ====================== SERVICES ======================
    //

    // Controllers + JSON (cycle-safe)
    builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

    //var provider = builder.Configuration["DatabaseProvider"];
    //var connection = builder.Configuration.GetConnectionString("DefaultConnection");

    //builder.Services.AddDbContext<MoneyPilotDbContext>(options =>
    //{
    //    if (provider == "Postgres")
    //    {
    //        options.UseNpgsql(connection);
    //    }
    //    else
    //    {
    //        options.UseSqlServer(connection);
    //    }
    //});
    //// DbContext
    //builder.Services.AddDbContext<MoneyPilotDbContext>(options =>
    //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    var provider = builder.Configuration["DatabaseProvider"];
    var connection = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<MoneyPilotDbContext>(options =>
    {
        if (provider == "Postgres")
        {
            options.UseNpgsql(connection,
                x => x.MigrationsAssembly("MoneyPilot.Infrastructure"));
        }
        else
        {
            options.UseSqlServer(connection,
                x => x.MigrationsAssembly("MoneyPilot.Infrastructure"));
        }
    });

    var conn = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"ConnectionString: {conn}");

    /// use PostgreSQL with Npgsql.EntityFrameworkCore.PostgreSQL package
    //builder.Services.AddDbContext<MoneyPilotDbContext>(options =>
    //    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Identity
    builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<MoneyPilotDbContext>()
    .AddDefaultTokenProviders();

// Repositories & Services
builder.Services.AddScoped<IUnitofWork, UnitOfWork>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
    // Add this with your other service registrations
builder.Services.AddScoped<IRecurringTransactionService, RecurringTransactionService>();
  
  builder.Services.AddScoped<ILoginTokenService, LoginTokenService>();


    // Registering Recurring Transaction Background Service
    builder.Services.Configure<RecurringTransactionConfig>(
        builder.Configuration.GetSection("RecurringTransactionProcessing"));

    builder.Services.AddHostedService<RecurringTransactionBackgroundService>();



    // ====================== HEALTH CHECKS ======================
    // Add this WITH your other service registrations
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<MoneyPilotDbContext>()
        .AddSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
            ,tags: new[] { "database", "sql" });

    //
    // ====================== JWT ======================
    //
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var jwtKey = jwtSettings["Key"]
        ?? throw new InvalidOperationException("JWT Key missing");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ClockSkew = TimeSpan.Zero
        };
    });

    //Add auto login and token Service
    builder.Services.AddScoped<TestUserService>();
    //test token helper 
    builder.Services.AddScoped<TestTokenHelper>();

    builder.Services.AddScoped<LoginTokenService>();

    //
    // ====================== SWAGGER-OLD ======================
    //
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "MoneyPilot API",
            Version = "v1"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT token. Do NOT include 'Bearer ' prefix."
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    });

    //========================CORS========================


    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular",
            policy =>
            {
                policy.WithOrigins("http://localhost:4200", "https://money-pilot-opal.vercel.app", "https://money-pilot-git-main-coders-projects-237f050f.vercel.app", "https://money-pilot-git-release-v000-coders-projects-237f050f.vercel.app", "money-pilot-ec3ext12v-coders-projects-237f050f.vercel.app")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://money-pilot-git-release-v000-coders-projects-237f050f.vercel.app", "https://money-pilot-5vozyg7zf-coders-projects-237f050f.vercel.app", "https://money-pilot-opal.vercel.app")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // if you use cookies/authorization headers
        });
    });

    // After app building

    ///////////FIX 
    ///
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    //
    // ====================== APP ======================
    //


    var app = builder.Build();

    app.UseCors("AllowVercel");
    app.UseCors("AllowFrontend");

    // Apply pending migrations at startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<MoneyPilotDbContext>();
        db.Database.Migrate();
    }

    //
    // ====================== MIDDLEWARE ======================
    //
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
    app.UseHttpsRedirection();
    /// CLEAN DEVELOPMENT API USAGE
   
    }
        app.UseSwaggerUI();

    
    //using custom security headers middleware from MoneyPilot.SecurityHeaders.Extensions
    app.UseMoneyPilotSecurityHeaders();

    app.UseCors("AllowAngular");

    // ⚠️ ORDER MATTERS
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapGet("/", () => Results.Content(GetLandingPageHtml(), "text/html"));

    app.MapGet("/health", () => "Healthy");


    //log ,db, seed sample
    if (app.Environment.IsDevelopment())
    {
        /// CLEAN DEVELOPMENT API USAGE

        //TEST LOGS
        // Add before app.Run()
        app.MapGet("/test", (ILogger<Program> logger) =>
        {
            logger.LogInformation("Test endpoint hit at {Time}", DateTime.UtcNow);
            return Results.Ok(new { message = "Test successful", time = DateTime.UtcNow });
        });

        app.MapGet("/test-db", async (MoneyPilotDbContext db) =>
        {
            try
            {
                var hasRecurringTable = await db.RecurringTransactions.AnyAsync();
                return Results.Ok(new
                {
                    status = "OK",
                    hasRecurringTable,
                    tables = new[] { "RecurringTransactions", "Expenses", "Categories" }
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Database error: {ex.Message}");
            }
        });

        app.MapPost("/seed", async (MoneyPilotDbContext db) =>
        {
            // Create a test category if none exists
            if (!await db.Categories.AnyAsync())
            {
                db.Categories.Add(new Category { Name = "Food" });
                db.Categories.Add(new Category { Name = "Transportation" });
                db.Categories.Add(new Category { Name = "Entertainment" });
                await db.SaveChangesAsync();
            }

            // Get a user (assuming you have at least one)
            var user = await db.Users.FirstOrDefaultAsync();
            if (user == null) return Results.Problem("No users found");

            // Create a sample recurring transaction
            var category = await db.Categories.FirstOrDefaultAsync(c => c.Name == "Food");

            var recurringTransaction = new RecurringTransaction
        {
            UserId = user.Id,
            Description = "Monthly Netflix Subscription",
            Amount = 15.99m,
            CategoryId = category.Id,
            RecurrenceType = RecurrenceType.Monthly, // Keep as string
            Interval = 1,
            DayOfMonth = 15,
            StartDate = DateTime.UtcNow.AddDays(-30),
            NextOccurrence = DateTime.UtcNow.AddDays(5),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        db.RecurringTransactions.Add(recurringTransaction);
        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            message = "Sample data created",
            transactionId = recurringTransaction.Id
        });
        });
    }

    // SIMPLIFIED Auto-login endpoint (development only)
    //test-bg service ,generate-token, 
    if (app.Environment.IsDevelopment())
    {
        app.MapGet("/generate-token", async (TestTokenHelper helper) =>
        {
            var token = await helper.GenerateTokenForTestUserAsync("test@email.com");
            return Results.Content($"""
               {token} 
               """, "text/html");
        });


        app.MapPost("/test-simple", async (MoneyPilotDbContext db) =>
        {
            try
            {
                // Create a simple category without navigation properties
                var category = new Category { Name = "TestCategory" };
                db.Categories.Add(category);
                await db.SaveChangesAsync();

                // Get any user
                var user = await db.Users.FirstOrDefaultAsync();
                if (user == null) return Results.Problem("No users found");

                // Create simple recurring transaction
                var transaction = new RecurringTransaction
                {
                    UserId = user.Id,
                    Description = "Test",
                    Amount = 10.00m,
                    CategoryId = category.Id,
                    // ⚠️ Choose based on your entity type:
                    // If string: RecurrenceType = "Monthly",
                    // If enum: RecurrenceType = RecurrenceType.Monthly,
                    RecurrenceType = RecurrenceType.Monthly, // Adjust this!
                    Interval = 1,
                    StartDate = DateTime.UtcNow,
                    NextOccurrence = DateTime.UtcNow.AddDays(1),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                db.RecurringTransactions.Add(transaction);
                await db.SaveChangesAsync();

                return Results.Ok(new { success = true, id = transaction.Id });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error: {ex.Message}");
            }
        });

        //user miminal api
        // Add this endpoint to Program.cs to check users
        app.MapGet("/users", async (MoneyPilotDbContext db) =>
        {
            var users = await db.Users.ToListAsync();
            return Results.Ok(users.Select(u => new { u.Id, u.Email }));
        });

        app.MapPost("/test-create-user",
            async (MoneyPilotDbContext db, UserManager<AppUser> userManager)
            => {
                try
                {
                    Log.Information("User creation test!");
                    var user = new AppUser { UserName = "tester@email.com", Email = "tester@email.com" };
                    var result = await userManager.CreateAsync(user, "Test@123");
                    return result.Succeeded
            ? Results.Ok(new { message = "User created", userId = user.Id })
            : Results.Problem(string.Join(", ", result.Errors.Select(e => e.Description)));

                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error: {ex.Message}");
                }
            });

        // Test user management endpoints
        app.MapGet("/test-user", async (TestUserService service) =>
        {
            var user = await service.EnsureTestUserCreatedAsync();
            return user != null
                ? Results.Ok(new { email = user.Email, id = user.Id })
                : Results.Problem("Test user not created");
        });

        app.MapGet("/test-token", async (TestTokenHelper helper) =>
        {
            try
            {
                var token = await helper.GenerateTokenForTestUserAsync("test@email.com");
                return Results.Ok(new { token });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        // SIMPLIFIED Auto-login endpoint
        app.MapGet("/auto-login-simple", async (TestTokenHelper helper) =>
        {
            var token = await helper.GenerateTokenForTestUserAsync("test@email.com");

            return Results.Content($"""
        <!DOCTYPE html>
        <html>
        <body style="font-family: Arial; padding: 20px;">
            <h1>MoneyPilot Auto-Login</h1>
            <p><strong>Test User:</strong> test@email.com</p>
            <p><strong>Token:</strong></p>
            <textarea style="width: 100%; height: 100px; font-family: monospace;">{token}</textarea>
            <p>
                <button onclick="navigator.clipboard.writeText('{token}')">📋 Copy Token</button>
                <button onclick="window.open('/swagger', '_blank')">📄 Open Swagger</button>
            </p>
            <p><strong>Usage:</strong> In Swagger, click "Authorize" (top right) and paste: <code>Bearer {token}</code></p>
            <p><a href="/swagger" target="_blank">👉 Open Swagger at https://localhost:44391/swagger</a></p>
        </body>
        </html>
        """, "text/html");
        });


        // Comprehensive startup test
        app.MapGet("/test/startup", async (IRecurringTransactionService service, IServiceProvider sp) =>
        {
            var results = new List<string>();

            // 1. Check if service is registered
            var bgServices = sp.GetServices<IHostedService>();
            var hasBgService = bgServices.Any(s => s.GetType().Name.Contains("RecurringTransaction"));
            results.Add($"Background Service Registered: {(hasBgService ? "✅" : "❌")}");

            // 2. Try to process (should show if working)
            try
            {
                var count = await service.ProcessDueTransactionsAsync();
                results.Add($"Service Method Call Successful: ✅ (processed {count} transactions)");
            }
            catch (Exception ex)
            {
                results.Add($"Service Method Call Failed: ❌ ({ex.Message})");
            }

            // 3. Check configuration
            var config = sp.GetRequiredService<IOptions<RecurringTransactionConfig>>();
            results.Add($"Configuration Loaded: ✅");
            results.Add($"- Enabled: {config.Value.Enabled}");
            results.Add($"- RunOnStartup: {config.Value.RunOnStartup}");
            results.Add($"- Processing Time: {config.Value.ProcessingTime}");

            // 4. Check log file
            var logFile = "Logs/moneypilot_api_log.txt";
            var hasLogFile = File.Exists(logFile);
            results.Add($"Log File Exists: {(hasLogFile ? "✅" : "❌")}");

            if (hasLogFile)
            {
                var logContent = File.ReadLines(logFile).TakeLast(10);
                var hasBgServiceLogs = logContent.Any(l => l.Contains("background service", StringComparison.OrdinalIgnoreCase));
                results.Add($"Background Service Logs Found: {(hasBgServiceLogs ? "✅" : "❌")}");
            }

            return Results.Json(new
            {
                test = "Background Service Startup Test",
                timestamp = DateTime.UtcNow,
                results
            });
        });


        // Add this test endpoint to verify config
        app.MapGet("/debug/config", (IOptions<RecurringTransactionConfig> config) =>
        {
            return Results.Json(config.Value);
        });

        


        // Role seeding: ensure roles and a default admin user exist
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();

                var roles = new[] { "Admin", "User" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Create default admin if missing
                var adminEmail = builder.Configuration["DefaultAdmin:Email"] ?? "admin@localhost";
                var admin = await userManager.FindByEmailAsync(adminEmail);
                if (admin == null)
                {
                    admin = new AppUser { UserName = adminEmail, Email = adminEmail };
                    var result = await userManager.CreateAsync(admin, builder.Configuration["DefaultAdmin:Password"] ?? "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
                else
                {
                    // ensure role
                    if (!await userManager.IsInRoleAsync(admin, "Admin"))
                        await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while seeding roles");
            }
        }

    }

    // Map health checks endpoint
    var healthGroup = app.MapGroup("/health")
    .WithTags("Health");

    //healthGroup.MapGet("/background-service", ...);
    //healthGroup.MapGet("/monitor", ...);
    healthGroup.MapHealthChecks("/");

    //monitor
    healthGroup.MapGet("/monitor", () =>
    {
        return Results.Content("""
        <!DOCTYPE html>
        <html>
        <head>
            <title>Live Monitor</title>
            <script>
                async function updateLogs() {
                    const response = await fetch('/api/logs/recent');
                    const logs = await response.text();
                    document.getElementById('logs').textContent = logs;
                }
                
                setInterval(updateLogs, 2000);
                updateLogs();
            </script>
        </head>
        <body>
            <h1>Live Log Monitor</h1>
            <pre id="logs" style="background: #000; color: #0f0; padding: 10px;"></pre>
        </body>
        </html>
        """, "text/html");
    });

    // Background service health check
    healthGroup.MapGet("/background-service", (IServiceProvider services) =>
    {
        try
        {
            var backgroundServices = services.GetServices<IHostedService>();
            var bgService = backgroundServices.OfType<RecurringTransactionBackgroundService>().FirstOrDefault();

            if (bgService == null)
            {
                return Results.Json(new
                {
                    status = "NOT_FOUND",
                    message = "Background service not registered in DI container",
                    time = DateTime.UtcNow
                });
            }

            // Get configuration
            var config = services.GetRequiredService<IOptions<RecurringTransactionConfig>>();

            return Results.Json(new
            {
                status = "RUNNING",
                service = nameof(RecurringTransactionBackgroundService),
                config = new
                {
                    config.Value.Enabled,
                    config.Value.RunOnStartup,
                    config.Value.ProcessingTime,
                    nextRun = CalculateNextRun(config.Value.ProcessingTime)
                },
                startupTime = DateTime.Now,
                uptime = DateTime.Now - Process.GetCurrentProcess().StartTime
            });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Health check failed: {ex.Message}");
        }
    });


    // Map Diagnostics endpoint
    var diagnosticsGroup = app.MapGroup("/diagnostics")
    .WithTags("Diagnostics");

    //// Startup diagnostics page
    diagnosticsGroup.MapGet("/startup", async (IServiceProvider services) =>
    {
        var html = """
        <!DOCTYPE html>
        <html>
        <head>
            <title>MoneyPilot Startup Diagnostics</title>
            <style>
                body { font-family: Consolas, monospace; padding: 20px; background: #0d1117; color: #c9d1d9; }
                .status-running { color: #3fb950; }
                .status-stopped { color: #f85149; }
                .log-entry { padding: 5px; border-left: 3px solid #30363d; margin: 5px 0; }
                .log-info { border-left-color: #1f6feb; }
                .log-warning { border-left-color: #d29922; }
                .log-error { border-left-color: #f85149; }
            </style>
        </head>
        <body>
            <h1>🔍 MoneyPilot Startup Diagnostics</h1>
            <h2>Background Service Status</h2>
            <div id="service-status">Checking...</div>
            <h2>Recent Logs</h2>
            <div id="logs"></div>
            <h2>Actions</h2>
            <button onclick="testService()">Test Background Service</button>
            <button onclick="checkHealth()">Check Health</button>
            <button onclick="viewLogs()">View Log File</button>
            
            <script>
                async function loadStatus() {
                    const response = await fetch('/health/background-service');
                    const data = await response.json();
                    
                    document.getElementById('service-status').innerHTML = `
                        <p><strong>Status:</strong> <span class="status-${data.status.toLowerCase()}">${data.status}</span></p>
                        <p><strong>Service:</strong> ${data.service}</p>
                        <p><strong>Enabled:</strong> ${data.config?.Enabled}</p>
                        <p><strong>Run on Startup:</strong> ${data.config?.RunOnStartup}</p>
                        <p><strong>Next Run:</strong> ${data.config?.nextRun}</p>
                        <p><strong>Startup Time:</strong> ${data.startupTime}</p>
                    `;
                }
                
                async function testService() {
                    const response = await fetch('/admin/background-service/trigger', { method: 'POST' });
                    const result = await response.json();
                    alert(result.message);
                    loadStatus();
                }
                
                async function checkHealth() {
                    window.open('/health', '_blank');
                }
                
                async function viewLogs() {
                    window.open('/api/logs/recent', '_blank');
                }
                
                // Load status on page load
                loadStatus();
                
                // Auto-refresh every 10 seconds
                setInterval(loadStatus, 10000);
            </script>
        </body>
        </html>
        """;

        return Results.Content(html, "text/html");
    });

    // View recent logs
    diagnosticsGroup.MapGet("/logs/recent", () =>
    {
        //var logFile = "Logs/moneypilot_api_log.txt";
        ////////////
        var logDir = "Logs";
        var pattern = "moneypilot_api_log*.txt";
        var files = Directory.GetFiles(logDir, pattern);

        if (files.Length == 0)
        {
            // No matching log files found
            return Results.Content("No log file found", "text/plain");
        }
        // At least one exists, you can pick the latest:
        var latestFile = files.OrderByDescending(f => f).First();
        var lines = File.ReadLines(latestFile).TakeLast(50);
        return Results.Content(string.Join(Environment.NewLine, lines), "text/plain");
    });

    // Helper function
    DateTime CalculateNextRun(string processingTime)
    {
        if (TimeSpan.TryParse(processingTime, out var time))
        {
            var now = DateTime.Now;
            var scheduled = now.Date.Add(time);
            return now > scheduled ? scheduled.AddDays(1) : scheduled;
        }
        return DateTime.Now.AddDays(1);
    }

    string GetLandingPageHtml() => """
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <title>MoneyPilot API</title>
        <style>
            :root {
                color-scheme: dark;
                --bg: #0f172a;
                --panel: #111c33;
                --panel-border: rgba(148, 163, 184, 0.18);
                --text: #e2e8f0;
                --muted: #94a3b8;
                --primary: #6366f1;
                --primary-hover: #4f46e5;
                --secondary: #1e293b;
                --accent: #22c55e;
                --warning: #f59e0b;
            }

            * {
                box-sizing: border-box;
            }

            body {
                margin: 0;
                min-height: 100vh;
                font-family: Arial, sans-serif;
                background:
                    radial-gradient(circle at top, rgba(99, 102, 241, 0.18), transparent 32%),
                    linear-gradient(180deg, #0b1220 0%, var(--bg) 100%);
                color: var(--text);
            }

            .container {
                max-width: 1040px;
                margin: 0 auto;
                padding: 48px 20px 64px;
            }

            .hero {
                padding: 28px;
                border: 1px solid var(--panel-border);
                border-radius: 20px;
                background: rgba(15, 23, 42, 0.72);
                backdrop-filter: blur(8px);
                box-shadow: 0 24px 60px rgba(2, 6, 23, 0.35);
            }

            .eyebrow {
                display: inline-block;
                margin-bottom: 14px;
                padding: 6px 10px;
                border-radius: 999px;
                background: rgba(99, 102, 241, 0.16);
                color: #c7d2fe;
                font-size: 12px;
                font-weight: 700;
                letter-spacing: 0.08em;
                text-transform: uppercase;
            }

            h1 {
                margin: 0 0 12px;
                font-size: clamp(32px, 5vw, 48px);
                line-height: 1.05;
            }

            .subtitle {
                max-width: 720px;
                margin: 0 0 28px;
                color: var(--muted);
                font-size: 17px;
                line-height: 1.6;
            }

            .buttons {
                display: flex;
                flex-wrap: wrap;
                gap: 12px;
                margin-bottom: 12px;
            }

            .btn {
                display: inline-flex;
                align-items: center;
                justify-content: center;
                padding: 12px 18px;
                border-radius: 10px;
                text-decoration: none;
                font-weight: 700;
                transition: transform 0.15s ease, background-color 0.15s ease;
            }

            .btn:hover {
                transform: translateY(-1px);
            }

            .btn-primary {
                background: var(--primary);
                color: white;
            }

            .btn-primary:hover {
                background: var(--primary-hover);
            }

            .btn-secondary {
                background: var(--secondary);
                color: var(--text);
                border: 1px solid var(--panel-border);
            }

            .grid {
                display: grid;
                grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
                gap: 16px;
                margin-top: 20px;
            }

            .card {
                background: var(--panel);
                border: 1px solid var(--panel-border);
                padding: 18px;
                border-radius: 16px;
            }

            .card h2 {
                margin: 0 0 12px;
                font-size: 18px;
            }

            .card p,
            .card li {
                color: var(--muted);
                line-height: 1.6;
            }

            ul {
                margin: 0;
                padding-left: 18px;
            }

            .endpoint-list {
                display: grid;
                gap: 10px;
            }

            .endpoint {
                display: flex;
                align-items: center;
                gap: 10px;
                padding: 10px 12px;
                border-radius: 10px;
                background: rgba(15, 23, 42, 0.7);
                border: 1px solid rgba(34, 197, 94, 0.16);
                font-family: Consolas, monospace;
                color: #bbf7d0;
                overflow-wrap: anywhere;
            }

            .method {
                color: var(--accent);
                font-weight: 700;
            }

            .status {
                margin-top: 18px;
                display: inline-flex;
                align-items: center;
                gap: 8px;
                padding: 10px 14px;
                border-radius: 999px;
                background: rgba(34, 197, 94, 0.12);
                color: #bbf7d0;
                font-weight: 700;
            }

            .status-dot {
                width: 10px;
                height: 10px;
                border-radius: 999px;
                background: var(--accent);
                box-shadow: 0 0 16px rgba(34, 197, 94, 0.65);
            }

            .small {
                margin-top: 16px;
                color: var(--warning);
                font-size: 13px;
            }

            @media (max-width: 640px) {
                .container {
                    padding: 24px 14px 40px;
                }

                .hero {
                    padding: 20px;
                    border-radius: 16px;
                }
            }
        </style>
    </head>
    <body>
        <main class="container">
            <section class="hero">
                <span class="eyebrow">MoneyPilot API</span>
                <h1>Secure personal finance backend for a modern full-stack app.</h1>
                <p class="subtitle">
                    MoneyPilot powers authentication, expense tracking, budget workflows, recurring transactions,
                    and dashboard analytics through a clean ASP.NET Core 8 API with JWT auth, Swagger, EF Core,
                    and background processing.
                </p>

                <div class="buttons">
                    <a class="btn btn-primary" href="/swagger">View Swagger Docs</a>
                    <a class="btn btn-secondary" href="/health">Health Check</a>
                    <a class="btn btn-secondary" href="/diagnostics/startup">Startup Diagnostics</a>
                </div>

                <div class="grid">
                    <section class="card">
                        <h2>Key Features</h2>
                        <ul>
                            <li>JWT authentication with protected endpoints</li>
                            <li>Expense and budget management workflows</li>
                            <li>Recurring transaction automation</li>
                            <li>Dashboard analytics and summaries</li>
                            <li>Swagger, health checks, and diagnostics support</li>
                        </ul>
                    </section>

                    <section class="card">
                        <h2>Important Endpoints</h2>
                        <div class="endpoint-list">
                            <div class="endpoint"><span class="method">POST</span><span>/api/auth/login</span></div>
                            <div class="endpoint"><span class="method">GET</span><span>/api/expense</span></div>
                            <div class="endpoint"><span class="method">GET</span><span>/api/budget</span></div>
                            <div class="endpoint"><span class="method">GET</span><span>/api/dashboard/summary</span></div>
                        </div>
                    </section>

                    <section class="card">
                        <h2>Architecture</h2>
                        <p>Built with ASP.NET Core 8, Entity Framework Core, Identity, JWT Bearer auth, Serilog, and a layered clean architecture.</p>
                        <div class="status">
                            <span class="status-dot"></span>
                            <span>API is running successfully</span>
                        </div>
                        <p class="small">For full request and response exploration, use the Swagger UI.</p>
                    </section>
                </div>
            </section>
        </main>
    </body>
    </html>
    """;

    //==================================================================
    // Log active URLs
    // Add this after app.Build() but before app.Run()

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        var server = app.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;

        if (addresses != null)
        {
            foreach (var address in addresses)
            {
                logger.LogInformation("🚀 MoneyPilot API running on: {Address}", address);
            }
        }
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
    throw;
}

finally
{
    Log.CloseAndFlush();
}

// End of try-catch-finally
