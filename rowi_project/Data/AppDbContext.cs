using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rowi_project.Models.Entities;

namespace rowi_project.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<Bank> Banks => Set<Bank>();
    public DbSet<Client> Clients => Set<Client>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new BankConfiguration());
        modelBuilder.ApplyConfiguration(new AgentConfiguration());
        modelBuilder.ApplyConfiguration(new ClientConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.Id);
    }
}

public class BankConfiguration : IEntityTypeConfiguration<Bank>
{
    public void Configure(EntityTypeBuilder<Bank> builder)
    {
        builder.HasKey(b => b.Id);

        // 1:1 Bank:Company
        builder.HasOne(b => b.Company)
            .WithOne(c => c.Bank)
            .HasForeignKey<Bank>(b => b.Id);
    }
}

public class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.HasKey(a => a.Id);

        // 1:1 Agent:Company
        builder.HasOne(a => a.Company)
            .WithOne(c => c.Agent)
            .HasForeignKey<Agent>(a => a.Id);

        // M:M Agent:Bank
        builder.HasMany(a => a.Banks)
            .WithMany(b => b.Agents);
    }
}

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(cl => cl.Id);

        // 1:1 Client:Company
        builder.HasOne(cl => cl.Company)
            .WithOne(c => c.Client)
            .HasForeignKey<Client>(cl => cl.Id);
    }
}