using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
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
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    deleted_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    token_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    jti = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    revoked_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.token_id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_Account_account_id",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Veilingmeester",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    country_code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    state_or_province = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    authorisatie_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
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
                name: "Veilingklok",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    status = table.Column<int>(type: "int", nullable: false),
                    peaked_live_views = table.Column<int>(type: "int", nullable: false),
                    bidding_product_index = table.Column<int>(type: "int", nullable: false),
                    veiling_duration = table.Column<int>(type: "int", nullable: false),
                    scheduled_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ended_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    highest_price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    lowest_price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    state_or_province = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    country = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    veilingmeester_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingklok", x => x.id);
                    table.ForeignKey(
                        name: "FK_Veilingklok_Veilingmeester_veilingmeester_id",
                        column: x => x.veilingmeester_id,
                        principalTable: "Veilingmeester",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Adresses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    street = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    state_or_province = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    postal_code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    country = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adresses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Koper",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    first_name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    primary_adress_id = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Koper_Adresses_primary_adress_id",
                        column: x => x.primary_adress_id,
                        principalTable: "Adresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kweker",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    kvk_nmr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    company_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    adress_id = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Kweker_Adresses_adress_id",
                        column: x => x.adress_id,
                        principalTable: "Adresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    status = table.Column<int>(type: "int", nullable: false),
                    closed_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    koper_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    veilingklok_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Order_Veilingklok_veilingklok_id",
                        column: x => x.veilingklok_id,
                        principalTable: "Veilingklok",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    image_base64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dimension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    auction_price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    minimum_price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    stock = table.Column<int>(type: "int", nullable: false),
                    auctioned_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    auctioned_count = table.Column<int>(type: "int", nullable: false),
                    kweker_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    veilingklok_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.id);
                    table.CheckConstraint("CK_Product_MinimumPrice", "[auction_price] IS NULL OR [minimum_price] <= [auction_price]");
                    table.ForeignKey(
                        name: "FK_Product_Kweker_kweker_id",
                        column: x => x.kweker_id,
                        principalTable: "Kweker",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Veilingklok_veilingklok_id",
                        column: x => x.veilingklok_id,
                        principalTable: "Veilingklok",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    veilingklok_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_order_id",
                        column: x => x.order_id,
                        principalTable: "Order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItem_Product_product_id",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItem_Veilingklok_veilingklok_id",
                        column: x => x.veilingklok_id,
                        principalTable: "Veilingklok",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_email",
                table: "Account",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Adresses_account_id",
                table: "Adresses",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Koper_primary_adress_id",
                table: "Koper",
                column: "primary_adress_id");

            migrationBuilder.CreateIndex(
                name: "IX_Kweker_adress_id",
                table: "Kweker",
                column: "adress_id");

            migrationBuilder.CreateIndex(
                name: "IX_Kweker_kvk_nmr",
                table: "Kweker",
                column: "kvk_nmr",
                unique: true,
                filter: "[kvk_nmr] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Order_koper_id",
                table: "Order",
                column: "koper_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_veilingklok_id",
                table: "Order",
                column: "veilingklok_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_order_id",
                table: "OrderItem",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_product_id",
                table: "OrderItem",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_veilingklok_id",
                table: "OrderItem",
                column: "veilingklok_id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_kweker_id",
                table: "Product",
                column: "kweker_id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_veilingklok_id",
                table: "Product",
                column: "veilingklok_id");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_account_id",
                table: "RefreshToken",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_jti",
                table: "RefreshToken",
                column: "jti");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingklok_veilingmeester_id",
                table: "Veilingklok",
                column: "veilingmeester_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Adresses_Koper_account_id",
                table: "Adresses",
                column: "account_id",
                principalTable: "Koper",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Adresses_Koper_account_id",
                table: "Adresses");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Kweker");

            migrationBuilder.DropTable(
                name: "Veilingklok");

            migrationBuilder.DropTable(
                name: "Veilingmeester");

            migrationBuilder.DropTable(
                name: "Koper");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Adresses");
        }
    }
}
