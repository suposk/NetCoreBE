// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace NetCoreBE.Api.Infrastructure.Persistence;

#nullable disable
public class ApiDbContext : DbContext
{
    private readonly string _connectionString;

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

    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
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