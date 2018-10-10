using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BSharpUnilever.Migrations
{
    public partial class ModifiedRolesSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportResultLineItems_Products_ProductId",
                table: "SupportResultLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportResultLineItems_SupportRequests_SupportRequestId",
                table: "SupportResultLineItems");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupportResultLineItems",
                table: "SupportResultLineItems");

            migrationBuilder.RenameTable(
                name: "SupportResultLineItems",
                newName: "SupportRequestLineItems");

            migrationBuilder.RenameIndex(
                name: "IX_SupportResultLineItems_SupportRequestId",
                table: "SupportRequestLineItems",
                newName: "IX_SupportRequestLineItems_SupportRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_SupportResultLineItems_ProductId",
                table: "SupportRequestLineItems",
                newName: "IX_SupportRequestLineItems_ProductId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Stores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupportRequestLineItems",
                table: "SupportRequestLineItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportRequestLineItems_Products_ProductId",
                table: "SupportRequestLineItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportRequestLineItems_SupportRequests_SupportRequestId",
                table: "SupportRequestLineItems",
                column: "SupportRequestId",
                principalTable: "SupportRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportRequestLineItems_Products_ProductId",
                table: "SupportRequestLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportRequestLineItems_SupportRequests_SupportRequestId",
                table: "SupportRequestLineItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupportRequestLineItems",
                table: "SupportRequestLineItems");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "SupportRequestLineItems",
                newName: "SupportResultLineItems");

            migrationBuilder.RenameIndex(
                name: "IX_SupportRequestLineItems_SupportRequestId",
                table: "SupportResultLineItems",
                newName: "IX_SupportResultLineItems_SupportRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_SupportRequestLineItems_ProductId",
                table: "SupportResultLineItems",
                newName: "IX_SupportResultLineItems_ProductId");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupportResultLineItems",
                table: "SupportResultLineItems",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportResultLineItems_Products_ProductId",
                table: "SupportResultLineItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportResultLineItems_SupportRequests_SupportRequestId",
                table: "SupportResultLineItems",
                column: "SupportRequestId",
                principalTable: "SupportRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
