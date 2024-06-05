using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardDelivery.Migrations
{
    public partial class AddDeliveryPriceColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "Deliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Deliveries");
        }
    }
}
