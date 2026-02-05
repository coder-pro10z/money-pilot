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
                .Include(rt => rt.GeneratedExpenses)
                .FirstOrDefaultAsync(rt => rt.Id == id && rt.UserId == userId);

            if (transaction == null)
                throw new KeyNotFoundException($"Recurring transaction {id} not found");

            return MapToDto(transaction);
        }

        public async Task<IEnumerable<RecurringTransactionDto>> GetAllAsync(string userId)
        {
            var transactions = await _context.RecurringTransactions
                .Include(rt => rt.Category)
                .Include(rt => rt.GeneratedExpenses)
                .Where(rt => rt.UserId == userId)
                .OrderByDescending(rt => rt.NextOccurrence)
                .ToListAsync();

            return transactions.Select(MapToDto);
        }

        public async Task<RecurringTransactionDto> CreateAsync(CreateRecurringTransactionDto dto, string userId)
        {
            // Validate category exists
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId);
            if (category == null)
                throw new ArgumentException($"Category {dto.CategoryId} not found");

            // Convert enum to string for storage
            string recurrenceTypeString = dto.RecurrenceType.ToString();

            // Calculate next occurrence
            DateTime nextOccurrence = CalculateFirstOccurrence(dto);

            var transaction = new RecurringTransaction
            {
                UserId = userId,
                Description = dto.Description,
                Amount = dto.Amount,
                CategoryId = dto.CategoryId,
                RecurrenceType = recurrenceTypeString,
                Interval = dto.Interval,
                DayOfWeek = dto.DayOfWeek?.ToString(),
                DayOfMonth = dto.DayOfMonth,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                NextOccurrence = nextOccurrence,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.RecurringTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created recurring transaction {TransactionId} for user {UserId}",
                transaction.Id, userId);

            return MapToDto(transaction);
        }

        public async Task<RecurringTransactionDto> UpdateAsync(int id, UpdateRecurringTransactionDto dto, string userId)
        {
            var transaction = await _context.RecurringTransactions
                .Include(rt => rt.Category)
                .Include(rt => rt.GeneratedExpenses)
                .FirstOrDefaultAsync(rt => rt.Id == id && rt.UserId == userId);

            if (transaction == null)
                throw new KeyNotFoundException($"Recurring transaction {id} not found");

            // Update fields
            if (!string.IsNullOrEmpty(dto.Description))
                transaction.Description = dto.Description;

            if (dto.Amount.HasValue)
                transaction.Amount = dto.Amount.Value;

            if (dto.CategoryId.HasValue)
            {
                var category = await _context.Categories.FindAsync(dto.CategoryId.Value);
                if (category == null)
                    throw new ArgumentException($"Category {dto.CategoryId} not found");
                transaction.CategoryId = dto.CategoryId.Value;
            }

            bool scheduleChanged = false;

            if (dto.RecurrenceType.HasValue)
            {
                transaction.RecurrenceType = dto.RecurrenceType.Value.ToString();
                scheduleChanged = true;
            }

            if (dto.Interval.HasValue)
            {
                transaction.Interval = dto.Interval.Value;
                scheduleChanged = true;
            }

            if (dto.DayOfWeek.HasValue)
            {
                transaction.DayOfWeek = dto.DayOfWeek.Value.ToString();
                scheduleChanged = true;
            }

            if (dto.DayOfMonth.HasValue)
            {
                transaction.DayOfMonth = dto.DayOfMonth.Value;
                scheduleChanged = true;
            }

            if (dto.StartDate.HasValue)
            {
                transaction.StartDate = dto.StartDate.Value;
                scheduleChanged = true;
            }

            if (dto.EndDate.HasValue)
                transaction.EndDate = dto.EndDate.Value;

            if (dto.IsActive.HasValue)
                transaction.IsActive = dto.IsActive.Value;

            // Recalculate next occurrence if schedule changed
            if (scheduleChanged)
            {
                transaction.NextOccurrence = CalculateNextOccurrence(transaction, transaction.NextOccurrence);
            }

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
                .Where(rt => rt.IsActive &&
                       rt.NextOccurrence.Date <= today &&
                       (!rt.EndDate.HasValue || rt.EndDate.Value.Date >= today))
                .ToListAsync();

            var processedCount = 0;

            foreach (var transaction in dueTransactions)
            {
                try
                {
                    // Create expense from recurring transaction
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
                    transaction.GeneratedExpenses.Add(expense);

                    // Update next occurrence
                    transaction.NextOccurrence = CalculateNextOccurrence(transaction, transaction.NextOccurrence);
                    transaction.LastProcessed = DateTime.UtcNow;

                    processedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing recurring transaction {TransactionId}", transaction.Id);
                }
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
                .Include(rt => rt.GeneratedExpenses)
                .Where(rt => rt.UserId == userId &&
                       rt.IsActive &&
                       rt.NextOccurrence.Date <= today &&
                       (!rt.EndDate.HasValue || rt.EndDate.Value.Date >= today))
                .OrderBy(rt => rt.NextOccurrence)
                .ToListAsync();

            return dueTransactions.Select(MapToDto);
        }

        // ==================== HELPER METHODS ====================

        private DateTime CalculateFirstOccurrence(CreateRecurringTransactionDto dto)
        {
            DateTime nextDate = dto.StartDate;

            // If start date is in past, keep calculating until we get a future date
            while (nextDate.Date < DateTime.UtcNow.Date)
            {
                nextDate = CalculateNextDate(
                    dto.RecurrenceType,
                    dto.Interval,
                    dto.DayOfWeek,
                    dto.DayOfMonth,
                    nextDate);
            }

            return nextDate;
        }

        private DateTime CalculateNextOccurrence(RecurringTransaction transaction, DateTime currentDate)
        {
            // Parse DayOfWeek from string if present
            DayOfWeek? dayOfWeek = null;
            if (!string.IsNullOrEmpty(transaction.DayOfWeek) &&
                Enum.TryParse<DayOfWeek>(transaction.DayOfWeek, out var parsedDay))
            {
                dayOfWeek = parsedDay;
            }

            // Parse RecurrenceType from string
            if (!Enum.TryParse<RecurrenceType>(transaction.RecurrenceType, out var recurrenceType))
            {
                recurrenceType = RecurrenceType.Monthly; // Default
            }

            return CalculateNextDate(
                recurrenceType,
                transaction.Interval,
                dayOfWeek,
                transaction.DayOfMonth,
                currentDate);
        }

        private DateTime CalculateNextDate(
            RecurrenceType recurrenceType,
            int interval,
            DayOfWeek? dayOfWeek,
            int? dayOfMonth,
            DateTime currentDate)
        {
            return recurrenceType switch
            {
                RecurrenceType.Daily => currentDate.AddDays(interval),

                RecurrenceType.Weekly =>
                    dayOfWeek.HasValue
                        ? GetNextWeekday(currentDate, dayOfWeek.Value).AddDays(7 * (interval - 1))
                        : currentDate.AddDays(7 * interval),

                RecurrenceType.Monthly =>
                    dayOfMonth.HasValue
                        ? GetNextMonthDay(currentDate, dayOfMonth.Value).AddMonths(interval - 1)
                        : currentDate.AddMonths(interval),

                RecurrenceType.Yearly => currentDate.AddYears(interval),

                _ => currentDate.AddDays(interval) // Default to daily
            };
        }

        private DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd == 0 ? 7 : daysToAdd); // If same day, go to next week
        }

        private DateTime GetNextMonthDay(DateTime start, int dayOfMonth)
        {
            try
            {
                var next = new DateTime(start.Year, start.Month, dayOfMonth);
                if (next > start) return next;

                // Move to next month
                return new DateTime(start.Year, start.Month, 1)
                    .AddMonths(1)
                    .AddDays(dayOfMonth - 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                // Invalid day for month, use last day
                return new DateTime(start.Year, start.Month, 1)
                    .AddMonths(1)
                    .AddDays(-1);
            }
        }

        private RecurringTransactionDto MapToDto(RecurringTransaction transaction)
        {
            // Try to parse the RecurrenceType string back to enum
            RecurrenceType recurrenceType = RecurrenceType.Monthly;
            if (Enum.TryParse<RecurrenceType>(transaction.RecurrenceType, out var parsedType))
            {
                recurrenceType = parsedType;
            }

            return new RecurringTransactionDto
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Amount = transaction.Amount,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category?.Name ?? "Unknown",
                RecurrenceType = recurrenceType,
                Interval = transaction.Interval,
                DayOfWeek = transaction.DayOfWeek,
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