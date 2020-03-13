using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common
{
    public sealed class Response
    {
        public HttpResponseMessage Result { get; set; }

        public async Task<JToken> ReadBody() => JToken.Parse(await Result.Content.ReadAsStringAsync());
    }
}
