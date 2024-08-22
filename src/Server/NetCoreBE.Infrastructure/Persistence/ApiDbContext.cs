using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace NetCoreBE.Infrastructure.Persistence;

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

    public DbSet<OldTicket> OldTickets { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketHistory> TicketHistorys { get; set; }
    public DbSet<OutboxDomaintEvent> OutboxDomaintEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //example
        //builder.Entity<Ticket>()
        //    .Property(c => c.Version)
        //        .HasDefaultValue(0)
        //        .IsRowVersion();

        builder.Entity<OldTicket>().Property(c => c.RowVersion).IsRowVersion();
        builder.Entity<Ticket>().Property(c => c.RowVersion).IsRowVersion();
        builder.Entity<TicketHistory>().Property(c => c.RowVersion).IsRowVersion();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {            
            if (!string.IsNullOrWhiteSpace(_connectionString))
            {
                //optionsBuilder.UseSqlite(_connectionString);
                optionsBuilder.UseSqlServer(_connectionString);
            }            
        }
        //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

}