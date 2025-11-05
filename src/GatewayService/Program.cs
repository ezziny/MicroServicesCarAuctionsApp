using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(o =>
{
    o.Authority = builder.Configuration["IdentityServiceUrl"];
    o.RequireHttpsMetadata = false;
    o.TokenValidationParameters.ValidateAudience = false;
    o.TokenValidationParameters.NameClaimType = "username";
});

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
