using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreBE.Infrastructure.Persistence.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class InitPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OldTickets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RequestedFor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsOnBehalf = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    State = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OldTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxDomaintEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: true),
                    Error = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    OccuredUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    NextRetryUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxDomaintEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    TicketType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketHistorys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    TicketId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    Operation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Details = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketHistorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketHistorys_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OldTickets_CreatedAt",
                table: "OldTickets",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OldTickets_CreatedBy",
                table: "OldTickets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OldTickets_Id",
                table: "OldTickets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxDomaintEvents_CreatedAt",
                table: "OutboxDomaintEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxDomaintEvents_CreatedBy",
                table: "OutboxDomaintEvents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxDomaintEvents_Id",
                table: "OutboxDomaintEvents",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxDomaintEvents_Type_IsProcessed_NextRetryUtc_EntityId",
                table: "OutboxDomaintEvents",
                columns: new[] { "Type", "IsProcessed", "NextRetryUtc", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistorys_CreatedAt",
                table: "TicketHistorys",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistorys_CreatedBy",
                table: "TicketHistorys",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistorys_Id",
                table: "TicketHistorys",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistorys_TicketId",
                table: "TicketHistorys",
                column: "TicketId");

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
                name: "OldTickets");

            migrationBuilder.DropTable(
                name: "OutboxDomaintEvents");

            migrationBuilder.DropTable(
                name: "TicketHistorys");

            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
