using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourPlanner.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteGeometry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RouteGeometryJson",
                table: "Tours",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RouteGeometryJson",
                table: "Tours");
        }
    }
}
