using MoneyPilot.Application.DTOs.Dashboard;

namespace MoneyPilot.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(string userId);
}
