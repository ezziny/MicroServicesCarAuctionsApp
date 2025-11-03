using AuctionService.Contexts;
using AuctionService.Dtos;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

public class AuctionsController: BaseController
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuctionsController(
        AuctionDbContext context,
        IMapper mapper,
        IPublishEndpoint publishEndpoint
        )
    {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions(string? date)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();
        if (!string.IsNullOrEmpty(date))
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime())>0);
        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null) return NotFound();
        return _mapper.Map<AuctionDto>(auction);
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto creationData)
    {
        var auction = _mapper.Map<Auction>(creationData);
        // TODO: add current user as seller when you've implemented identity
        auction.Seller = User.Identity.Name;
        await _context.Auctions.AddAsync(auction);
        var publishAuction = _mapper.Map<AuctionDto>(auction);
        await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(publishAuction));
        var result = await _context.SaveChangesAsync() > 0; //it's fine to leave the SaveChangesAsync after the publish because it won't publish anyways if you don't save changes as we have outbox remember?
        if (!result) return BadRequest(new { error = "Couldn't Save Changes To The Database" });
        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, publishAuction);
    }
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateData)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null) return NotFound();
        // TODO : check if current user is the seller when you've implemented identity 
        if (auction.Seller != User.Identity.Name) return Forbid();
        // no need for mapping 
        auction.Item.Make = updateData.Make ?? auction.Item.Make;
        auction.Item.Model = updateData.Model ?? auction.Item.Model;
        auction.Item.Year = updateData.Year ?? auction.Item.Year;
        auction.Item.Color = updateData.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateData.Mileage ?? auction.Item.Mileage;
        await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));
        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return BadRequest(new { error = "Couldn't Save Changes To The Database" });
        return Ok();
    }
    //usuallly you don't delete auctions but for the sake of the example i will implement it lmao
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        if (auction == null) return NotFound();
        if (auction.Seller != User.Identity.Name) return Forbid();
        _context.Auctions.Remove(auction);
        await _publishEndpoint.Publish(new AuctionDeleted { Id = id.ToString() });
        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return BadRequest(new {error = "Couldn't Delete Auction From The Database"});
        return Ok();
    }

}