using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Services;

public class AuctionServiceHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AuctionServiceHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<Item>> GetItemsForSearchDB()
    {
        var lastUpdated = await DB.Find<Item, String>()
            .Sort(x => x.Descending(a => a.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString())
            .ExecuteFirstAsync();
        return await _httpClient.GetFromJsonAsync<List<Item>>(_configuration["AuctionServiceUrl"] +
                                                               $"/api/auctions?date={lastUpdated}");
    }
}