using Microsoft.EntityFrameworkCore.Migrations;

namespace AtomStore.Data.EF.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderDetails_Products_PrductId",
                table: "orderDetails");

            migrationBuilder.DropIndex(
                name: "IX_orderDetails_PrductId",
                table: "orderDetails");

            migrationBuilder.DropColumn(
                name: "PrductId",
                table: "orderDetails");

            migrationBuilder.CreateIndex(
                name: "IX_orderDetails_ProductId",
                table: "orderDetails",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetails_Products_ProductId",
                table: "orderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderDetails_Products_ProductId",
                table: "orderDetails");

            migrationBuilder.DropIndex(
                name: "IX_orderDetails_ProductId",
                table: "orderDetails");

            migrationBuilder.AddColumn<int>(
                name: "PrductId",
                table: "orderDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_orderDetails_PrductId",
                table: "orderDetails",
                column: "PrductId");

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetails_Products_PrductId",
                table: "orderDetails",
                column: "PrductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
