using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Repositories
{
    public interface IOdsRepository
    {
        Task<OdsOrganisation> GetBuyerOrganisationByOdsCode(string odsCode);
    }
}
