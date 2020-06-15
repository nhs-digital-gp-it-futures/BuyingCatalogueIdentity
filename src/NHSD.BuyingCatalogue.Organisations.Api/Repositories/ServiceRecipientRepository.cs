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
            var retrievedAll = false;

            var costCentres = new List<ServiceRecipient>();
            int offset = 0;

            while (!retrievedAll)
            {
                var children = await _settings.ApiBaseUrl
                    .AppendPathSegment("organisations")
                    .SetQueryParam("RelTypeId", "RE4")
                    .SetQueryParam("TargetOrgId", odsCode)
                    .SetQueryParam("RelStatus", "active")
                    .SetQueryParam("Limit", _settings.GetChildOrganisationSearchLimit)
                    .SetQueryParam("Offset", offset)
                    .AllowHttpStatus("3xx,4xx")
                    .GetJsonAsync<ServiceRecipientResponse>();

                if (children.Organisations == null)
                {
                    break;
                }

                var centres = children.Organisations.Where(o => o.PrimaryRoleId == _settings.GpPracticeRoleId);
                costCentres.AddRange(centres);

                retrievedAll = children.Organisations.Count() != _settings.GetChildOrganisationSearchLimit;
                offset += _settings.GetChildOrganisationSearchLimit;
            }

            return costCentres;
        }

        internal class ServiceRecipientResponse
        {
            public IEnumerable<ServiceRecipient> Organisations { get; set; }
        }
    }
}
