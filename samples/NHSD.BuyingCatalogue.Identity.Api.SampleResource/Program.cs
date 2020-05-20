using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

namespace NHSD.BuyingCatalogue.Identity.Api.SampleResource
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IdentityModelEventSource.ShowPII = true;
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
