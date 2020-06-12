using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;

namespace NHSD.BuyingCatalogue.Organisations.Api.Repositories
{
    internal sealed class ServiceRecipientRepository : IServiceRecipientRepository
    {
        private readonly OdsSettings _settings;

        public ServiceRecipientRepository(OdsSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<IEnumerable<ServiceRecipient>> GetServiceRecipientsByParentOdsCode(string odsCode)
        {
            var children = await _settings.ApiBaseUrl
                .AppendPathSegment("organisations")
                .SetQueryParam("RelTypeId", "RE4")
                .SetQueryParam("TargetOrgId", odsCode)
                .SetQueryParam("RelStatus", "active")
                .SetQueryParam("Limit", 1000)
                .AllowHttpStatus("3xx,4xx")
                .GetJsonAsync<Children>();

            const string prescribingCostCentre = "RO177";

            var prescribingCostCentres = children.Organisations.Where(o => o.PrimaryRoleId == prescribingCostCentre);
            return prescribingCostCentres;
        }

        internal class Children
        {
            public IEnumerable<ServiceRecipient> Organisations { get; set; }
        }
    }
}
