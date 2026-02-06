using System;
using System.Collections.Generic;
using MoneyPilot.Domain.Enums; // Add this using

namespace MoneyPilot.Domain.Entities
{
    public class RecurringTransaction
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }

        // Change from string to enum
        public RecurrenceType RecurrenceType { get; set; } // Enum, not string
        public int Interval { get; set; } = 1;

        // Change from string? to DayOfWeek?
        public DayOfWeek? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime NextOccurrence { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastProcessed { get; set; }

        public Category Category { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public ICollection<Expense> GeneratedExpenses { get; set; } = new List<Expense>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}