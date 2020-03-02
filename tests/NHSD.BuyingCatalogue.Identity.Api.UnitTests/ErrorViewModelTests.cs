using NHSD.BuyingCatalogue.Identity.Api.ViewModels;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class ErrorViewModelTests
    {
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("\t", false)]
        [TestCase("A message", true)]
        public void ShowMessage_ReturnsExpectedValue(string message, bool expectedValue)
        {
            var viewModel = new ErrorViewModel { Message = message };

            Assert.AreEqual(expectedValue, viewModel.ShowMessage);
        }
    }
}
