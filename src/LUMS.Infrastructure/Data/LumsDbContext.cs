using LUMS.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LUMS.Infrastructure.Data;

public class LumsDbContext : DbContext
{
    public LumsDbContext(DbContextOptions<LumsDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<BookTitle> BookTitles => Set<BookTitle>();
    public DbSet<Copy> Copies => Set<Copy>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Fine> Fines => Set<Fine>();
    public DbSet<PolicyConfiguration> PolicyConfigurations => Set<PolicyConfiguration>();
    public DbSet<SystemLog> SystemLogs => Set<SystemLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LumsDbContext).Assembly);

        foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
