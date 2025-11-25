using BiddingService.Consumers;
using BiddingService.Models;
using BiddingService.Services;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Driver;
using MongoDB.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bids", false));
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
            cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
            {
                h.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
                h.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
            });
        });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(o =>
{
    o.Authority = builder.Configuration["IdentityServiceUrl"];
    o.RequireHttpsMetadata = false;
    o.TokenValidationParameters.ValidateAudience = false;
    o.TokenValidationParameters.NameClaimType = "username";
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHostedService<CheckAuctionFinished>();
var app = builder.Build();


app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await DB.InitAsync("BidDb",
    MongoClientSettings.FromConnectionString(
        builder.Configuration.GetConnectionString("MongoDb") ??
        throw new InvalidOperationException("MongoDB connection string not found.")));
await DB.Index<Bid>()
    .Key(b => b.AuctionId, KeyType.Ascending)
    .Key(b => b.Amount, KeyType.Descending)
    .CreateAsync();

app.Run();
