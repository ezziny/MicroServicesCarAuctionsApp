using System;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        System.Console.WriteLine($"consuming deletAuction with ID {context.Message.Id}");
        await DB.DeleteAsync<Item>(context.Message.Id);
    }
}
