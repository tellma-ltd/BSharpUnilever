using Microsoft.EntityFrameworkCore.Migrations;

namespace BSharpUnilever.Migrations
{
    public partial class FinishingTouches : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AccountExecutiveId",
                table: "Stores",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AccountExecutiveId",
                table: "Stores",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
