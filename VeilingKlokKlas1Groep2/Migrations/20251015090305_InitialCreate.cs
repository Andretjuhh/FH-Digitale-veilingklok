using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeilingKlokKlas1Groep2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Koper",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    adress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    post_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    regio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Koper", x => x.id);
                    table.ForeignKey(
                        name: "FK_Koper_Account_id",
                        column: x => x.id,
                        principalTable: "Account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kweker",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    adress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    regio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    kvk_nmr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kweker", x => x.id);
                    table.ForeignKey(
                        name: "FK_Kweker_Account_id",
                        column: x => x.id,
                        principalTable: "Account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Veilingmeester",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    soort_veiling = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    regio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingmeester", x => x.id);
                    table.ForeignKey(
                        name: "FK_Veilingmeester_Account_id",
                        column: x => x.id,
                        principalTable: "Account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    bought_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    koper_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.id);
                    table.ForeignKey(
                        name: "FK_Order_Koper_koper_id",
                        column: x => x.koper_id,
                        principalTable: "Koper",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Veilingklok",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    duration_in_seconds = table.Column<int>(type: "int", nullable: false),
                    live_views = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    veilingmeester_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingklok", x => x.id);
                    table.ForeignKey(
                        name: "FK_Veilingklok_Veilingmeester_veilingmeester_id",
                        column: x => x.veilingmeester_id,
                        principalTable: "Veilingmeester",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_email",
                table: "Account",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_koper_id",
                table: "Order",
                column: "koper_id");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingklok_veilingmeester_id",
                table: "Veilingklok",
                column: "veilingmeester_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kweker");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Veilingklok");

            migrationBuilder.DropTable(
                name: "Koper");

            migrationBuilder.DropTable(
                name: "Veilingmeester");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
