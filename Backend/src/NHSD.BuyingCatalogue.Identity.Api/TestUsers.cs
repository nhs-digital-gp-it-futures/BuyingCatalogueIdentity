// This will be replaced when a database is in

using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Test;

namespace NHSD.BuyingCatalogue.Identity.Api
{
    public sealed class TestUsers
    {
        public static readonly List<TestUser> Users = new List<TestUser>
        {
            new TestUser{SubjectId = "818727", Username = "michael", Password = "michael",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Michael Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Michael"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "MichaelSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://michael.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                    new Claim("upn", "Michael.Smith"),
                    new Claim("ods_code", "03Q")
                }
            },
            new TestUser{SubjectId = "88421113", Username = "jane", Password = "jane",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Jane Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Jane"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "JaneSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://jane.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                    new Claim("location", "somewhere"),
                    new Claim("upn", "Jane.Smith"),
                    new Claim("ods_code", "99A"),
                }
            }
        };
    }
}
