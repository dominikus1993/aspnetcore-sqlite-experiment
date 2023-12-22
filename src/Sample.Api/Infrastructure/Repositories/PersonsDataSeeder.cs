using System.Globalization;
using System.Runtime.CompilerServices;

using CsvHelper;
using CsvHelper.Configuration;

using Sample.Api.Core.Types;

namespace Sample.Api.Infrastructure.Repositories;

public sealed class PersonsDataSeeder
{
    private static readonly CsvConfiguration CsvConfiguration = new(CultureInfo.InvariantCulture)
    {
        NewLine = Environment.NewLine,
        Delimiter = ","
    };
    
    public async IAsyncEnumerable<Person> Seed([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader("./data/persons.csv");
        using var csv = new CsvReader(reader, CsvConfiguration);
        await foreach(var person in csv.GetRecordsAsync<Person>(cancellationToken))
        {
            yield return person;
        }
    }
}