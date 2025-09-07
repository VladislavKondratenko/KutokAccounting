using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutokAccounting.DataProvider.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "store",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    is_opened = table.Column<int>(type: "INTEGER", nullable: false),
                    setup_date = table.Column<long>(type: "INTEGER", nullable: false),
                    address = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "transaction_type",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    is_positive_value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_type", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vendor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vendor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "invoice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    created_at = table.Column<long>(type: "INTEGER", nullable: false),
                    number = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StoreId = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invoice_store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_invoice_vendor_VendorId",
                        column: x => x.VendorId,
                        principalTable: "vendor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    value = table.Column<int>(type: "INTEGER", nullable: false),
                    created_at = table.Column<long>(type: "INTEGER", nullable: false),
                    StoreId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_transaction_invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "invoice",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_transaction_store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transaction_transaction_type_TransactionTypeId",
                        column: x => x.TransactionTypeId,
                        principalTable: "transaction_type",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "index_created_at",
                table: "invoice",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_StoreId",
                table: "invoice",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_VendorId",
                table: "invoice",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "index_name",
                table: "store",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "index_created_at",
                table: "transaction",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_InvoiceId",
                table: "transaction",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_StoreId",
                table: "transaction",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_TransactionTypeId",
                table: "transaction",
                column: "TransactionTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transaction");

            migrationBuilder.DropTable(
                name: "invoice");

            migrationBuilder.DropTable(
                name: "transaction_type");

            migrationBuilder.DropTable(
                name: "store");

            migrationBuilder.DropTable(
                name: "vendor");
        }
    }
}
