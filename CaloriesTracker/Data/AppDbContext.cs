using CaloriesTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CaloriesTracker.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Food> Products { get; set; }
        public DbSet<DailyIntake> DailyIntakes { get; set; }
        public DbSet<FileRecord> FileRecords { get; set; }
        public DbSet<DailyWeight> DailyWeight { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Food>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);
            builder.Entity<Food>()
                .Property(e => e.ProteinPerPortion)
                .HasPrecision(8, 2);
            builder.Entity<Food>()
                .Property(e => e.CarbsPerPortion)
                .HasPrecision(8, 2);
            builder.Entity<Food>()
                .Property(e => e.CaloriesPerPortion)
                .HasPrecision(8, 2);
            builder.Entity<Food>()
                .Property(e => e.FatPerPortion)
                .HasPrecision(8, 2);
            builder.Entity<DailyIntake>()
                .HasOne(d => d.User)
                .WithMany(u => u.DailyIntakes)
                .HasForeignKey(d => d.UserId);
            builder.Entity<DailyIntake>()
                .HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<DailyIntake>()
                .Property(e => e.Quantity)
                .HasPrecision(8, 2);
            builder.Entity<FileRecord>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);
            builder.Entity<User>()
                .Property(e => e.DailyCalorieGoal)
                .HasPrecision(8, 2);
            builder.Entity<DailyWeight>()
                .HasKey(w => new { w.UserId, w.Date });
            builder.Entity<DailyWeight>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);
            builder.Entity<DailyWeight>()
                .Property(e => e.WeightInKg)
                .HasPrecision(8, 2);
        }
    }
}
