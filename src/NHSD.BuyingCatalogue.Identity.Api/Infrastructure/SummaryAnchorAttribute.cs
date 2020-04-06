using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SummaryAnchorAttribute : Attribute
    {
        public string Link { get; set; }
    }
}
