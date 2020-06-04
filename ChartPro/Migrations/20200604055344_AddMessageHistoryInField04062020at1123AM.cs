using Microsoft.EntityFrameworkCore.Migrations;

namespace ChartPro.Migrations
{
    public partial class AddMessageHistoryInField04062020at1123AM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "MessageHistories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MessageHistories");
        }
    }
}
