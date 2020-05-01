using System.Security.Cryptography.X509Certificates;

namespace NHSD.BuyingCatalogue.Identity.Api.Certificates
{
    internal interface ICertificate
    {
        X509Certificate2 Instance { get; }

        bool IsAvailable { get; }

        string Path { get; }
    }
}
