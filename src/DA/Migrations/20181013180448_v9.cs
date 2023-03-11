using Microsoft.EntityFrameworkCore.Migrations;

namespace DA.Migrations
{
    public partial class v9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DueInDays",
                table: "Task",
                nullable: true,
                computedColumnSql: "DATEDIFF(day, GETDATE(), [Due])",
                oldClrType: typeof(int),
                oldComputedColumnSql: "DATEDIFF(day, GETDATE(), [Due])");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DueInDays",
                table: "Task",
                nullable: false,
                computedColumnSql: "DATEDIFF(day, GETDATE(), [Due])",
                oldClrType: typeof(int),
                oldNullable: true,
                oldComputedColumnSql: "DATEDIFF(day, GETDATE(), [Due])");
        }
    }
}
