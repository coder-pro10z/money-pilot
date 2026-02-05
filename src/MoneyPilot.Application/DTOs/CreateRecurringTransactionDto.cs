using System;
using System.ComponentModel.DataAnnotations;

namespace MoneyPilot.Application.DTOs
{
    public class CreateRecurringTransactionDto
    {
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(0.01, 1000000)]
        public decimal Amount { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string RecurrenceType { get; set; } = null!; // "Daily", "Weekly", "Monthly", "Yearly"

        [Range(1, 365)]
        public int Interval { get; set; } = 1;

        public string? DayOfWeek { get; set; } // "Monday", "Tuesday", etc
        public int? DayOfMonth { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}