using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtomStore.Data.EF.Migrations
{
    public partial class create_viewedlist_tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishList_Products_ProductId",
                table: "WishList");

            migrationBuilder.DropForeignKey(
                name: "FK_WishList_AppUsers_UserId",
                table: "WishList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WishList",
                table: "WishList");

            migrationBuilder.RenameTable(
                name: "WishList",
                newName: "Wishlists");

            migrationBuilder.RenameIndex(
                name: "IX_WishList_UserId",
                table: "Wishlists",
                newName: "IX_Wishlists_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_WishList_ProductId",
                table: "Wishlists",
                newName: "IX_Wishlists_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wishlists",
                table: "Wishlists",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Viewedlists",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viewedlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Viewedlists_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Viewedlists_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Viewedlists_ProductId",
                table: "Viewedlists",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Viewedlists_UserId",
                table: "Viewedlists",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_Products_ProductId",
                table: "Wishlists",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_AppUsers_UserId",
                table: "Wishlists",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_Products_ProductId",
                table: "Wishlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_AppUsers_UserId",
                table: "Wishlists");

            migrationBuilder.DropTable(
                name: "Viewedlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wishlists",
                table: "Wishlists");

            migrationBuilder.RenameTable(
                name: "Wishlists",
                newName: "WishList");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlists_UserId",
                table: "WishList",
                newName: "IX_WishList_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlists_ProductId",
                table: "WishList",
                newName: "IX_WishList_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WishList",
                table: "WishList",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WishList_Products_ProductId",
                table: "WishList",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishList_AppUsers_UserId",
                table: "WishList",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
