using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreMVC_Exam.Migrations
{
    public partial class AddPhotoInProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PathImg",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PathImg",
                table: "Product");
        }
    }
}
