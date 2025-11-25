using System;
using MongoDB.Entities;

namespace BiddingService.Models;

public class Bid: Entity
{
    public int Amount { get; set; }
    public string AuctionId { get; set; }
    public DateTime BidTime { get; set; } = DateTime.Now;
    public string Bidder { get; set; }
    public BidStatus BidStatus { get; set; }
}
