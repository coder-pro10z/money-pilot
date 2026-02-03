using System;
using System.Collections.Generic;

namespace MoneyPilot.Domain.Entities
{
    public class RecurringTransaction
    {
        public int Id { get; set; }

        // Basic transaction info
        public string UserId { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }

        // Recurrence settings (simplified)
        public string RecurrenceType { get; set; } = null!; // "Daily", "Weekly", "Monthly"
        public int Interval { get; set; } = 1; // Every X days/weeks/months

        // Optional specific day
        public string? DayOfWeek { get; set; } // "Monday", "Tuesday", etc
        public int? DayOfMonth { get; set; }

        // Schedule dates
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime NextOccurrence { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Category Category { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public ICollection<Expense> GeneratedExpenses { get; set; } = new List<Expense>();

        // Timestamps (optional but good for audit)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}