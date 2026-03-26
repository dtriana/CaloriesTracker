namespace CaloriesTracker.Models.Stats
{
    public class DailySummary
    {
        private decimal dailyGoal;

        public DateOnly Date { get; set; }
        public decimal TotalCalories { get; set; }
        public decimal TotalProtein { get; set; }
        public decimal TotalFat { get; set; }
        public decimal TotalCarbs { get; set; }
        public decimal DailyGoal { get => (dailyGoal == 0) ? 1600 : dailyGoal; set => dailyGoal = value; }
        public decimal ProteinGoal { get; set; }
        public List<DailyIntake> Intakes { get; set; } = new();
    }
}
