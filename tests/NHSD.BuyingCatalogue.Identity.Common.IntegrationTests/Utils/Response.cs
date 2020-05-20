using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils
{
    internal sealed class Response
    {
        public HttpResponseMessage Result { get; set; }

        public async Task<JToken> ReadBodyAsJsonAsync() => JToken.Parse(await Result.Content.ReadAsStringAsync());
    }
}
