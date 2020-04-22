using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.Validators;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Validators
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OrganisationValidatorTests
    {
        [Test]
        public void Constructor_NullRepository_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new OrganisationValidator(null);
            });
        }

        [Test]
        public async Task Validate_Organisation_Does_Not_Exist_Returns_Success()
        {
            var newOrganisation = OrganisationBuilder.Create(1).Build();

            var mockOrganisationRepository = SetUpGetByNameAsync(null);

            var validator = new OrganisationValidator(mockOrganisationRepository);

            var result = await validator.ValidateAsync(newOrganisation);

            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task Validate_Organisation_Does_Exist_Returns_Failure()
        {
            var existingOrganisation = OrganisationBuilder.Create(1).Build();

            var mockOrganisationRepository = SetUpGetByNameAsync(existingOrganisation);

            var validator = new OrganisationValidator(mockOrganisationRepository);

            var result = await validator.ValidateAsync(existingOrganisation);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.FirstOrDefault()?.Id.Should().BeEquivalentTo("OrganisationAlreadyExists");
        }

        [Test]
        public async Task Validate_Calls_OrganisationRepository_Once()
        {
            var newOrganisation = OrganisationBuilder.Create(1).Build();

            var mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(newOrganisation);

            var validator = new OrganisationValidator(mockOrganisationRepository.Object);

            await validator.ValidateAsync(newOrganisation);

            mockOrganisationRepository.Verify(r => r.GetByNameAsync(newOrganisation.Name), Times.Once);
        }

        [Test]
        public void Validate_NullOrganisation_Throws()
        {
            var mockOrganisationRepository = SetUpGetByNameAsync(null);

            var validator = new OrganisationValidator(mockOrganisationRepository);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await validator.ValidateAsync(null));
        }

        private static IOrganisationRepository SetUpGetByNameAsync(Organisation organisationToReturn)
        {
            var mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(organisationToReturn);

            return mockOrganisationRepository.Object;
        }
    }
}
