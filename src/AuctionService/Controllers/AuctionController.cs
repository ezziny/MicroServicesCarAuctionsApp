using AuctionService.Contexts;
using AuctionService.Dtos;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

public class AuctionsController: BaseController
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions()
    {
        var auctions = await _context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();
        return _mapper.Map<List<AuctionDto>>(auctions);
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

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto creationData)
    {
        var auction = _mapper.Map<Auction>(creationData);
        // TODO: add current user as seller when you've implemented identity
        auction.Seller = "test";
        await _context.Auctions.AddAsync(auction);
        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return BadRequest(new {error = "Couldn't Save Changes To The Database"});
        return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, _mapper.Map<AuctionDto>(auction));
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateData)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null) return NotFound();
        // TODO : check if current user is the seller when you've implemented identity 
        // return Unauthorized();
        // no need for mapping 
        auction.Item.Make = updateData.Make ?? auction.Item.Make;
        auction.Item.Model = updateData.Model ?? auction.Item.Model;
        auction.Item.Year = updateData.Year ?? auction.Item.Year;
        auction.Item.Color = updateData.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateData.Mileage ?? auction.Item.Mileage;
        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return BadRequest(new {error = "Couldn't Save Changes To The Database"});
        return Ok();
    }
    //usuallly you don't delete auctions but for the sake of the example i will implement it lmao
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        if (auction == null) return NotFound();
        _context.Auctions.Remove(auction);
        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return BadRequest(new {error = "Couldn't Delete Auction From The Database"});
        return NoContent();
    }

}