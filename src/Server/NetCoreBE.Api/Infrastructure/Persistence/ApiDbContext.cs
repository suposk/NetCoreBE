// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;

namespace NetCoreBE.Api.Infrastructure.Persistence;

#nullable disable
public class ApiDbContext : DbContext, IApiDbContext
{
    private readonly string _connectionString;

    #region Constructor
    public ApiDbContext() : base()
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
    public ApiDbContext(string connectionString) : base()
    {
        _connectionString = connectionString;
        //exception here
        //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public ApiDbContext(DbContextOptions<ApiDbContext> options)
       : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
    #endregion

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<RequestHistory> RequestHistorys { get; set; }
    public DbSet<OutboxMessageDomaintEvent> OutboxMessageDomaintEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //builder.Entity<Ticket>()
        //    .Property(c => c.Version)
        //        .HasDefaultValue(0)
        //        .IsRowVersion();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //sql Lite
            if (!string.IsNullOrWhiteSpace(_connectionString))
            {
                //optionsBuilder.UseSqlite(_connectionString);
                optionsBuilder.UseSqlServer(_connectionString);
            }
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
    }

}