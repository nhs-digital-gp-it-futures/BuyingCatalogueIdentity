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
        private readonly OdsSettings settings;

        public ServiceRecipientRepository(OdsSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<IEnumerable<ServiceRecipient>> GetServiceRecipientsByParentOdsCode(string odsCode)
        {
            var retrievedAll = false;

            var costCentres = new List<ServiceRecipient>();
            int offset = 0;
            int searchLimit = settings.GetChildOrganisationSearchLimit;

            while (!retrievedAll)
            {
                var query = settings.ApiBaseUrl
                    .AppendPathSegment("organisations")
                    .SetQueryParam("RelTypeId", "RE4")
                    .SetQueryParam("TargetOrgId", odsCode)
                    .SetQueryParam("RelStatus", "active")
                    .SetQueryParam("Limit", searchLimit)
                    .AllowHttpStatus("3xx,4xx");

                if (offset > 0)
                {
                    query.SetQueryParam("Offset", offset);
                }

                var serviceRecipientResponse = await query.GetJsonAsync<ServiceRecipientResponse>();

                if (serviceRecipientResponse.Organisations is null)
                {
                    break;
                }

                var centres = serviceRecipientResponse.Organisations.Where(o => o.PrimaryRoleId == settings.GpPracticeRoleId);
                costCentres.AddRange(centres);

                retrievedAll = serviceRecipientResponse.Organisations.Count() != searchLimit;
                offset += searchLimit;
            }

            return costCentres;
        }

        internal sealed class ServiceRecipientResponse
        {
            public IEnumerable<ServiceRecipient> Organisations { get; set; }
        }
    }
}
