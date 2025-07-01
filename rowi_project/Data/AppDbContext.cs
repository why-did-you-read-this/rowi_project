using Microsoft.EntityFrameworkCore;
using rowi_project.Models.Entities;

namespace rowi_project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Agent> Agents => Set<Agent>();
        public DbSet<Bank> Banks => Set<Bank>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<AgentBank> AgentBanks => Set<AgentBank>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgentBank>()
                .HasKey(ab => new { ab.AgentId, ab.BankId });

            modelBuilder.Entity<AgentBank>()
                .HasOne(ab => ab.Agent)
                .WithMany(a => a.AgentBanks)
                .HasForeignKey(ab => ab.AgentId);

            modelBuilder.Entity<AgentBank>()
                .HasOne(ab => ab.Bank)
                .WithMany(b => b.AgentBanks)
                .HasForeignKey(ab => ab.BankId);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.Agent)
                .WithOne(a => a.Company)
                .HasForeignKey<Agent>(a => a.Id);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.Bank)
                .WithOne(b => b.Company)
                .HasForeignKey<Bank>(b => b.Id);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.Client)
                .WithOne(cl => cl.Company)
                .HasForeignKey<Client>(cl => cl.Id);
        }
    }

}
