using System.Threading.Tasks;
using Moq;
using NHSD.BuyingCatalogue.EmailClient;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.SharedMocks
{
    internal sealed class MockEmailService : IEmailService
    {
        private readonly IEmailService _service;

        internal MockEmailService()
        {
            var mockService = new Mock<IEmailService>();
            mockService.Setup(s => s.SendEmailAsync(It.IsNotNull<EmailMessage>()))
                .Callback<EmailMessage>(m => SentMessage = m);

            _service = mockService.Object;
        }

        internal EmailMessage SentMessage { get; private set; }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            await _service.SendEmailAsync(emailMessage);
        }
    }
}
