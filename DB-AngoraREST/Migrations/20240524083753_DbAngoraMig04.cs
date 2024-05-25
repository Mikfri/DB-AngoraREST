using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB_AngoraREST.Migrations
{
    /// <inheritdoc />
    public partial class DbAngoraMig04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedRaceColorCombination",
                table: "Rabbits");

            migrationBuilder.RenameColumn(
                name: "IsPublic",
                table: "Rabbits",
                newName: "OpenProfile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OpenProfile",
                table: "Rabbits",
                newName: "IsPublic");

            migrationBuilder.AddColumn<bool>(
                name: "ApprovedRaceColorCombination",
                table: "Rabbits",
                type: "bit",
                nullable: true);
        }
    }
}
