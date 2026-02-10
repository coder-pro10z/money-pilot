using HealthChecks.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;
using MoneyPilot.Domain.Enums;
using MoneyPilot.Infrastructure.Data;
using MoneyPilot.Infrastructure.Repositories;
using MoneyPilot.Infrastructure.Services;
using Serilog;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
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

// DbContext
builder.Services.AddDbContext<MoneyPilotDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<MoneyPilotDbContext>()
    .AddDefaultTokenProviders();

// Repositories & Services
builder.Services.AddScoped<IUnitofWork, UnitOfWork>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
    // Add this with your other service registrations
builder.Services.AddScoped<IRecurringTransactionService, RecurringTransactionService>();
 


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

    builder.Services.AddScoped<AutoLoginTokenService>();

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

    //
    // ====================== APP ======================
    //


    var app = builder.Build();

    //
    // ====================== MIDDLEWARE ======================
    //
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        //app.UseSwagger();
        //app.UseSwaggerUI();
        // Later in the middleware pipeline:
        //app.UseSwagger();
        //app.UseSwaggerUI(c =>
        //{
        //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MoneyPilot API v1");
        //    c.RoutePrefix = "swagger";

        //    // Add custom CSS/JS for auto-login
        //    c.InjectStylesheet("/swagger-ui/custom.css");
        //    c.InjectJavascript("/swagger-ui/custom.js");

        //    // Pre-fill the token if available
        //    c.ConfigObject.AdditionalItems["autoLoginToken"] = "check_local_storage";

        //    // Add a custom index page with auto-login button
        //    c.IndexStream = () => GetType().Assembly
        //        .GetManifestResourceStream("MoneyPilot.API.SwaggerIndex.html");
        //});

        app.UseSwagger();
        app.UseSwaggerUI();

    }

    app.UseHttpsRedirection();

    // ⚠️ ORDER MATTERS
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapGet("/", () => "🎉 MoneyPilot API is running!");


    // Map health checks endpoint
    app.MapHealthChecks("/health");

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
        => { try {
                Log.Information("User creation test!");
                var user = new AppUser {  UserName = "tester@email.com",Email="tester@email.com"} ;
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

    // SIMPLIFIED Auto-login endpoint
    app.MapGet("/generate-token", async (TestTokenHelper helper) =>
    {
        var token = await helper.GenerateTokenForTestUserAsync("test@email.com");
    return Results.Content($"""
       {token} 
       """, "text/html");
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