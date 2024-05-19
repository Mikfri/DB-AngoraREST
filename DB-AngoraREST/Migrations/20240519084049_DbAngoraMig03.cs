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
            migrationBuilder.AlterColumn<string>(
                name: "BreederRegNo",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rabbits_RightEarId_LeftEarId",
                table: "Rabbits",
                columns: new[] { "RightEarId", "LeftEarId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BreederRegNo",
                table: "AspNetUsers",
                column: "BreederRegNo",
                unique: true,
                filter: "[BreederRegNo] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rabbits_RightEarId_LeftEarId",
                table: "Rabbits");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BreederRegNo",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "BreederRegNo",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
