using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("cardanobi-core-read", "CardanoBI Core API Read"),
            new ApiScope("cardanobi-core-stream", "CardanoBI Core API Stream"),
            new ApiScope("cardanobi-bi-read", "CardanoBI BI API Read"),
            new ApiScope("cardanobi-bi-stream", "CardanoBI BI API Stream")
        };

    public static IEnumerable<Client> Clients =>
       new List<Client>
       {
            new Client
            {
                ClientId = "client",

                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                // scopes that client has access to
                AllowedScopes = { "cardanobi-core-read", "cardanobi-bi-read" }
            },
            new Client
            {
                ClientId = "client2",

                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets =
                {
                    new Secret("secret2".Sha256())
                },

                // scopes that client has access to
                AllowedScopes = { "cardanobi-core-read" }
            }
       };
}