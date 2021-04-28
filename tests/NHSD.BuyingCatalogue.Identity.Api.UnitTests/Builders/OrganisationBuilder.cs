using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class OrganisationBuilder
    {
        private readonly Guid organisationId;
        private readonly string name;
        private IEnumerable<Organisation> relatedOrganisations;

        private OrganisationBuilder()
        {
            organisationId = Guid.NewGuid();
            name = "Primary HealthTrust";
        }

        public Organisation Build()
        {
            Organisation toBuild = new() { Name = name, OrganisationId = organisationId };

            if (relatedOrganisations is not null && relatedOrganisations.Any())
                relatedOrganisations.ToList().ForEach(ro => toBuild.RelatedOrganisations.Add(ro));

            return toBuild;
        }

        internal static OrganisationBuilder Create() => new();

        internal OrganisationBuilder WithRelatedOrganisations(List<Guid> relatedOrganisations)
        {
            this.relatedOrganisations = relatedOrganisations
                .Select((value, index) => new { value, Name = string.Format(CultureInfo.CurrentCulture, "HealthTrust {0}", index) })
                .Select(ro => new Organisation { OrganisationId = ro.value, Name = ro.Name });

            return this;
        }
    }
}
