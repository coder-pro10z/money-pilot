using System;
using System.ComponentModel.DataAnnotations;

namespace MoneyPilot.Application.DTOs
{
    public class UpdateRecurringTransactionDto
    {
        [StringLength(200)]
        public string? Description { get; set; }

        [Range(0.01, 1000000)]
        public decimal? Amount { get; set; }

        public int? CategoryId { get; set; }
        public string? RecurrenceType { get; set; }

        [Range(1, 365)]
        public int? Interval { get; set; }

        public string? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }
}