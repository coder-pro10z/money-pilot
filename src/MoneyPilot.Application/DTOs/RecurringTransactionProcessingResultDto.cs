using System;
using System.Collections.Generic;

namespace MoneyPilot.Application.DTOs
{
    public class RecurringTransactionProcessingResultDto
    {
        public int TotalProcessed { get; set; }
        public int SuccessfulCreations { get; set; }
        public int FailedCreations { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public List<string>? Errors { get; set; }

        // Helper properties
        public bool HasErrors => Errors?.Count > 0;
        public string Status => SuccessfulCreations == TotalProcessed ? "Complete" : "Partial";
    }
}