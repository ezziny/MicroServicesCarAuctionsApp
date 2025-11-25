using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("auctionApp", "Auction app full access"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "insomnia",
                ClientName = "Insomnia",
                AllowedScopes = {"openid", "profile", "auctionApp"},
                RedirectUris = {"https://chatgpt.com"},
                ClientSecrets = new [] {new Secret("someSecretjey7783y".Sha256())}, //TODO : switch to .env when things are serious
                AllowedGrantTypes = {GrantType.ResourceOwnerPassword},
            },
            new Client
            {
                ClientId = "nextApplication",
                ClientName = "nextApplication",
                ClientSecrets = {new Secret("nextSecret".Sha256())}, //TODO : switch to .env when things are serious
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                RequirePkce = false,
                RedirectUris = {"http://localhost:3000/api/auth/callback/id-server" }, //??
                AllowOfflineAccess = true,
                AllowedScopes = {"openid","profile","auctionApp"},
                AccessTokenLifetime = 3600*24*30,
                AlwaysIncludeUserClaimsInIdToken = true
            }
        };
}
