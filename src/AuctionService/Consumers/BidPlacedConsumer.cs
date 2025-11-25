using System;
using AuctionService.Contexts;
using AuctionService.Entities;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AuctionService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContext _context;

    public BidPlacedConsumer(AuctionDbContext context)
    {
        _context = context;
    }
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        var auction = await _context.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));
        if (auction.CurrentHighBid == null || context.Message.Amount > auction.CurrentHighBid
            && context.Message.BidStatus.Contains("accepted", StringComparison.OrdinalIgnoreCase)
        )
        {
            auction.CurrentHighBid = context.Message.Amount;
            await _context.SaveChangesAsync();
        }
    }
}
