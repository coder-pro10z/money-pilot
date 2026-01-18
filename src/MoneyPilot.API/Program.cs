using Microsoft.EntityFrameworkCore;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Infrastructure.Data;
using MoneyPilot.Infrastructure.Repositories;
using MoneyPilot.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// SERVICES
builder.Services.AddControllers();
builder.Services.AddDbContext<MoneyPilotDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitofWork, UnitOfWork>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();

// SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// MIDDLEWARE
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// TEST endpoint
app.MapGet("/", () => "🎉 MoneyPilot API is running!");
app.MapGet("/test/expenses", async (IExpenseService expenseService) =>
{
    var expenses = await expenseService.GetAllAsync("test-user");
    return Results.Ok(expenses);
});

app.Run();
