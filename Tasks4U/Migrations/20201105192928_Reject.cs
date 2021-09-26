using Microsoft.EntityFrameworkCore.Migrations;

namespace Tasks4U.Migrations
{
    public partial class Reject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectMessage",
                table: "TodoItems",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Rejected",
                table: "TodoItems",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectMessage",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "Rejected",
                table: "TodoItems");
        }
    }
}
