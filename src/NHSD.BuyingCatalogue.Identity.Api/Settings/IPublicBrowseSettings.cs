namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    public interface IPublicBrowseSettings
    {
        string BaseAddress { get; }

        string LoginPath { get; }
    }
}
