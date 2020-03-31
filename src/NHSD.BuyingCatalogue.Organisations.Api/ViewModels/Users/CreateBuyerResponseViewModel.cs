using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Messages;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Users
{
    public sealed class CreateBuyerResponseViewModel
    {
        public IEnumerable<ErrorViewModel> Errors { get; set; }
    }
}
