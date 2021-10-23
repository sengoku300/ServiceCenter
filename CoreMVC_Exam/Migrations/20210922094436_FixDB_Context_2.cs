using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreMVC_Exam.Migrations
{
    public partial class FixDB_Context_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PathToImage",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PathToImage",
                table: "AspNetUsers");
        }
    }
}
