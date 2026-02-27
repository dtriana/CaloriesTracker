namespace CaloriesTracker.Models
{
    public class DailyWeight
    {
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public decimal WeightInKg { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }

}
