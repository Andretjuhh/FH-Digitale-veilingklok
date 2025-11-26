using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeilingKlokKlas1Groep2.Migrations
{
    /// <inheritdoc />
    public partial class DBFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Kweker_kweker_id",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Veilingklok_Veilingmeester_veilingmeester_id",
                table: "Veilingklok");

            migrationBuilder.AddColumn<int>(
                name: "veilingklok_id",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Product_veilingklok_id",
                table: "Product",
                column: "veilingklok_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Kweker_kweker_id",
                table: "Product",
                column: "kweker_id",
                principalTable: "Kweker",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Veilingklok_veilingklok_id",
                table: "Product",
                column: "veilingklok_id",
                principalTable: "Veilingklok",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Veilingklok_Veilingmeester_veilingmeester_id",
                table: "Veilingklok",
                column: "veilingmeester_id",
                principalTable: "Veilingmeester",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Kweker_kweker_id",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Veilingklok_veilingklok_id",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Veilingklok_Veilingmeester_veilingmeester_id",
                table: "Veilingklok");

            migrationBuilder.DropIndex(
                name: "IX_Product_veilingklok_id",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "veilingklok_id",
                table: "Product");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Kweker_kweker_id",
                table: "Product",
                column: "kweker_id",
                principalTable: "Kweker",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Veilingklok_Veilingmeester_veilingmeester_id",
                table: "Veilingklok",
                column: "veilingmeester_id",
                principalTable: "Veilingmeester",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
