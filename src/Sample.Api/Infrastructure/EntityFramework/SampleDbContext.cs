using Microsoft.EntityFrameworkCore;

using Sample.Api.Core.Types;

namespace Sample.Api.Infrastructure.EntityFramework;

public sealed class SampleDbContext(DbContextOptions<SampleDbContext> options) : DbContext(options)
{
    
    private static readonly Func<SampleDbContext, PersonId, CancellationToken,Task<Person?>> GetPersonQ =
        EF.CompileAsyncQuery(
            (SampleDbContext dbContext, PersonId id, CancellationToken cancellationToken) =>
                dbContext.Persons.FirstOrDefault(n => n.Id == id));
    
    private static readonly Func<SampleDbContext, CancellationToken,Task<int>> CountPersonsQ =
        EF.CompileAsyncQuery(
            (SampleDbContext dbContext, CancellationToken cancellationToken) =>
                dbContext.Persons.Count());
    public DbSet<Person> Persons { get; set; } = null!;

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

    public Task<Person?> GetPerson(PersonId id, CancellationToken cancellationToken) =>
        GetPersonQ(this, id, cancellationToken);

    public Task<int> CountPersons(CancellationToken cancellationToken) => CountPersonsQ(this, cancellationToken);
}