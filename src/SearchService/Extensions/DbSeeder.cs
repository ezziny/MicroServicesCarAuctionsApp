using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Entities;
using SearchService.Services;

namespace SearchService.Extensions;

public class DbSeeder
{
    public static async Task SeedAsync(WebApplication app)
    {
        await DB.InitAsync("SearchDb",
            MongoClientSettings.FromConnectionString((app.Configuration.GetConnectionString("MongoDb") ??
                                                      throw new InvalidOperationException(
                                                          "MongoDB connection string not found."))));
        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();
        var count = await DB.CountAsync<Item>();
        using var scope = app.Services.CreateScope();
        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();
        var items = await httpClient.GetItemsForSearchDB();
        Console.WriteLine($"{items.Count} returned from the auctions service");
        if (items.Count > 0) await DB.SaveAsync(items); 
    }
}