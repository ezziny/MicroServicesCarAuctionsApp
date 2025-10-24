using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Entities;
using SearchService.Helpers;

namespace SearchService.Controllers;

public class SearchController: BaseController
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> Search([FromQuery] SearchParameters parameters)
    {
        var query = DB.PagedSearch<Item, Item>();
        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            query.Match(MongoDB.Entities.Search.Full, parameters.SearchTerm).SortByTextScore();
        }

        query = parameters.FilterBy switch
        {
            "Finished"   => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "EndingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
            _            => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };

        query = parameters.OrderBy switch
        {
            "Make"    => query.Sort(x => x.Ascending(a => a.Make)),
            "Color"   => query.Sort(x => x.Ascending(a => a.Color)),
            "Mileage" => query.Sort(x => x.Ascending(a => a.Mileage)),
            "Model"   => query.Sort(x => x.Ascending(a => a.Model)),
            "New"     => query.Sort(x => x.Descending(a => a.CreatedAt)),
            _         => query.Sort(x => x.Ascending(a => a.SoldAmount))
        };


        if (!string.IsNullOrEmpty(parameters.Seller))
        {
            query.Match(x => x.Seller == parameters.Seller);
        }
        if (!string.IsNullOrEmpty(parameters.Winner))
        {
            query.Match(x => x.Winner == parameters.Winner);
        }

        query.PageNumber(parameters.PageNumber); query.PageSize(parameters.PageSize);
        var result = await query.ExecuteAsync();
        return Ok(new { results = result.Results, pageCount = result.PageCount, totalCount = result.TotalCount });
    }
}