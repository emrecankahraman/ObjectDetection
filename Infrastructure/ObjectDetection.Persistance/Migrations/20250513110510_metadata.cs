using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ObjectDetection.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class metadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Images",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Images",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Road",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Road",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Images");
        }
    }
}
