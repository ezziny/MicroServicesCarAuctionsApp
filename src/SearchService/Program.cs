using System.Net;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;using Polly;
using Polly.Extensions.Http;
using SearchService.Entities;
using SearchService.Extensions;
using SearchService.Services;
using AutoMapper;
using Contracts;
using SearchService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("rabbit");
            h.Password("rabbit");
        });
        cfg.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
await DB.InitAsync("SearchDb",
    MongoClientSettings.FromConnectionString(
        builder.Configuration.GetConnectionString("MongoDb") ??
        throw new InvalidOperationException("MongoDB connection string not found.")));

await DB.Index<Item>()
    .Key(x => x.Make, KeyType.Text)
    .Key(x => x.Model, KeyType.Text)
    .Key(x => x.Color, KeyType.Text)
    .CreateAsync();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     // app.MapOpenApi();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbSeeder.SeedAsync(app);
    }
    catch (Exception e)
    {
        Console.WriteLine("Failed to seed database: " + e.Message);
        Console.WriteLine("The application will continue running without seeded data.");
    }
});
app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy() => HttpPolicyExtensions.HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(5));
