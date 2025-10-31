using System;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    private readonly IMapper _mapper;

    public AuctionUpdatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        System.Console.WriteLine($"Consuming AcutionUpdated with Guid: {context.Message.Id}");
        var item = _mapper.Map<Item>(context.Message);
        await DB.Update<Item>().
        MatchID(item.ID).
        ModifyOnly(i => new {i.Color,i.Mileage,i.Make,i.Year, i.Model}, item).
        ExecuteAsync();
    }
}
