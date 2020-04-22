using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Identity.Api.Repositories
{
    public interface IScopeRepository
    {
        IReadOnlyCollection<string> Scopes { get; }
    }
}
