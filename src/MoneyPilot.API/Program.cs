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
    // ====================== SWAGGER ======================
    //
    builder.Services.AddEndpointsApiExplorer();

    //builder.Services.AddSwaggerGen(c =>
    //{
    //    c.SwaggerDoc("v1", new OpenApiInfo
    //    {
    //        Title = "MoneyPilot API",
    //        Version = "v1"
    //    });

    //    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //    {
    //        Name = "Authorization",
    //        Type = SecuritySchemeType.Http,
    //        Scheme = "Bearer",
    //        BearerFormat = "JWT",
    //        In = ParameterLocation.Header,
    //        Description = "Enter JWT token. Do NOT include 'Bearer ' prefix."
    //    });

    //    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        },
    //        Array.Empty<string>()
    //    }
    //});
    //});


    // Add Swagger with auto-login support
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "MoneyPilot API",
            Version = "v1",
            Description = $"<strong>Auto-Login Available:</strong> Visit <a href='/auto-login' target='_blank'>/auto-login</a> for test token"
        });

        // Add JWT Authentication to Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter: 'Bearer {token}'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
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
            new string[] {}
        }
    });

        // Add auto-login endpoint to Swagger
        c.TagActionsBy(api => new[] { api.GroupName });
        c.DocInclusionPredicate((name, api) => true);
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
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "MoneyPilot API v1");
            c.RoutePrefix = "swagger";

            // Optional: Add a link to auto-login in Swagger
            c.HeadContent = @"
        <script>
            window.addEventListener('load', function() {
                // Add auto-login button to Swagger UI
                const nav = document.querySelector('.topbar-wrapper');
                if (nav) {
                    const autoLoginBtn = document.createElement('a');
                    autoLoginBtn.href = '/auto-login';
                    autoLoginBtn.target = '_blank';
                    autoLoginBtn.style.cssText = 'margin-left: 20px; padding: 8px 15px; background: #28a745; color: white; border-radius: 4px; text-decoration: none;';
                    autoLoginBtn.textContent = '🔓 Auto-Login';
                    nav.appendChild(autoLoginBtn);
                }
            });
        </script>
    ";
        });
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

        //var recurringTransaction = new RecurringTransaction
        //{
        //    UserId = user.Id,
        //    Description = "Monthly Netflix Subscription",
        //    Amount = 15.99m,
        //    CategoryId = category.Id,
        //    RecurrenceType = "Monthly",
        //    Interval = 1,
        //    DayOfMonth = 15,
        //    StartDate = DateTime.UtcNow.AddDays(-30),
        //    NextOccurrence = DateTime.UtcNow.AddDays(5), // Due in 5 days
        //    IsActive = true,
        //    CreatedAt = DateTime.UtcNow
        //};

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

    // Auto-login convenience endpoint
    //    app.MapGet("/auto-login", async (TestTokenHelper helper) =>
    //    {
    //        var token = await helper.GenerateTokenForTestUserAsync("test@email.com");

    //        // Fixed raw string literal - no unescaped triple quotes in the content
    //        var html = $$"""
    //    <!DOCTYPE html>
    //    <html>
    //    <head>
    //        <title>MoneyPilot Auto Login</title>
    //        <script>
    //            function authorizeSwagger() {
    //                const swaggerUrl = '/swagger/index.html';
    //                const swaggerWindow = window.open(swaggerUrl, '_blank');

    //                setTimeout(() => {
    //                    try {
    //                        localStorage.setItem('moneyPilotAutoToken', '{{token}}');
    //                        sessionStorage.setItem('swaggerAutoAuth', 'true');

    //                        swaggerWindow.postMessage({
    //                            type: 'SET_SWAGGER_TOKEN',
    //                            token: '{{token}}'
    //                        }, window.location.origin);

    //                        alert('Swagger opened! Click "Authorize" button and paste the token if not auto-filled.');
    //                    } catch (e) {
    //                        console.error('Could not auto-authorize:', e);
    //                        alert('Swagger opened. Please manually paste the token in the Authorize dialog.');
    //                    }
    //                }, 1000);
    //            }

    //            function copyToken() {
    //                navigator.clipboard.writeText('{{token}}');
    //                alert('Token copied to clipboard!');
    //            }

    //            function autoFillSwagger() {
    //                const token = '{{token}}';
    //                const authorizeModal = document.querySelector('.dialog-ux .modal-ux');
    //                if (authorizeModal) {
    //                    const input = authorizeModal.querySelector('input[type="text"]');
    //                    if (input) {
    //                        input.value = token;
    //                        input.dispatchEvent(new Event('input', { bubbles: true }));
    //                        alert('Token filled in Swagger authorize modal!');
    //                    }
    //                } else {
    //                    alert('Open Swagger and click "Authorize" first, then click this button again.');
    //                }
    //            }
    //        </script>
    //        <style>
    //            body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 800px; margin: 40px auto; padding: 20px; }
    //            h2 { color: #2c3e50; border-bottom: 2px solid #3498db; padding-bottom: 10px; }
    //            .token-box { background: #f8f9fa; border: 2px dashed #6c757d; padding: 15px; margin: 20px 0; word-break: break-all; font-family: monospace; font-size: 14px; }
    //            .button-group { margin: 25px 0; }
    //            button { background: #3498db; color: white; border: none; padding: 12px 20px; margin-right: 10px; border-radius: 5px; cursor: pointer; font-size: 16px; }
    //            button:hover { background: #2980b9; }
    //            .secondary { background: #6c757d; }
    //            .secondary:hover { background: #5a6268; }
    //            .success { background: #28a745; }
    //            .success:hover { background: #218838; }
    //            pre { background: #2c3e50; color: #ecf0f1; padding: 15px; border-radius: 5px; overflow-x: auto; }
    //            .usage { background: #e8f4fc; padding: 15px; border-radius: 5px; margin: 20px 0; }
    //        </style>
    //    </head>
    //    <body>
    //        <h2>💰 MoneyPilot Test User Auto-Login</h2>

    //        <div class="usage">
    //            <p><strong>Test User:</strong> test@email.com</p>
    //            <p><strong>Password:</strong> Test@123</p>
    //            <p><strong>Token Expires:</strong> 60 minutes from now</p>
    //        </div>

    //        <h3>Your Bearer Token:</h3>
    //        <div class="token-box">
    //            {{token}}
    //        </div>

    //        <div class="button-group">
    //            <button onclick="copyToken()">📋 Copy Token to Clipboard</button>
    //            <button onclick="authorizeSwagger()" class="success">🔓 Auto-Open & Authorize Swagger</button>
    //            <button onclick="window.open('/swagger', '_blank')" class="secondary">📄 Open Swagger</button>
    //            <button onclick="window.open('/test-token', '_blank')">🔄 Get JSON Token</button>
    //        </div>

    //        <h3>Usage Instructions:</h3>
    //        <ol>
    //            <li>Click "Copy Token" above</li>
    //            <li>Open Swagger (button above or go to <a href="/swagger" target="_blank">/swagger</a>)</li>
    //            <li>Click the <strong>Authorize</strong> button (top right)</li>
    //            <li>Paste token in the value field: <code>Bearer {{token}}</code></li>
    //            <li>Click "Authorize" then "Close"</li>
    //            <li>All API calls will now use this token</li>
    //        </ol>

    //        <h3>Quick API Test:</h3>
    //        <button onclick="window.open('/api/expenses', '_blank')">📊 Test Expenses API</button>
    //        <button onclick="window.open('/api/recurring-transactions', '_blank')">🔄 Test Recurring Transactions</button>

    //        <h3>CURL Example:</h3>
    //        <pre>curl -H "Authorization: Bearer {{token}}" \
    //     -H "Content-Type: application/json" \
    //     https://localhost:7030/api/expenses</pre>

    //        <h3>PowerShell Example:</h3>
    //        <pre>$token = "{{token}}"
    //$headers = @{
    //    "Authorization" = "Bearer $token"
    //    "Content-Type" = "application/json"
    //}
    //Invoke-RestMethod -Uri "https://localhost:7030/api/expenses" `
    //    -Headers $headers `
    //    -SkipCertificateCheck</pre>

    //        <h3>JavaScript/Fetch Example:</h3>
    //        <pre>fetch('/api/expenses', {
    //    headers: {
    //        'Authorization': 'Bearer {{token}}'
    //    }
    //})
    //.then(response => response.json())
    //.then(data => console.log(data));</pre>

    //        <script>
    //            console.log('💰 MoneyPilot Auto-Login Token:');
    //            console.log('Bearer {{token}}');
    //            console.log('Use: fetch("/api/expenses", { headers: { "Authorization": "Bearer {{token}}" } })');
    //        </script>
    //    </body>
    //    </html>
    //    """;

    //        return Results.Content(html, "text/html");
    //    });

    // Auto-login convenience endpoint
    app.MapGet("/auto-login", async (TestTokenHelper helper) =>
    {
        var token = await helper.GenerateTokenForTestUserAsync("test@email.com");

        // Return HTML page with auto-filled Swagger authorization
        var html = $$"""
        <!DOCTYPE html>
        <html>
        <head>
            <title>MoneyPilot Auto Login</title>
            <script>
                function authorizeSwagger() {
                    const swaggerUrl = '/swagger/index.html';
                    const swaggerWindow = window.open(swaggerUrl, '_blank');
                    
                    setTimeout(() => {
                        try {
                            localStorage.setItem('moneyPilotAutoToken', '{{token}}');
                            sessionStorage.setItem('swaggerAutoAuth', 'true');
                            
                            swaggerWindow.postMessage({
                                type: 'SET_SWAGGER_TOKEN',
                                token: '{{token}}'
                            }, window.location.origin);
                            
                            alert('Swagger opened! Click "Authorize" button and paste the token if not auto-filled.');
                        } catch (e) {
                            console.error('Could not auto-authorize:', e);
                            alert('Swagger opened. Please manually paste the token in the Authorize dialog.');
                        }
                    }, 1000);
                }
                
                function copyToken() {
                    navigator.clipboard.writeText('{{token}}');
                    alert('Token copied to clipboard!');
                }
            </script>
            <style>
                body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 800px; margin: 40px auto; padding: 20px; }
                h2 { color: #2c3e50; border-bottom: 2px solid #3498db; padding-bottom: 10px; }
                .token-box { background: #f8f9fa; border: 2px dashed #6c757d; padding: 15px; margin: 20px 0; word-break: break-all; font-family: monospace; font-size: 14px; }
                .button-group { margin: 25px 0; }
                button { background: #3498db; color: white; border: none; padding: 12px 20px; margin-right: 10px; border-radius: 5px; cursor: pointer; font-size: 16px; }
                button:hover { background: #2980b9; }
                .secondary { background: #6c757d; }
                .secondary:hover { background: #5a6268; }
                .success { background: #28a745; }
                .success:hover { background: #218838; }
                pre { background: #2c3e50; color: #ecf0f1; padding: 15px; border-radius: 5px; overflow-x: auto; }
                .usage { background: #e8f4fc; padding: 15px; border-radius: 5px; margin: 20px 0; }
            </style>
        </head>
        <body>
            <h2>💰 MoneyPilot Test User Auto-Login</h2>
            
            <div class="usage">
                <p><strong>Test User:</strong> test@email.com</p>
                <p><strong>Password:</strong> Test@123</p>
                <p><strong>Token Expires:</strong> 60 minutes from now</p>
            </div>
            
            <h3>Your Bearer Token:</h3>
            <div class="token-box">
                {{token}}
            </div>
            
            <div class="button-group">
                <button onclick="copyToken()">📋 Copy Token to Clipboard</button>
                <button onclick="authorizeSwagger()" class="success">🔓 Auto-Open & Authorize Swagger</button>
                <button onclick="window.open('/swagger', '_blank')" class="secondary">📄 Open Swagger</button>
                <button onclick="window.open('/test-token', '_blank')">🔄 Get JSON Token</button>
            </div>
            
            <h3>Usage Instructions:</h3>
            <ol>
                <li>Click "Copy Token" above</li>
                <li>Open Swagger (button above or go to <a href="/swagger" target="_blank">/swagger</a>)</li>
                <li>Click the <strong>Authorize</strong> button (top right)</li>
                <li>Paste token in the value field: <code>Bearer {{token}}</code></li>
                <li>Click "Authorize" then "Close"</li>
                <li>All API calls will now use this token</li>
            </ol>
            
            <h3>Quick API Test:</h3>
            <button onclick="window.open('/api/expenses', '_blank')">📊 Test Expenses API</button>
            <button onclick="window.open('/api/recurring-transactions', '_blank')">🔄 Test Recurring Transactions</button>
            
            <h3>CURL Example:</h3>
            <pre>curl -H "Authorization: Bearer {{token}}" \
         -H "Content-Type: application/json" \
         https://localhost:44391/api/expenses</pre>
            
            <h3>PowerShell Example:</h3>
            <pre>$token = "{{token}}"
            $headers = @{
                "Authorization" = "Bearer $token"
                "Content-Type" = "application/json"
            }
            Invoke-RestMethod -Uri "https://localhost:44391/api/expenses" `
                -Headers $headers `
                -SkipCertificateCheck</pre>
            
                        <h3>JavaScript/Fetch Example:</h3>
                        <pre>fetch('/api/expenses', {
                headers: {
                    'Authorization': 'Bearer {{token}}'
                }
            })
            .then(response => response.json())
            .then(data => console.log(data));</pre>
            
                        <script>
                            console.log('💰 MoneyPilot Auto-Login Token:');
                            console.log('Bearer {{token}}');
                            console.log('Use: fetch("/api/expenses", { headers: { "Authorization": "Bearer {{token}}" } })');
                        </script>
                    </body>
        </html>
        """;

        return Results.Content(html, "text/html");
    });
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
    throw;
}
//catch (HostAbortedException)
//{
//    // This is normal when running EF Core migrations
//    Log.Information("Host was aborted (normal for EF migrations)");
//}

finally
{
    Log.CloseAndFlush();
}

// End of try-catch-finally