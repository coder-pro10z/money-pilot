using MoneyPilot.Domain.Enums;
using System;

namespace MoneyPilot.Application.DTOs
{
    public class RecurringTransactionDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public RecurrenceType RecurrenceType { get; set; } // Enum in DTO for type safety
        public int Interval { get; set; }
        public string? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime NextOccurrence { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int GeneratedExpensesCount { get; set; }
    }
}