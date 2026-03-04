using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyMate.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLectureStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Lectures",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Lectures");
        }
    }
}
