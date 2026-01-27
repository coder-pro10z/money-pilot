using MoneyPilot.Domain.Entities;

public class Budget
{
    public int Id { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string UserId { get; set; } = null!;   // ✅ FIX
    public AppUser User { get; set; } = null!;

    public decimal MonthlyLimit { get; set; }
    public DateTime Month { get; set; }
}
