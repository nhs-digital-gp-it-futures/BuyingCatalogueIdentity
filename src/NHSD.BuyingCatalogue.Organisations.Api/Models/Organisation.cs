using System;
using Newtonsoft.Json;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
{
    public sealed class Organisation
    {
        private Address _location;

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OdsCode { get; set; }

        public string PrimaryRoleId { get; set; }

        public string Address { get; set; }

        public bool CatalogueAgreementSigned { get; set; }

        public DateTime LastUpdated { get; set; }

        public Address Location
        {
            get
            {
                if (Address != null)
                {
                    _location = JsonConvert.DeserializeObject<Address>(Address);
                }
                    
                return _location;
            }
        }
    }
}
