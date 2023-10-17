using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreBE.Api.Infrastructure.Persistence.Migrations.SqlServerMigrations
{
    /// <inheritdoc />
    public partial class Version : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Tickets");
        }
    }
}
