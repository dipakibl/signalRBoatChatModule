using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChartPro.Migrations
{
    public partial class UpdateActiveMessagetable04062020at0209pm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AddDate",
                table: "Active_Messages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddDate",
                table: "Active_Messages");
        }
    }
}
