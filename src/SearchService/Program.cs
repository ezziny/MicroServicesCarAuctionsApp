using System.Net;
using MongoDB.Driver;
using MongoDB.Entities;using Polly;
using Polly.Extensions.Http;
using SearchService.Entities;
using SearchService.Extensions;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
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
