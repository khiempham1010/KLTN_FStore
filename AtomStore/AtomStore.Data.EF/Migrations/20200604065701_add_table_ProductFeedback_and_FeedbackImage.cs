using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtomStore.Data.EF.Migrations
{
    public partial class add_table_ProductFeedback_and_FeedbackImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    Content = table.Column<string>(maxLength: 255, nullable: false),
                    Rating = table.Column<decimal>(nullable: false),
                    ParentId = table.Column<int>(nullable: false),
                    Like = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductFeedbacks_AppUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductFeedbacks_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackImage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductFeedbackId = table.Column<int>(nullable: false),
                    Path = table.Column<string>(maxLength: 250, nullable: true),
                    Caption = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackImage_ProductFeedbacks_ProductFeedbackId",
                        column: x => x.ProductFeedbackId,
                        principalTable: "ProductFeedbacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackImage_ProductFeedbackId",
                table: "FeedbackImage",
                column: "ProductFeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeedbacks_OwnerId",
                table: "ProductFeedbacks",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeedbacks_ProductId",
                table: "ProductFeedbacks",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedbackImage");

            migrationBuilder.DropTable(
                name: "ProductFeedbacks");
        }
    }
}
