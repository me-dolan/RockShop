using Microsoft.EntityFrameworkCore.Migrations;

namespace Rock_DataAccess.Migrations
{
    public partial class AddApplicationTypeToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ApplicationId",
                table: "Products",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ApplicationTypes_ApplicationId",
                table: "Products",
                column: "ApplicationId",
                principalTable: "ApplicationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ApplicationTypes_ApplicationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ApplicationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Products");
        }
    }
}
