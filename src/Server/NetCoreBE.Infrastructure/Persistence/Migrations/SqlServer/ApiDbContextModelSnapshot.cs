﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetCoreBE.Infrastructure.Persistence;

#nullable disable

namespace NetCoreBE.Infrastructure.Persistence.Migrations.SqlServer
{
    [DbContext(typeof(ApiDbContext))]
    partial class ApiDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("NetCoreBE.Domain.Entities.OldTicket", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<bool>("IsOnBehalf")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("RequestedFor")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("State")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Status")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("Id");

                    b.ToTable("OldTickets");
                });

            modelBuilder.Entity("NetCoreBE.Domain.Entities.OutboxDomaintEvent", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("EntityId")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Error")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool?>("IsProcessed")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime?>("NextRetryUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("OccuredUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ProcessedUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("RetryCount")
                        .HasColumnType("int");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Type")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("Id");

                    b.HasIndex("Type", "IsProcessed", "NextRetryUtc", "EntityId");

                    b.ToTable("OutboxDomaintEvents");
                });

            modelBuilder.Entity("NetCoreBE.Domain.Entities.TicketHistory", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Details")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Operation")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("TicketId")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("Id");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketHistorys");
                });

            modelBuilder.Entity("Ticket", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Note")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TicketType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("Id");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("NetCoreBE.Domain.Entities.TicketHistory", b =>
                {
                    b.HasOne("Ticket", "Ticket")
                        .WithMany("TicketHistoryList")
                        .HasForeignKey("TicketId");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Ticket", b =>
                {
                    b.Navigation("TicketHistoryList");
                });
#pragma warning restore 612, 618
        }
    }
}
