using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyPilot.Application.DTOs;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;
using MoneyPilot.Domain.Enums;
using MoneyPilot.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyPilot.Infrastructure.Services
{
    public class RecurringTransactionService : IRecurringTransactionService
    {
        private readonly MoneyPilotDbContext _context;
        private readonly ILogger<RecurringTransactionService> _logger;

        public RecurringTransactionService(
            MoneyPilotDbContext context,
            ILogger<RecurringTransactionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RecurringTransactionDto> GetByIdAsync(int id, string userId)
        {
            var transaction = await _context.RecurringTransactions
                .Include(rt => rt.Category)
                .FirstOrDefaultAsync(rt => rt.Id == id && rt.UserId == userId);

            if (transaction == null)
                throw new KeyNotFoundException($"Recurring transaction {id} not found");

            return MapToDto(transaction);
        }

        public async Task<IEnumerable<RecurringTransactionDto>> GetAllAsync(string userId)
        {
            var transactions = await _context.RecurringTransactions
                .Include(rt => rt.Category)
                .Where(rt => rt.UserId == userId)
                .OrderByDescending(rt => rt.NextOccurrence)
                .ToListAsync();

            return transactions.Select(MapToDto);
        }

        public async Task<RecurringTransactionDto> CreateAsync(CreateRecurringTransactionDto dto, string userId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId);
            if (category == null)
                throw new ArgumentException($"Category {dto.CategoryId} not found");

            // Parse string to enum
            if (!Enum.TryParse<RecurrenceType>(dto.RecurrenceType, true, out var recurrenceType))
                throw new ArgumentException($"Invalid recurrence type: {dto.RecurrenceType}");

            // Simple next occurrence calculation
            DateTime nextOccurrence = dto.StartDate;
            if (nextOccurrence < DateTime.UtcNow)
            {
                nextOccurrence = recurrenceType switch
                {
                    RecurrenceType.Daily => DateTime.UtcNow.AddDays(dto.Interval),
                    RecurrenceType.Weekly => DateTime.UtcNow.AddDays(7 * dto.Interval),
                    RecurrenceType.Monthly => DateTime.UtcNow.AddMonths(dto.Interval),
                    RecurrenceType.Yearly => DateTime.UtcNow.AddYears(dto.Interval),
                    _ => DateTime.UtcNow.AddDays(dto.Interval)
                };
            }

            var transaction = new RecurringTransaction
            {
                UserId = userId,
                Description = dto.Description,
                Amount = dto.Amount,
                CategoryId = dto.CategoryId,
                RecurrenceType = recurrenceType,
                Interval = dto.Interval,
                DayOfWeek = null, // Simplified for now
                DayOfMonth = dto.DayOfMonth,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                NextOccurrence = nextOccurrence,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.RecurringTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return MapToDto(transaction);
        }

        public async Task<RecurringTransactionDto> UpdateAsync(int id, UpdateRecurringTransactionDto dto, string userId)
        {
            var transaction = await _context.RecurringTransactions
                .Include(rt => rt.Category)
                .FirstOrDefaultAsync(rt => rt.Id == id && rt.UserId == userId);

            if (transaction == null)
                throw new KeyNotFoundException($"Recurring transaction {id} not found");

            // Simple update - implement as needed
            if (!string.IsNullOrEmpty(dto.Description))
                transaction.Description = dto.Description;

            if (dto.Amount.HasValue)
                transaction.Amount = dto.Amount.Value;

            transaction.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToDto(transaction);
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var transaction = await _context.RecurringTransactions
                .FirstOrDefaultAsync(rt => rt.Id == id && rt.UserId == userId);

            if (transaction == null)
                throw new KeyNotFoundException($"Recurring transaction {id} not found");

            _context.RecurringTransactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<int> ProcessDueTransactionsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var dueTransactions = await _context.RecurringTransactions
                .Include(rt => rt.Category)
                .Where(rt => rt.IsActive && rt.NextOccurrence.Date <= today)
                .ToListAsync();

            var processedCount = 0;

            foreach (var transaction in dueTransactions)
            {
                // Create expense
                var expense = new Expense
                {
                    UserId = transaction.UserId,
                    Description = $"[Recurring] {transaction.Description}",
                    Amount = transaction.Amount,
                    CategoryId = transaction.CategoryId,
                    Date = transaction.NextOccurrence,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Expenses.Add(expense);

                // Update next occurrence (simple)
                transaction.NextOccurrence = transaction.RecurrenceType switch
                {
                    RecurrenceType.Daily => transaction.NextOccurrence.AddDays(transaction.Interval),
                    RecurrenceType.Weekly => transaction.NextOccurrence.AddDays(7 * transaction.Interval),
                    RecurrenceType.Monthly => transaction.NextOccurrence.AddMonths(transaction.Interval),
                    RecurrenceType.Yearly => transaction.NextOccurrence.AddYears(transaction.Interval),
                    _ => transaction.NextOccurrence.AddDays(transaction.Interval)
                };

                transaction.LastProcessed = DateTime.UtcNow;
                processedCount++;
            }

            if (processedCount > 0)
                await _context.SaveChangesAsync();

            return processedCount;
        }

        public async Task<IEnumerable<RecurringTransactionDto>> GetDueTransactionsAsync(string userId)
        {
            var today = DateTime.UtcNow.Date;
            var dueTransactions = await _context.RecurringTransactions
                .Include(rt => rt.Category)
                .Where(rt => rt.UserId == userId &&
                       rt.IsActive &&
                       rt.NextOccurrence.Date <= today)
                .OrderBy(rt => rt.NextOccurrence)
                .ToListAsync();

            return dueTransactions.Select(MapToDto);
        }

        private RecurringTransactionDto MapToDto(RecurringTransaction transaction)
        {
            return new RecurringTransactionDto
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Amount = transaction.Amount,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category?.Name ?? "Unknown",
                RecurrenceType = transaction.RecurrenceType,
                Interval = transaction.Interval,
                DayOfWeek = transaction.DayOfWeek?.ToString(),
                DayOfMonth = transaction.DayOfMonth,
                StartDate = transaction.StartDate,
                EndDate = transaction.EndDate,
                NextOccurrence = transaction.NextOccurrence,
                IsActive = transaction.IsActive,
                CreatedAt = transaction.CreatedAt,
                GeneratedExpensesCount = transaction.GeneratedExpenses?.Count ?? 0
            };
        }
    }
}