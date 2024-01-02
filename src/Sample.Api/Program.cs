using System.Net;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;

using Polly;
using Polly.Timeout;

using Sample.Api.Core.Types;
using Sample.Api.Infrastructure.EntityFramework;
using Sample.Api.Infrastructure.Extensions;
using Sample.Api.Infrastructure.Repositories;
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

builder.Services.AddHttpClient<TestHttpClient>()
    .AddResilienceHandler(nameof(TestHttpClient), b =>
    {
        b.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 5,
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<TimeoutRejectedException>()
                .Handle<HttpRequestException>()
                .HandleResult(response => response.StatusCode == HttpStatusCode.InternalServerError),
            Delay = TimeSpan.FromSeconds(2),
            BackoffType = DelayBackoffType.Exponential
        });
                
        b.AddTimeout(TimeSpan.FromSeconds(5));
    });

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

app.MapGet("/ping", (TestHttpClient testHttpClient, CancellationToken cancellationToken) => testHttpClient.Get(cancellationToken))
    .WithName("Ping")
    .WithOpenApi();
await app.RunAsync();

[JsonSerializable(typeof(IAsyncEnumerable<Person>))]
[JsonSerializable(typeof(Person))]
internal sealed partial class AppJsonSerializerContext : JsonSerializerContext;


internal partial class Program;