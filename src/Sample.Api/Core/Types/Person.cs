namespace Sample.Api.Core.Types;

public sealed class Person
{
    public PersonId Id { get; set; }
    public string Name { get; set; } = null!;
}