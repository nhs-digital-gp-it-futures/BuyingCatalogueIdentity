using NHSD.BuyingCatalogue.Identity.Common.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
{
    public static class OrganisationErrors
    {
        public static ErrorDetails OrganisationAlreadyExists()
        {
            return new ErrorDetails("OrganisationAlreadyExists");
        }
    }
}
