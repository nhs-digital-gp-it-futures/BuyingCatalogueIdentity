using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Identity.Api.Repositories
{
    public interface IScopeRepository
    {
        IEnumerable<string> Scopes { get; }
    }
}
