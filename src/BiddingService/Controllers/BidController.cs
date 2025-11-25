using AutoMapper;
using BiddingService.Dtos;
using BiddingService.Models;
using Contracts;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BiddingService.Controllers;
public class BidController : BaseController
{
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public BidController(IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BidDto>> PlaceBid(string auctionId, int amount)
    {
        var auction = await DB.Find<Auction>().OneAsync(auctionId);
        if (auction == null)
        // TODO : check if AuctionService is reachable and fetch auction details if not found in local DB
            return NotFound("Auction not found.");
        if (auction.Seller == User.Identity.Name)
            return BadRequest("You cannot Bid on your own Auction");
        var bid = new Bid
        {
          Amount = amount,
          AuctionId = auctionId,
          Bidder = User.Identity.Name  
        };
        if (auction.AuctionEnd < DateTime.UtcNow)
        {
            bid.BidStatus = BidStatus.Finished;
        }
        else{
            var highestBid = await DB.Find<Bid>().Match(a => a.AuctionId == auctionId).Sort(b => b.Descending(a => a.Amount)).ExecuteFirstAsync();
            if (highestBid is null || (highestBid is not null && amount > highestBid.Amount))
            {
                bid.BidStatus = amount > auction.ReservePrice ? BidStatus.Accepted : BidStatus.AcceptedBelowReserve;
            }
            if (highestBid is not null && bid.Amount <= highestBid.Amount)
            {
                bid.BidStatus = BidStatus.TooLow;
            }
        }
        await DB.SaveAsync(bid);
        await _publishEndpoint.Publish(_mapper.Map<BidPlaced>(bid));
        return Ok(_mapper.Map<BidDto>(bid));
    }
    [HttpGet("{auctionId}")]
    public async Task<ActionResult<List<BidDto>>> GetAuctionBids(string auctionId)
    {
        var bids = await DB.Find<Bid>().Match(b => b.AuctionId == auctionId).Sort(s =>s.Descending(x => x.BidTime)).ExecuteAsync();
        return bids.Select(_mapper.Map<BidDto>).ToList();
    }
}

