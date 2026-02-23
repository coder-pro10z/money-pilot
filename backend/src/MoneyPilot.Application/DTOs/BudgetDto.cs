namespace MoneyPilot.Application.DTOs
{
    public class BudgetDto
    {
        public decimal MonthlyLimit { get; set; }
        public int CategoryId { get; set; }
        public DateTime Month { get; set; }
    }
} 