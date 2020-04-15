using System;

namespace NHSD.BuyingCatalogue.Identity.Common.ViewModels.Messages
{
    public sealed class ErrorMessageViewModel
    {
        public string Id { get; }

        public string? Field { get; }

        public ErrorMessageViewModel(string id, string? field = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }
    }
}
