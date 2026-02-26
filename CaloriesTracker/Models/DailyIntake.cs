namespace CaloriesTracker.Models
{
    public class DailyIntake
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int ProductId { get; set; }
        public Food Product { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }

}
