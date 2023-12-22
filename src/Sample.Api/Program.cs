using Sample.Api.Infrastructure.EntityFramework;
using Sample.Api.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/persons", (SampleDbContext dbContext) => dbContext.Persons.AsAsyncEnumerable())
.WithName("GetPersons")
.WithOpenApi();

app.MapGet("/persons/{personId:guid}", (Guid personId, SampleDbContext dbContext, CancellationToken cancellationToken) => dbContext.Persons.FindAsync([personId], cancellationToken))
    .WithName("GetPersonById")
    .WithOpenApi();


await app.RunAsync();
