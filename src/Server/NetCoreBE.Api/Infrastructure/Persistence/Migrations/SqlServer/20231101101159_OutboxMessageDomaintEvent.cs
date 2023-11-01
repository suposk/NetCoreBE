using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreBE.Api.Infrastructure.Persistence.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class OutboxMessageDomaintEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboxMessageDomaintEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TypeDetail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OccuredUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    NextRetryUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessageDomaintEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessageDomaintEvents_CreatedAt",
                table: "OutboxMessageDomaintEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessageDomaintEvents_CreatedBy",
                table: "OutboxMessageDomaintEvents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessageDomaintEvents_Id",
                table: "OutboxMessageDomaintEvents",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessageDomaintEvents_Type_IsSuccess_NextRetryUtc",
                table: "OutboxMessageDomaintEvents",
                columns: new[] { "Type", "IsSuccess", "NextRetryUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessageDomaintEvents");
        }
    }
}
