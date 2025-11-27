using System;
using AuctionService.Contexts;
using AuctionService.Entities;
using AuctionService.Protos;
using Grpc.Core;

namespace AuctionService.Services;

public class GrpcAuctionService : Protos.AuctionService.AuctionServiceBase
{
    private readonly AuctionDbContext _dbcontext;

    public GrpcAuctionService(AuctionDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    public override async Task<AuctionResponse> GetAuction(GetAuctionRequest request, ServerCallContext context)
    {
        System.Console.WriteLine("grpc request for auctigon");
        var auction = await _dbcontext.FindAsync<Auction>(Guid.Parse(request.Id));
        if (auction == null) throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, "Auction not found"));
        var response = new AuctionResponse
        {
            Auction = new AuctionModel
            {
                AuctionEnd = auction.AuctionEnd.ToString(),
                Id = auction.Id.ToString(),
                ReservePrice = auction.ReservePrice,
                Seller = auction.Seller
            }
        };
        return response;
    }
}
