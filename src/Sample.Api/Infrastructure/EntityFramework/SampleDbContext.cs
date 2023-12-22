using Microsoft.EntityFrameworkCore;

using Sample.Api.Core.Types;

namespace Sample.Api.Infrastructure.EntityFramework;

public sealed class SampleDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; } = null!;
    
    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Name).HasMaxLength(200);
        });
        base.OnModelCreating(modelBuilder);
    }
}