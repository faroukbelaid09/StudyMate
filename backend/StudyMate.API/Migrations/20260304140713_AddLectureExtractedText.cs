using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyMate.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLectureExtractedText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtractedText",
                table: "Lectures",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtractedText",
                table: "Lectures");
        }
    }
}
