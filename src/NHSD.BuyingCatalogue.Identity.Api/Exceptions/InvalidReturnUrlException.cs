using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Exceptions
{
    public sealed class InvalidReturnUrlException : Exception
    {
        internal const string DefaultMessage = "Invalid return URL.";

        public InvalidReturnUrlException()
            : this(DefaultMessage)
        {
        }

        public InvalidReturnUrlException(string message)
            : base(message)
        {
        }

        public InvalidReturnUrlException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
