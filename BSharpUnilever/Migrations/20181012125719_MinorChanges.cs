using Microsoft.EntityFrameworkCore.Migrations;

namespace BSharpUnilever.Migrations
{
    public partial class MinorChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "SupportRequests",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "AccountExecutiveId",
                table: "SupportRequests",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "SupportRequests",
                maxLength: 1023,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserRole",
                table: "StateChanges",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "SupportRequests");

            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "StateChanges");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "SupportRequests",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountExecutiveId",
                table: "SupportRequests",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
