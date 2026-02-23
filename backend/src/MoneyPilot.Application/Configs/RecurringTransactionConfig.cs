namespace MoneyPilot.Application.Configs
{
    public class RecurringTransactionConfig
    {
        public bool Enabled { get; set; } = true;
        public string ProcessingTime { get; set; } = "02:00:00";
        public int RetryCount { get; set; } = 3;
        public int RetryDelaySeconds { get; set; } = 60;
        public bool RunOnStartup { get; set; } = true;

        // Helper method
        public TimeSpan GetProcessingTimeSpan()
        {
            return TimeSpan.TryParse(ProcessingTime, out var time)
                ? time
                : TimeSpan.Parse("02:00:00");
        }
    }
}