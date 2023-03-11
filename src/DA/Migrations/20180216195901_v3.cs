using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DA.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDue",
                table: "Task");

            migrationBuilder.CreateTable(
                name: "UserStatistics",
                columns: table => new
                {
                    UserName = table.Column<string>(nullable: false),
                    NumberOfTasks = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatistics", x => x.UserName);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserStatistics");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDue",
                table: "Task",
                nullable: true);
        }
    }
}
