namespace CaloriesTracker.Models
{
    public class Food
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal CaloriesPerPortion { get; set; }
        public decimal ProteinPerPortion { get; set; }
        public decimal FatPerPortion { get; set; }
        public decimal CarbsPerPortion { get; set; }
        public required string PortionName { get; set; }
        public decimal PortionSize { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}
