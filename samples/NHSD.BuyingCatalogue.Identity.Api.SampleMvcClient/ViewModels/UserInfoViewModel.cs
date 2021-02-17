using System.Collections.Generic;
using System.Security.Claims;

namespace NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.ViewModels
{
    public sealed class UserInfoViewModel
    {
        public IEnumerable<Claim> UserClaims { get; set; } = new List<Claim>();

        public string AccessToken { get; set; }
    }
}
