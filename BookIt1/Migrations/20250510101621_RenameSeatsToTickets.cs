using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookIt1.Migrations
{
    /// <inheritdoc />
    public partial class RenameSeatsToTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalSeats",
                table: "Events",
                newName: "TotalTickets");

            migrationBuilder.RenameColumn(
                name: "AvailableSeats",
                table: "Events",
                newName: "AvailableTickets");

            migrationBuilder.RenameColumn(
                name: "SeatsBooked",
                table: "Bookings",
                newName: "TicketsBooked");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalTickets",
                table: "Events",
                newName: "TotalSeats");

            migrationBuilder.RenameColumn(
                name: "AvailableTickets",
                table: "Events",
                newName: "AvailableSeats");

            migrationBuilder.RenameColumn(
                name: "TicketsBooked",
                table: "Bookings",
                newName: "SeatsBooked");
        }
    }
}
