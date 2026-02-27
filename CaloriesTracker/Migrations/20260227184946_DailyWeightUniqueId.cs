using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaloriesTracker.Migrations
{
    /// <inheritdoc />
    public partial class DailyWeightUniqueId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyWeight",
                table: "DailyWeight");

            migrationBuilder.DropIndex(
                name: "IX_DailyWeight_UserId",
                table: "DailyWeight");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DailyWeight");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyWeight",
                table: "DailyWeight",
                columns: new[] { "UserId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyWeight",
                table: "DailyWeight");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DailyWeight",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyWeight",
                table: "DailyWeight",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DailyWeight_UserId",
                table: "DailyWeight",
                column: "UserId");
        }
    }
}
