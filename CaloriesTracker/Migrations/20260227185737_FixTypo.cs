using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaloriesTracker.Migrations
{
    /// <inheritdoc />
    public partial class FixTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WeigthInKg",
                table: "DailyWeight",
                newName: "WeightInKg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WeightInKg",
                table: "DailyWeight",
                newName: "WeigthInKg");
        }
    }
}
