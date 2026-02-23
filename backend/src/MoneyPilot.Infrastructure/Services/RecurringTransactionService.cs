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
            //var transaction = await _context.RecurringTransactions
            //    .Include(rt => rt.Category)
            //    .FirstOrDefaultAsync(rt => rt.Id == id && rt.UserId == userId);

            //if (transaction == null)
            //    throw new KeyNotFoundException($"Recurring transaction {id} not found");

            //// Simple update - implement as needed
            //if (!string.IsNullOrEmpty(dto.Description))
            //    transaction.Description = dto.Description;

            //if (dto.Amount.HasValue)
            //    transaction.Amount = dto.Amount.Value;

            //transaction.UpdatedAt = DateTime.UtcNow;
            //await _context.SaveChangesAsync();

            //return MapToDto(transaction);

            var transaction = await _context.RecurringTransactions
       .Include(rt => rt.Category)
       .Include(rt => rt.GeneratedExpenses)
       .FirstOrDefaultAsync(rt => rt.Id == id && rt.UserId == userId);

            if (transaction == null)
                throw new KeyNotFoundException($"Recurring transaction {id} not found");

            // Track if schedule changed
            bool scheduleChanged = false;

            // Update fields if provided
            if (!string.IsNullOrEmpty(dto.Description))
                transaction.Description = dto.Description;

            if (dto.Amount.HasValue)
                transaction.Amount = dto.Amount.Value;

            if (dto.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == dto.CategoryId.Value);
                if (!categoryExists)
                    throw new ArgumentException($"Category {dto.CategoryId} not found");
                transaction.CategoryId = dto.CategoryId.Value;
            }

            //if (!string.IsNullOrEmpty(dto.RecurrenceType))
            //{
            //    transaction.RecurrenceType = dto.RecurrenceType;
            //    scheduleChanged = true;
            //}
            // ✅ FIX: Convert string to RecurrenceType enum
            if (!string.IsNullOrEmpty(dto.RecurrenceType))
            {
                if (Enum.TryParse<RecurrenceType>(dto.RecurrenceType, true, out var recurrenceType))
                {
                    transaction.RecurrenceType = recurrenceType;
                    scheduleChanged = true;
                }
                else
                {
                    throw new ArgumentException($"Invalid recurrence type: {dto.RecurrenceType}");
                }
            }

            if (dto.Interval.HasValue)
            {
                transaction.Interval = dto.Interval.Value;
                scheduleChanged = true;
            }

            //if (dto.DayOfWeek != null)
            //{
            //    transaction.DayOfWeek = dto.DayOfWeek; // Could be empty string to clear
            //    scheduleChanged = true;
            //}
            // ✅ FIX: Convert string to DayOfWeek enum
            if (dto.DayOfWeek != null)
            {
                if (string.IsNullOrEmpty(dto.DayOfWeek))
                {
                    // Clear the value
                    transaction.DayOfWeek = null;
                }
                else if (Enum.TryParse<DayOfWeek>(dto.DayOfWeek, true, out var dayOfWeek))
                {
                    transaction.DayOfWeek = dayOfWeek;
                }
                else
                {
                    throw new ArgumentException($"Invalid day of week: {dto.DayOfWeek}");
                }
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
                transaction.NextOccurrence = CalculateNextOccurrence(
                    transaction.RecurrenceType,
                    transaction.Interval,
                    transaction.DayOfWeek,
                    transaction.DayOfMonth,
                    transaction.StartDate);
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

        //public async Task<int> ProcessDueTransactionsAsync()
        //{
        //    var today = DateTime.UtcNow.Date;
        //    var dueTransactions = await _context.RecurringTransactions
        //        .Include(rt => rt.Category)
        //        .Where(rt => rt.IsActive && rt.NextOccurrence.Date <= today)
        //        .ToListAsync();

        //    var processedCount = 0;

        //    foreach (var transaction in dueTransactions)
        //    {
        //        // Create expense
        //        var expense = new Expense
        //        {
        //            UserId = transaction.UserId,
        //            Description = $"[Recurring] {transaction.Description}",
        //            Amount = transaction.Amount,
        //            CategoryId = transaction.CategoryId,
        //            Date = transaction.NextOccurrence,
        //            CreatedAt = DateTime.UtcNow
        //        };

        //        _context.Expenses.Add(expense);

        //        // Update next occurrence (simple)
        //        transaction.NextOccurrence = transaction.RecurrenceType switch
        //        {
        //            RecurrenceType.Daily => transaction.NextOccurrence.AddDays(transaction.Interval),
        //            RecurrenceType.Weekly => transaction.NextOccurrence.AddDays(7 * transaction.Interval),
        //            RecurrenceType.Monthly => transaction.NextOccurrence.AddMonths(transaction.Interval),
        //            RecurrenceType.Yearly => transaction.NextOccurrence.AddYears(transaction.Interval),
        //            _ => transaction.NextOccurrence.AddDays(transaction.Interval)
        //        };

        //        transaction.LastProcessed = DateTime.UtcNow;
        //        processedCount++;
        //    }

        //    if (processedCount > 0)
        //        await _context.SaveChangesAsync();

        //    return processedCount;
        //}

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
        private DateTime CalculateNextOccurrence(
         RecurrenceType recurrenceType,  // Changed from string to enum
         int interval,
         DayOfWeek? dayOfWeek,          // Changed from string? to DayOfWeek?
         int? dayOfMonth,
         DateTime fromDate)
        {
            switch (recurrenceType)
            {
                case RecurrenceType.Daily:
                    return fromDate.AddDays(interval);

                case RecurrenceType.Weekly:
                    if (dayOfWeek.HasValue)
                    {
                        // Find next occurrence of this day
                        var next = fromDate;
                        while (next.DayOfWeek != dayOfWeek.Value)
                        {
                            next = next.AddDays(1);
                        }
                        return next.AddDays(7 * (interval - 1));
                    }
                    return fromDate.AddDays(7 * interval);

                case RecurrenceType.Monthly:
                    if (dayOfMonth.HasValue)
                    {
                        try
                        {
                            var next = new DateTime(fromDate.Year, fromDate.Month, dayOfMonth.Value);
                            if (next <= fromDate)
                            {
                                next = next.AddMonths(1);
                            }
                            return next.AddMonths(interval - 1);
                        }
                        catch
                        {
                            // Invalid day for month, use last day
                            var next = new DateTime(fromDate.Year, fromDate.Month, 1)
                                .AddMonths(1)
                                .AddDays(-1);
                            if (next <= fromDate)
                            {
                                next = next.AddMonths(1);
                            }
                            return next.AddMonths(interval - 1);
                        }
                    }
                    return fromDate.AddMonths(interval);

                case RecurrenceType.Yearly:
                    return fromDate.AddYears(interval);

                default:
                    return fromDate.AddMonths(interval);
            }
        }


        // Enhanced version with detailed result
        public async Task<RecurringTransactionProcessingResultDto> ProcessDueTransactionsWithResultAsync()
        {
            var result = new RecurringTransactionProcessingResultDto();
            var errors = new List<string>();

            var today = DateTime.UtcNow.Date;

            // ✅ Using Serilog correctly - injected via constructor
            _logger.LogInformation("Processing recurring transactions for {Date}", today);

            try
            {
                var dueTransactions = await _context.RecurringTransactions
                    .Include(rt => rt.Category)
                    .Where(rt => rt.IsActive && rt.NextOccurrence.Date <= today)
                    .ToListAsync();

                result.TotalProcessed = dueTransactions.Count;
                _logger.LogInformation("Found {Count} transactions due for processing", result.TotalProcessed);

                foreach (var transaction in dueTransactions)
                {
                    try
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

                        // Update next occurrence
                        transaction.NextOccurrence = transaction.RecurrenceType switch
                        {
                            RecurrenceType.Daily => transaction.NextOccurrence.AddDays(transaction.Interval),
                            RecurrenceType.Weekly => transaction.NextOccurrence.AddDays(7 * transaction.Interval),
                            RecurrenceType.Monthly => transaction.NextOccurrence.AddMonths(transaction.Interval),
                            RecurrenceType.Yearly => transaction.NextOccurrence.AddYears(transaction.Interval),
                            _ => transaction.NextOccurrence.AddDays(transaction.Interval)
                        };

                        transaction.LastProcessed = DateTime.UtcNow;
                        result.SuccessfulCreations++;

                        _logger.LogDebug("Processed transaction {Id} for user {UserId}",
                            transaction.Id, transaction.UserId);
                    }
                    catch (Exception ex)
                    {
                        result.FailedCreations++;
                        var errorMsg = $"Transaction {transaction.Id}: {ex.Message}";
                        errors.Add(errorMsg);

                        // ✅ Using Serilog with structured logging
                        _logger.LogError(ex, "Failed to process recurring transaction {TransactionId}",
                            transaction.Id);
                    }
                }

                if (result.TotalProcessed > 0)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Saved {Count} changes to database", result.TotalProcessed);
                }

                result.Errors = errors;

                // ✅ Final log using Serilog
                _logger.LogInformation(
                    "Completed processing: {Successful}/{Total} successful, {Failed} failed",
                    result.SuccessfulCreations, result.TotalProcessed, result.FailedCreations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process recurring transactions");
                errors.Add($"Service error: {ex.Message}");
                result.Errors = errors;
            }

            return result;
        }

        // Keep your existing method for backward compatibility
        public async Task<int> ProcessDueTransactionsAsync()
        {
            var result = await ProcessDueTransactionsWithResultAsync();
            return result.TotalProcessed;
        }



    }
}