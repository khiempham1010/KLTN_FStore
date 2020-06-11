using Microsoft.EntityFrameworkCore.Migrations;

namespace AtomStore.Data.EF.Migrations
{
    public partial class add_column_image_to_Productfeeback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "ProductFeedbacks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "ProductFeedbacks");
        }
    }
}
