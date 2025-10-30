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
        var count = await DB.CountAsync<Item>();
        if (count == 0) // Only seed if database is empty
        {
            using var scope = app.Services.CreateScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();
            var items = await httpClient.GetItemsForSearchDB();
            Console.WriteLine($"{items.Count} returned from the auctions service");
            if (items.Count > 0) await DB.SaveAsync(items);
        }
    }
}