using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SummaryRedirectAttribute : Attribute
    {
        public string Link { get; set; }
    }
}
