using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreMVC_Exam.Migrations
{
    public partial class ChangeDbProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PathImg",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreate",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PathImg",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
