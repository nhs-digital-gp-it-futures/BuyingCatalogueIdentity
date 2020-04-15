using System.Collections.Generic;
using NHSD.BuyingCatalogue.Identity.Common.ViewModels.Messages;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Users
{
    public sealed class CreateBuyerResponseViewModel
    {
        public string UserId { get; set; }

        public IEnumerable<ErrorMessageViewModel> Errors { get; set; }
    }
}
