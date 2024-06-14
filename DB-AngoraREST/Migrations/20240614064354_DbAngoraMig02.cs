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
            migrationBuilder.AddColumn<string>(
                name: "OriginId",
                table: "Rabbits",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rabbits_OriginId",
                table: "Rabbits",
                column: "OriginId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rabbits_AspNetUsers_OriginId",
                table: "Rabbits",
                column: "OriginId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rabbits_AspNetUsers_OriginId",
                table: "Rabbits");

            migrationBuilder.DropIndex(
                name: "IX_Rabbits_OriginId",
                table: "Rabbits");

            migrationBuilder.DropColumn(
                name: "OriginId",
                table: "Rabbits");
        }
    }
}
