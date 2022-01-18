using GCCars.Domain.Models.Battles;
using GCCars.Domain.Models.Cars;
using GCCars.Domain.Models.Identity;
using GCCars.Domain.Models.Transactions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.Design;

namespace GCCars.Application.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public DbSet<Car> Cars { get; set; }
        public IQueryable<Car> _Cars => Cars.AsNoTracking();

        public DbSet<History> History { get; set; }
        public IQueryable<History> _History => History.AsNoTracking();

        public DbSet<Parameter> Parameters { get; set; }
        public IQueryable<Parameter> _Parameters => Parameters.AsNoTracking();

        public DbSet<PvpBattle> PvpBattles { get; set; }
        public IQueryable<PvpBattle> _PvpBattles => PvpBattles.AsNoTracking();

        public DbSet<Fighter> Fighters { get; set; }
        public IQueryable<Fighter> _Fighters => Fighters.AsNoTracking();

        public DbSet<PveBattle> PveBattles { get; set; }
        public IQueryable<PveBattle> _PveBattles => PveBattles.AsNoTracking();

        public DbSet<Transaction> Transactions { get; set; }
        public IQueryable<Transaction> _Transactions => Transactions.AsNoTracking();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Car>()
                .HasIndex(r => r.Name);

            builder.Entity<History>()
                .HasOne(e => e.Car)
                .WithMany(e => e.History)
                .HasForeignKey(e => e.CarId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<History>()
                .HasOne(e => e.Owner)
                .WithMany(e => e.History)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Fighter>()
                .HasOne(r => r.PvpBattle)
                .WithMany(r => r.Fighters)
                .HasForeignKey(r => r.PvpBattleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Transaction>()
                .HasIndex(r => r.Address);
            builder.Entity<Transaction>()
                .HasIndex(r => r.StartTime);
        }
    }
    public class ContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=tcp:enigma.database.windows.net,1433;Initial Catalog=gccars;Persist Security Info=False;User ID=enigma;Password=Mgry123$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
