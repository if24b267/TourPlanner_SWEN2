using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourPlanner.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TourDescription = table.Column<string>(type: "text", nullable: false),
                    From = table.Column<string>(type: "text", nullable: false),
                    To = table.Column<string>(type: "text", nullable: false),
                    TransportType = table.Column<string>(type: "text", nullable: false),
                    TourDistance = table.Column<double>(type: "double precision", nullable: false),
                    EstimatedTimeHours = table.Column<double>(type: "double precision", nullable: false),
                    RouteImagePath = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    TotalDistance = table.Column<double>(type: "double precision", nullable: false),
                    TotalTimeHours = table.Column<double>(type: "double precision", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourLogs_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourLogs_TourId",
                table: "TourLogs",
                column: "TourId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourLogs");

            migrationBuilder.DropTable(
                name: "Tours");
        }
    }
}
