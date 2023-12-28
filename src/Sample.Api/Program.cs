using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Sample.Api.Core.Types;
using Sample.Api.Infrastructure.EntityFramework;
using Sample.Api.Infrastructure.Extensions;
using Sample.Api.Services;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
  options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddHostedService<SimplePeriodicBackgroundService>();
builder.AddInfrastructure();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.SetupDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/persons", ([FromServices]SampleDbContext dbContext, int take = 10) => dbContext.Persons.OrderBy(x => x.Id).Take(take).AsAsyncEnumerable())
.WithName("GetPersons")
.WithOpenApi();

app.MapGet("/persons/{personId:guid}",
        async (PersonId personId, SampleDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var person = await dbContext.GetPerson(personId, cancellationToken);
            return person is null ? Results.NotFound() : Results.Ok(person);
        })
    .WithName("GetPersonById")
    .WithOpenApi();


await app.RunAsync();

[JsonSerializable(typeof(IAsyncEnumerable<Person>))]
[JsonSerializable(typeof(Person))]
internal sealed partial class AppJsonSerializerContext : JsonSerializerContext;


internal partial class Program;