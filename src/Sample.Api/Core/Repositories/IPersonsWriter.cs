using Sample.Api.Core.Types;

namespace Sample.Api.Core.Repositories;

public interface IPersonsWriter
{
    Task AddMany(IAsyncEnumerable<Person> persons, CancellationToken cancellationToken = default);
}