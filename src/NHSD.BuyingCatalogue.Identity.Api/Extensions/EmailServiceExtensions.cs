using System.Threading.Tasks;
using NHSD.BuyingCatalogue.EmailClient;

namespace NHSD.BuyingCatalogue.Identity.Api.Extensions
{
    internal static class EmailServiceExtensions
    {
        internal static async Task SendEmailAsync(
            this IEmailService service,
            EmailMessageTemplate messageTemplate,
            EmailAddress recipient,
            params object[] formatItems)
        {
            await service.SendEmailAsync(new EmailMessage(messageTemplate, new[] { recipient }, null, formatItems));
        }
    }
}
