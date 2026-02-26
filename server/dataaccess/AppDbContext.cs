using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace dataaccess;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Turbine> Turbines => Set<Turbine>();
    public DbSet<TelemetryReading> TelemetryReadings => Set<TelemetryReading>();
    public DbSet<AlertEvent> AlertEvents => Set<AlertEvent>();
    public DbSet<TurbineCommand> TurbineCommands => Set<TurbineCommand>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---- AppUser
        modelBuilder.Entity<AppUser>(e =>
        {
            e.ToTable("app_users");
            e.HasKey(x => x.Id);

            e.Property(x => x.Username).HasMaxLength(64).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();

            e.HasIndex(x => x.Username).IsUnique();
        });

        // ---- Turbine
        modelBuilder.Entity<Turbine>(e =>
        {
            e.ToTable("turbines");
            e.HasKey(x => x.Id);

            e.Property(x => x.TurbineId).HasMaxLength(64).IsRequired();
            e.Property(x => x.TurbineName).HasMaxLength(128).IsRequired();
            e.Property(x => x.FarmId).HasMaxLength(128).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();

            e.HasIndex(x => x.TurbineId).IsUnique();
            e.HasIndex(x => new { x.FarmId, x.TurbineId }).IsUnique();
        });

        // ---- TelemetryReading
        modelBuilder.Entity<TelemetryReading>(e =>
        {
            e.ToTable("telemetry_readings");
            e.HasKey(x => x.Id);

            e.Property(x => x.Timestamp).IsRequired();
            e.Property(x => x.Status).HasConversion<int>().IsRequired();

            e.HasOne(x => x.Turbine)
                .WithMany(t => t.Telemetry)
                .HasForeignKey(x => x.TurbineIdFk)
                .OnDelete(DeleteBehavior.Cascade);

            // for graphs: fast query by turbine + time
            e.HasIndex(x => new { x.TurbineIdFk, x.Timestamp });
        });

        // ---- AlertEvent
        modelBuilder.Entity<AlertEvent>(e =>
        {
            e.ToTable("alert_events");
            e.HasKey(x => x.Id);

            e.Property(x => x.Timestamp).IsRequired();
            e.Property(x => x.Severity).HasConversion<int>().IsRequired();
            e.Property(x => x.Message).HasMaxLength(512).IsRequired();

            e.HasOne(x => x.Turbine)
                .WithMany(t => t.Alerts)
                .HasForeignKey(x => x.TurbineIdFk)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.TurbineIdFk, x.Timestamp });
        });

        // ---- TurbineCommand
        modelBuilder.Entity<TurbineCommand>(e =>
        {
            e.ToTable("turbine_commands");
            e.HasKey(x => x.Id);

            e.Property(x => x.Timestamp).IsRequired();
            e.Property(x => x.Action).HasConversion<int>().IsRequired();
            e.Property(x => x.PayloadJson).IsRequired();
            e.Property(x => x.Published).IsRequired();

            e.HasOne(x => x.Turbine)
                .WithMany(t => t.Commands)
                .HasForeignKey(x => x.TurbineIdFk)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.User)
                .WithMany(u => u.Commands)
                .HasForeignKey(x => x.UserIdFk)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.TurbineIdFk, x.Timestamp });
            e.HasIndex(x => new { x.UserIdFk, x.Timestamp });
        });
    }
}