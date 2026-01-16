using Microsoft.EntityFrameworkCore;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Infrastructure.Data;
using MoneyPilot.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MoneyPilotDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//DI
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();

var app = builder.Build();
