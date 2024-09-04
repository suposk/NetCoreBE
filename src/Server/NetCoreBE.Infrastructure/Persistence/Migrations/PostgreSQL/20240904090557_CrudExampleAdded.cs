using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreBE.Infrastructure.Persistence.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class CrudExampleAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrudExamples",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrudExamples", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrudExamples_CreatedAt",
                table: "CrudExamples",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CrudExamples_CreatedBy",
                table: "CrudExamples",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CrudExamples_Id",
                table: "CrudExamples",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrudExamples");
        }
    }
}
