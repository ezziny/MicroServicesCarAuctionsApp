using AuctionService.Contexts;
using AuctionService.Extensions;
using AuctionService.Helpers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuctionDatabase"));
    
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(15);
        o.UsePostgres();
        o.UseBusOutbox();
    }
    );
    x.UsingRabbitMq((context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
            cfg.Host("localhost", "/", h =>
            {
                h.Username("rabbit");
                h.Password("rabbit");
            });
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();
try
{
    DbSeeder.InitDb(app);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

app.Run();
