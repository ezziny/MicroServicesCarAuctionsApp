using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Entities;
using SearchService.Extensions;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionServiceHttpClient>();
var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     // app.MapOpenApi();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
try
{
    await DbSeeder.SeedAsync(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
app.Run();
