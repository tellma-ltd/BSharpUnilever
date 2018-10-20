using Microsoft.EntityFrameworkCore.Migrations;

namespace BSharpUnilever.Migrations
{
    public partial class UniqueIndexOnSerialNo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SupportRequests_SerialNumber",
                table: "SupportRequests",
                column: "SerialNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupportRequests_SerialNumber",
                table: "SupportRequests");
        }
    }
}
