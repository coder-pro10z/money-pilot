namespace MoneyPilot.Application.DTOs
{
    public class ExpenseResponseDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } // Optional: include from navigation property
    }
}
