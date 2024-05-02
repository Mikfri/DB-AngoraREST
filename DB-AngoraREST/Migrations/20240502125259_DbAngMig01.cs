using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DB_AngoraREST.Migrations
{
    /// <inheritdoc />
    public partial class DbAngMig01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoadNameAndNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rabbits",
                columns: table => new
                {
                    RightEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Race = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<int>(type: "int", nullable: false),
                    ApprovedRaceColorCombination = table.Column<bool>(type: "bit", nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    DateOfDeath = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    IsPublic = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rabbits", x => new { x.RightEarId, x.LeftEarId });
                    table.ForeignKey(
                        name: "FK_Rabbits_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RabbitLeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RabbitRightEarId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Rabbits_RabbitRightEarId_RabbitLeftEarId",
                        columns: x => new { x.RabbitRightEarId, x.RabbitLeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" });
                });

            migrationBuilder.CreateTable(
                name: "RabbitParents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MotherRightEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MotherLeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FatherRightEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FatherLeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChildRightEarId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChildLeftEarId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RabbitChildRightEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RabbitChildLeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RabbitParents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RabbitParents_Rabbits_FatherRightEarId_FatherLeftEarId",
                        columns: x => new { x.FatherRightEarId, x.FatherLeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" });
                    table.ForeignKey(
                        name: "FK_RabbitParents_Rabbits_MotherRightEarId_MotherLeftEarId",
                        columns: x => new { x.MotherRightEarId, x.MotherLeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" });
                    table.ForeignKey(
                        name: "FK_RabbitParents_Rabbits_RabbitChildRightEarId_RabbitChildLeftEarId",
                        columns: x => new { x.RabbitChildRightEarId, x.RabbitChildLeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RightEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateRated = table.Column<DateOnly>(type: "date", nullable: false),
                    WeightPoint = table.Column<int>(type: "int", nullable: false),
                    WeightNotice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyPoint = table.Column<int>(type: "int", nullable: false),
                    BodyNotice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FurPoint = table.Column<int>(type: "int", nullable: false),
                    FurNotice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalPoint = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Rabbits_RightEarId_LeftEarId",
                        columns: x => new { x.RightEarId, x.LeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "City", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsAdmin", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RoadNameAndNo", "SecurityStamp", "TwoFactorEnabled", "UserName", "ZipCode" },
                values: new object[,]
                {
                    { "5053", 0, "Benløse", "562200cc-3561-4065-9a0b-1d67eedfed05", "MajaJoensen89@gmail.com", false, "Maja", false, "Hulstrøm", false, null, null, null, "AQAAAAIAAYagAAAAEGQuyYe6f7chvubR9A75SurPrJhRKgihyjzDLX1+WqBE5VM/+2LtnUF+Aa/cRTmc8w==", "28733085", false, "Sletten 4", "039c721e-82da-4687-be75-549217d258db", false, null, 4100 },
                    { "5095", 0, "Kirke Såby", "412e0ac1-c6a8-4a3d-8966-6afe7aa9ec28", "IdaFribor87@gmail.com", false, "Ida", true, "Friborg", false, null, null, null, "AQAAAAIAAYagAAAAEM+w4FIfD/YOwnbGlTs2d92NrQRTdWhp7+hVU5P3NQeA+S3DS4QovgwrgjqHxjsprg==", "27586455", false, "Fynsvej 14", "b0648f8f-cfc7-46a4-a3c8-9361312229f9", false, null, 4060 }
                });

            migrationBuilder.InsertData(
                table: "Rabbits",
                columns: new[] { "LeftEarId", "RightEarId", "ApprovedRaceColorCombination", "Color", "DateOfBirth", "DateOfDeath", "Gender", "IsPublic", "NickName", "OwnerId", "Race" },
                values: new object[,]
                {
                    { "3020", "4398", null, 26, new DateOnly(2022, 7, 22), null, 1, 0, "Douglas", "5053", 10 },
                    { "105", "4640", null, 13, new DateOnly(2021, 4, 5), null, 1, 0, "Ingolf", "5095", 0 },
                    { "120", "4640", null, 16, new DateOnly(2021, 5, 11), new DateOnly(2023, 11, 3), 0, 0, "Mulan", "5095", 0 },
                    { "206", "4977", null, 15, new DateOnly(2022, 2, 2), null, 1, 0, "Dario", "5053", 17 },
                    { "213", "4977", null, 6, new DateOnly(2022, 3, 24), null, 0, 0, "Frida", "5053", 17 },
                    { "315", "4977", null, 15, new DateOnly(2023, 1, 13), null, 0, 0, "Miranda", "5053", 17 },
                    { "0223", "5053", null, 16, new DateOnly(2023, 5, 30), null, 0, 0, "Chinchou", "5053", 17 },
                    { "0423", "5053", null, 15, new DateOnly(2023, 5, 30), null, 0, 0, "Gastly", "5053", 17 },
                    { "0623", "5053", null, 19, new DateOnly(2023, 8, 17), null, 0, 0, "Karla", "5053", 17 },
                    { "0723", "5053", null, 21, new DateOnly(2024, 10, 15), null, 1, 0, "Sandshrew", "5053", 17 },
                    { "0823", "5053", null, 25, new DateOnly(2024, 10, 15), null, 0, 0, "Pepsi", "5053", 17 },
                    { "0923", "5053", null, 25, new DateOnly(2024, 10, 15), new DateOnly(2024, 3, 14), 1, 0, "Cola", "5053", 17 },
                    { "1023", "5053", null, 21, new DateOnly(2024, 10, 15), null, 0, 0, "Marabou", "5053", 17 },
                    { "001", "5095", null, 18, new DateOnly(2019, 2, 27), new DateOnly(2024, 4, 13), 0, 0, "Kaliba", "5095", 0 },
                    { "002", "5095", null, 16, new DateOnly(2020, 6, 12), new DateOnly(2022, 7, 22), 0, 0, "Sov", "5095", 0 },
                    { "003", "5095", null, 26, new DateOnly(2020, 3, 12), new DateOnly(2023, 11, 3), 0, 0, "Smørklat Smør", "5095", 0 },
                    { "2104", "M63", null, 30, new DateOnly(2023, 5, 22), null, 0, 0, "Ortovi", "5053", 17 },
                    { "3102", "M63", null, 19, new DateOnly(2023, 9, 23), null, 0, 0, "Xådda", "5053", 17 },
                    { "023", "V23", null, 4, new DateOnly(2020, 4, 10), new DateOnly(2024, 4, 23), 1, 0, "Aslan", "5053", 17 }
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
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_RabbitRightEarId_RabbitLeftEarId",
                table: "Photos",
                columns: new[] { "RabbitRightEarId", "RabbitLeftEarId" });

            migrationBuilder.CreateIndex(
                name: "IX_RabbitParents_FatherRightEarId_FatherLeftEarId",
                table: "RabbitParents",
                columns: new[] { "FatherRightEarId", "FatherLeftEarId" });

            migrationBuilder.CreateIndex(
                name: "IX_RabbitParents_MotherRightEarId_MotherLeftEarId",
                table: "RabbitParents",
                columns: new[] { "MotherRightEarId", "MotherLeftEarId" });

            migrationBuilder.CreateIndex(
                name: "IX_RabbitParents_RabbitChildRightEarId_RabbitChildLeftEarId",
                table: "RabbitParents",
                columns: new[] { "RabbitChildRightEarId", "RabbitChildLeftEarId" });

            migrationBuilder.CreateIndex(
                name: "IX_Rabbits_OwnerId",
                table: "Rabbits",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_RightEarId_LeftEarId",
                table: "Ratings",
                columns: new[] { "RightEarId", "LeftEarId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "RabbitParents");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Rabbits");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
