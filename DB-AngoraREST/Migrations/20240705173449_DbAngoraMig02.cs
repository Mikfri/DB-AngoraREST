using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB_AngoraREST.Migrations
{
    /// <inheritdoc />
    public partial class DbAngoraMig02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RabbitTransfers_AspNetUsers_IssuerId",
                table: "RabbitTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_RabbitTransfers_AspNetUsers_RecipentId",
                table: "RabbitTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_RabbitTransfers_Rabbits_RabbitId",
                table: "RabbitTransfers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RabbitTransfers",
                table: "RabbitTransfers");

            migrationBuilder.RenameTable(
                name: "RabbitTransfers",
                newName: "TransferRequests");

            migrationBuilder.RenameIndex(
                name: "IX_RabbitTransfers_RecipentId",
                table: "TransferRequests",
                newName: "IX_TransferRequests_RecipentId");

            migrationBuilder.RenameIndex(
                name: "IX_RabbitTransfers_RabbitId",
                table: "TransferRequests",
                newName: "IX_TransferRequests_RabbitId");

            migrationBuilder.RenameIndex(
                name: "IX_RabbitTransfers_IssuerId",
                table: "TransferRequests",
                newName: "IX_TransferRequests_IssuerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransferRequests",
                table: "TransferRequests",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_AspNetUsers_IssuerId",
                table: "TransferRequests",
                column: "IssuerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_AspNetUsers_RecipentId",
                table: "TransferRequests",
                column: "RecipentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_Rabbits_RabbitId",
                table: "TransferRequests",
                column: "RabbitId",
                principalTable: "Rabbits",
                principalColumn: "EarCombId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_AspNetUsers_IssuerId",
                table: "TransferRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_AspNetUsers_RecipentId",
                table: "TransferRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_Rabbits_RabbitId",
                table: "TransferRequests");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransferRequests",
                table: "TransferRequests");

            migrationBuilder.RenameTable(
                name: "TransferRequests",
                newName: "RabbitTransfers");

            migrationBuilder.RenameIndex(
                name: "IX_TransferRequests_RecipentId",
                table: "RabbitTransfers",
                newName: "IX_RabbitTransfers_RecipentId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferRequests_RabbitId",
                table: "RabbitTransfers",
                newName: "IX_RabbitTransfers_RabbitId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferRequests_IssuerId",
                table: "RabbitTransfers",
                newName: "IX_RabbitTransfers_IssuerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RabbitTransfers",
                table: "RabbitTransfers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RabbitTransfers_AspNetUsers_IssuerId",
                table: "RabbitTransfers",
                column: "IssuerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RabbitTransfers_AspNetUsers_RecipentId",
                table: "RabbitTransfers",
                column: "RecipentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RabbitTransfers_Rabbits_RabbitId",
                table: "RabbitTransfers",
                column: "RabbitId",
                principalTable: "Rabbits",
                principalColumn: "EarCombId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
