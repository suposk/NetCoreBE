﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreBE.Api.Infrastructure.Persistence.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboxMessageDomaintEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequestedFor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsOnBehalf = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    State = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestHistorys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Operation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestHistorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestHistorys_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id");
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
                name: "IX_OutboxMessageDomaintEvents_Type_IsProcessed_NextRetryUtc_EntityId",
                table: "OutboxMessageDomaintEvents",
                columns: new[] { "Type", "IsProcessed", "NextRetryUtc", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestHistorys_CreatedAt",
                table: "RequestHistorys",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHistorys_CreatedBy",
                table: "RequestHistorys",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHistorys_Id",
                table: "RequestHistorys",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHistorys_RequestId",
                table: "RequestHistorys",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CreatedAt",
                table: "Requests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CreatedBy",
                table: "Requests",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Id",
                table: "Requests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatedAt",
                table: "Tickets",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatedBy",
                table: "Tickets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Id",
                table: "Tickets",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessageDomaintEvents");

            migrationBuilder.DropTable(
                name: "RequestHistorys");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Requests");
        }
    }
}
