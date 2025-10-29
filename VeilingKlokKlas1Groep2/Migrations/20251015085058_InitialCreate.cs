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
                name: "Accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Kopers",
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
                    table.PrimaryKey("PK_Kopers", x => x.id);
                    table.ForeignKey(
                        name: "FK_Kopers_Accounts_id",
                        column: x => x.id,
                        principalTable: "Accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "kwekers",
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
                    table.PrimaryKey("PK_kwekers", x => x.id);
                    table.ForeignKey(
                        name: "FK_kwekers_Accounts_id",
                        column: x => x.id,
                        principalTable: "Accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "veilingmeesters",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    soort_veiling = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    regio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_veilingmeesters", x => x.id);
                    table.ForeignKey(
                        name: "FK_veilingmeesters_Accounts_id",
                        column: x => x.id,
                        principalTable: "Accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    bought_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    koper_id = table.Column<int>(type: "int", nullable: false),
                    kweker_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_orders_Kopers_koper_id",
                        column: x => x.koper_id,
                        principalTable: "Kopers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_orders_kwekers_kweker_id",
                        column: x => x.kweker_id,
                        principalTable: "kwekers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "veilingklokken",
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
                    table.PrimaryKey("PK_veilingklokken", x => x.id);
                    table.ForeignKey(
                        name: "FK_veilingklokken_veilingmeesters_veilingmeester_id",
                        column: x => x.veilingmeester_id,
                        principalTable: "veilingmeesters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_email",
                table: "Accounts",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_koper_id",
                table: "orders",
                column: "koper_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_kweker_id",
                table: "orders",
                column: "kweker_id");

            migrationBuilder.CreateIndex(
                name: "IX_veilingklokken_veilingmeester_id",
                table: "veilingklokken",
                column: "veilingmeester_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "veilingklokken");

            migrationBuilder.DropTable(
                name: "Kopers");

            migrationBuilder.DropTable(
                name: "kwekers");

            migrationBuilder.DropTable(
                name: "veilingmeesters");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
