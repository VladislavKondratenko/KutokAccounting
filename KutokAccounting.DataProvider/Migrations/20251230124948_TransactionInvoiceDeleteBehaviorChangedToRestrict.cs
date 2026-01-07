using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutokAccounting.DataProvider.Migrations
{
    /// <inheritdoc />
    public partial class TransactionInvoiceDeleteBehaviorChangedToRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transaction_invoice_InvoiceId",
                table: "transaction");

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_invoice_InvoiceId",
                table: "transaction",
                column: "InvoiceId",
                principalTable: "invoice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transaction_invoice_InvoiceId",
                table: "transaction");

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_invoice_InvoiceId",
                table: "transaction",
                column: "InvoiceId",
                principalTable: "invoice",
                principalColumn: "Id");
        }
    }
}
