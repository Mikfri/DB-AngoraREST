using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB_AngoraREST.Migrations
{
    /// <inheritdoc />
    public partial class DbAngoraMig03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Rabbits_RabbitEarCombId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "LeftEarId",
                table: "Ratings");

            migrationBuilder.RenameColumn(
                name: "RightEarId",
                table: "Ratings",
                newName: "EarCombId");

            migrationBuilder.RenameColumn(
                name: "RabbitEarCombId",
                table: "Ratings",
                newName: "RabbitRatedEarCombId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_RabbitEarCombId",
                table: "Ratings",
                newName: "IX_Ratings_RabbitRatedEarCombId");

            migrationBuilder.CreateTable(
                name: "BreederApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateApplied = table.Column<DateOnly>(type: "date", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestedBreederRegNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentationPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreederApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BreederApplications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BreederApplications_UserId",
                table: "BreederApplications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Rabbits_RabbitRatedEarCombId",
                table: "Ratings",
                column: "RabbitRatedEarCombId",
                principalTable: "Rabbits",
                principalColumn: "EarCombId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Rabbits_RabbitRatedEarCombId",
                table: "Ratings");

            migrationBuilder.DropTable(
                name: "BreederApplications");

            migrationBuilder.RenameColumn(
                name: "RabbitRatedEarCombId",
                table: "Ratings",
                newName: "RabbitEarCombId");

            migrationBuilder.RenameColumn(
                name: "EarCombId",
                table: "Ratings",
                newName: "RightEarId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_RabbitRatedEarCombId",
                table: "Ratings",
                newName: "IX_Ratings_RabbitEarCombId");

            migrationBuilder.AddColumn<string>(
                name: "LeftEarId",
                table: "Ratings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Rabbits_RabbitEarCombId",
                table: "Ratings",
                column: "RabbitEarCombId",
                principalTable: "Rabbits",
                principalColumn: "EarCombId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
