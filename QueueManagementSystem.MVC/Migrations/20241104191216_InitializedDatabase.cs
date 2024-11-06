using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QueueManagementSystem.MVC.Migrations
{
    /// <inheritdoc />
    public partial class InitializedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderAssignments_ServiceProvider_ServiceProviderId",
                table: "ServiceProviderAssignments");

            migrationBuilder.DropTable(
                name: "ServiceProvider");

            migrationBuilder.RenameColumn(
                name: "ServiceProviderId",
                table: "ServiceProviderAssignments",
                newName: "SystemUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceProviderAssignments_ServiceProviderId",
                table: "ServiceProviderAssignments",
                newName: "IX_ServiceProviderAssignments_SystemUserId");

            migrationBuilder.RenameColumn(
                name: "LockedByServiceProviderId",
                table: "QueuedTicket",
                newName: "LockedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderAssignments_SystemUsers_SystemUserId",
                table: "ServiceProviderAssignments",
                column: "SystemUserId",
                principalTable: "SystemUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderAssignments_SystemUsers_SystemUserId",
                table: "ServiceProviderAssignments");

            migrationBuilder.RenameColumn(
                name: "SystemUserId",
                table: "ServiceProviderAssignments",
                newName: "ServiceProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceProviderAssignments_SystemUserId",
                table: "ServiceProviderAssignments",
                newName: "IX_ServiceProviderAssignments_ServiceProviderId");

            migrationBuilder.RenameColumn(
                name: "LockedByUserId",
                table: "QueuedTicket",
                newName: "LockedByServiceProviderId");

            migrationBuilder.CreateTable(
                name: "ServiceProvider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FullNames = table.Column<string>(type: "text", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    Password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceProvider_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProvider_ServiceId",
                table: "ServiceProvider",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderAssignments_ServiceProvider_ServiceProviderId",
                table: "ServiceProviderAssignments",
                column: "ServiceProviderId",
                principalTable: "ServiceProvider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
