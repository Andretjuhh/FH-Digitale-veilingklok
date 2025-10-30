using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeilingKlokKlas1Groep2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "regio",
                table: "Veilingmeester");

            migrationBuilder.DropColumn(
                name: "regio",
                table: "Kweker");

            migrationBuilder.DropColumn(
                name: "regio",
                table: "Koper");

            migrationBuilder.AddColumn<string>(
                name: "regio",
                table: "Account",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "regio",
                table: "Account");

            migrationBuilder.AddColumn<string>(
                name: "regio",
                table: "Veilingmeester",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "regio",
                table: "Kweker",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "regio",
                table: "Koper",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
