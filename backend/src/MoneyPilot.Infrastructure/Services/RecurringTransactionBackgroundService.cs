using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoneyPilot.Application.Configs;
using MoneyPilot.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MoneyPilot.Infrastructure.Services
{
    public class RecurringTransactionBackgroundService : BackgroundService
    {
        private readonly ILogger<RecurringTransactionBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RecurringTransactionConfig _config;
        private Timer? _timer;

        public RecurringTransactionBackgroundService(
            ILogger<RecurringTransactionBackgroundService> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<RecurringTransactionConfig> config)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _config = config.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_config.Enabled)
            {
                _logger.LogInformation("Recurring transaction background service is disabled");
                return;
            }

            _logger.LogInformation("💰 Recurring transaction background service started");

            // Run immediately on startup for testing
            if (_config.RunOnStartup)
            {
                _logger.LogInformation("🚀 Running initial processing on startup...");
                await ProcessDueTransactionsAsync();
            }

            // Schedule daily processing
            ScheduleDailyProcessing();

            // Keep service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private void ScheduleDailyProcessing()
        {
            try
            {
                var processingTime = _config.GetProcessingTimeSpan();
                var now = DateTime.Now;
                var scheduledTime = now.Date.Add(processingTime);

                // If the scheduled time has already passed today, schedule for tomorrow
                if (now > scheduledTime)
                    scheduledTime = scheduledTime.AddDays(1);

                var initialDelay = scheduledTime - now;

                _logger.LogInformation("⏰ Next processing scheduled for: {ScheduledTime} (in {InitialDelay})",
                    scheduledTime, initialDelay);

                _timer = new Timer(async _ => await ProcessDueTransactionsAsync(),
                    null,
                    initialDelay,
                    TimeSpan.FromDays(1)); // Run daily
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to schedule background processing");
            }
        }

        private async Task ProcessDueTransactionsAsync()
        {
            // ✅ Using Serilog correctly
            _logger.LogInformation("🔄 Starting recurring transaction processing at {Time}", DateTime.Now);

            for (int attempt = 1; attempt <= _config.RetryCount; attempt++)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IRecurringTransactionService>();

                    // Use the detailed result method
                    var result = await service.ProcessDueTransactionsWithResultAsync();

                    if (result.TotalProcessed > 0)
                    {
                        _logger.LogInformation("✅ Processed {Total} transactions: {Successful} created, {Failed} failed",
                            result.TotalProcessed, result.SuccessfulCreations, result.FailedCreations);
                    }
                    else
                    {
                        _logger.LogInformation("📭 No transactions due for processing");
                    }

                    return; // Success
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "⚠️ Attempt {Attempt}/{MaxAttempts} failed",
                        attempt, _config.RetryCount);

                    if (attempt == _config.RetryCount)
                    {
                        _logger.LogCritical("❌ All {MaxAttempts} attempts failed", _config.RetryCount);
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_config.RetryDelaySeconds));
                    }
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            await base.StopAsync(stoppingToken);
            _logger.LogInformation("🛑 Recurring transaction background service stopped");
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}