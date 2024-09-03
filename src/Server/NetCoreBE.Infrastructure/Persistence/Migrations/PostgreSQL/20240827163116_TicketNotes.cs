using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreBE.Infrastructure.Persistence.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class TicketNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Tickets");

            migrationBuilder.AddColumn<List<string>>(
                name: "Notes",
                table: "Tickets",
                type: "text[]",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Tickets");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Tickets",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
