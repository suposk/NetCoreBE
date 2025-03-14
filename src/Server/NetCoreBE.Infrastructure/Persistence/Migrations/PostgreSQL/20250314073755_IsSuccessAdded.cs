using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreBE.Infrastructure.Persistence.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class IsSuccessAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                table: "OutboxDomaintEvents",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuccess",
                table: "OutboxDomaintEvents");
        }
    }
}
