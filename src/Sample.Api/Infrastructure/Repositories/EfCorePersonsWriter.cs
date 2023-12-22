using Microsoft.EntityFrameworkCore;

using Sample.Api.Core.Repositories;
using Sample.Api.Core.Types;
using Sample.Api.Infrastructure.EntityFramework;

namespace Sample.Api.Infrastructure.Repositories;

public class EfCorePersonsWriter : IPersonsWriter
{
    private readonly SampleDbContext _sampleDbContext;

    public EfCorePersonsWriter(SampleDbContext sampleDbContext)
    {
        _sampleDbContext = sampleDbContext;
    }

    public async Task AddMany(IAsyncEnumerable<Person> persons, CancellationToken cancellationToken = default)
    {
        var personsList = await persons.ToListAsync(cancellationToken: cancellationToken);
        if (personsList is {Count: >0})
        {
            await _sampleDbContext.Persons
                .UpsertRange(personsList)
                    .On(p => p.Id)
                    .RunAsync(cancellationToken);
        }
    }
}