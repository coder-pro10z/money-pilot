using System.Collections.Generic;

namespace MoneyPilot.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public decimal TotalBudget { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal RemainingBalance { get; set; }
    public List<CategoryBreakdownDto> CategoryBreakdown { get; set; }
    public List<MonthlyTrendDto> MonthlyTrend { get; set; }
}
