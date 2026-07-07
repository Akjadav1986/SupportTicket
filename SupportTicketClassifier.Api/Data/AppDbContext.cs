using Microsoft.EntityFrameworkCore;
using SupportTicketClassifier.Api.Models;

namespace SupportTicketClassifier.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketClassification> TicketClassifications => Set<TicketClassification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Tickets");
            entity.Property(x => x.Status)
                  .HasConversion<string>()
                  .HasMaxLength(30);

            entity.Property(x => x.CreatedAtUtc)
                  .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasOne(x => x.Classification)
                  .WithOne(x => x.Ticket)
                  .HasForeignKey<TicketClassification>(x => x.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TicketClassification>(entity =>
        {
            entity.ToTable("TicketClassifications");
            entity.Property(x => x.Confidence)
                  .HasPrecision(5, 2);

            entity.Property(x => x.ClassifiedAtUtc)
                  .HasDefaultValueSql("SYSUTCDATETIME()");
        });
    }
}
