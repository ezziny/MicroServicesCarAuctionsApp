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
builder.Services.AddCors(o =>
{
    o.AddPolicy("customPolicy", p =>
    {
        p.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["nextApplication"]);
    });
});
var app = builder.Build();
app.UseCors();
app.MapReverseProxy();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
