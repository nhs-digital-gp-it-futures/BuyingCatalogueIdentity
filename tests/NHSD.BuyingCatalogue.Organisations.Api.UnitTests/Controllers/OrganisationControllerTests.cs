using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Identity.Common.Extensions;
using NHSD.BuyingCatalogue.Identity.Common.ViewModels.Messages;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Services;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrganisationControllerTests
    {
        private static readonly Address Address1 = AddressBuilder.Create().WithLine1("18 Stone Road").Build();

        [Test]
        public static void Constructor_NullOrganisationRepository_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = new OrganisationsController(null, Mock.Of<ICreateOrganisationService>(), Mock.Of<IServiceRecipientRepository>());
            });
        }

        [Test]
        public static void Constructor_NullOrganisationService_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = new OrganisationsController(Mock.Of<IOrganisationRepository>(), null, Mock.Of<IServiceRecipientRepository>());
            });
        }

        [Test]
        public static void Constructor_NullServiceRecipientRepository_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = new OrganisationsController(Mock.Of<IOrganisationRepository>(), Mock.Of<ICreateOrganisationService>(), null);
            });
        }

        [Test]
        public static async Task GetAllAsync_NoOrganisationsExist_EmptyResultIsReturned()
        {
            var controller = OrganisationControllerBuilder
                .Create()
                .WithListOrganisation(new List<Organisation>())
                .Build();

            var result = await controller.GetAllAsync();

            result.Should().BeOfType<OkObjectResult>();
            result.Should().NotBe(null);

            result.As<OkObjectResult>().Value.Should().BeOfType<GetAllOrganisationsModel>();
            result.As<OkObjectResult>().Value.As<GetAllOrganisationsModel>().Organisations.Count().Should().Be(0);
        }

        [TestCase(null, null, null, false, false)]
        [TestCase("Organisation One", null, null, false, false)]
        [TestCase(null, "ODS 1", null, false, false)]
        [TestCase("Organisation One", "ODS 1", null, false, false)]
        [TestCase("Organisation One", "ODS 1", null, true, false)]
        [TestCase("Organisation One", "ODS 1", null, true, true)]
        public static async Task GetAllAsync_SingleOrganisationExists_ReturnsTheOrganisation(string name, string ods, string primaryRoleId, bool catalogueAgreementSigned, bool hasAddress)
        {
            var controller = OrganisationControllerBuilder
                .Create()
                .WithListOrganisation(new List<Organisation>
                {
                    OrganisationBuilder.Create(1)
                        .WithName(name)
                        .WithOdsCode(ods)
                        .WithPrimaryRoleId(primaryRoleId)
                        .WithCatalogueAgreementSigned(catalogueAgreementSigned)
                        .WithAddress(hasAddress == false ? null : Address1)
                        .Build(),
                })
                .Build();

            var result = await controller.GetAllAsync();

            result.Should().BeOfType<OkObjectResult>();
            result.Should().NotBe(null);
            result.As<OkObjectResult>().Value.Should().BeOfType<GetAllOrganisationsModel>();

            var organisationResult = (GetAllOrganisationsModel)result.As<OkObjectResult>().Value;

            organisationResult.Organisations.Count().Should().Be(1);

            var organisationList = organisationResult.Organisations.ToList();
            organisationList[0].Name.Should().Be(name);
            organisationList[0].OdsCode.Should().Be(ods);
            organisationList[0].PrimaryRoleId.Should().Be(primaryRoleId);
            organisationList[0].CatalogueAgreementSigned.Should().Be(catalogueAgreementSigned);

            if (hasAddress)
            {
                organisationList[0].Address.Should().BeEquivalentTo(Address1);
            }
            else
            {
                organisationList[0].Address.Should().BeNull();
            }
        }

        [Test]
        public static async Task GetAllAsync_ListOfOrganisationsExist_ReturnsTheOrganisations()
        {
            Address address = AddressBuilder.Create().WithLine1("2 City Close").Build();

            var org1 = OrganisationBuilder.Create(1).WithCatalogueAgreementSigned(false).WithAddress(Address1).Build();

            var org2 = OrganisationBuilder.Create(2).WithAddress(address).Build();

            var org3 = OrganisationBuilder.Create(3).Build();

            var controller = OrganisationControllerBuilder
                .Create()
                .WithListOrganisation(new List<Organisation>
                    { org1, org2, org3 })
                .Build();

            var result = await controller.GetAllAsync();

            result.Should().BeOfType<OkObjectResult>();
            result.Should().NotBe(null);
            result.As<OkObjectResult>().Value.Should().BeOfType<GetAllOrganisationsModel>();

            var organisationResult = (GetAllOrganisationsModel)result.As<OkObjectResult>().Value;

            organisationResult.Organisations.Count().Should().Be(3);

            var organisationList = organisationResult.Organisations.ToList();

            organisationList[0].Should().BeEquivalentTo(
                org1,
                config => config
                    .Excluding(o => o.LastUpdated)
                    .Excluding(o => o.RelatedOrganisations)
                    .Excluding(o => o.ParentRelatedOrganisations));

            organisationList[1].Should().BeEquivalentTo(
                org2,
                config => config
                    .Excluding(o => o.LastUpdated)
                    .Excluding(o => o.RelatedOrganisations)
                    .Excluding(o => o.ParentRelatedOrganisations));

            organisationList[2].Should().BeEquivalentTo(
                org3,
                config => config
                    .Excluding(o => o.LastUpdated)
                    .Excluding(o => o.RelatedOrganisations)
                    .Excluding(o => o.ParentRelatedOrganisations));
        }

        [Test]
        public static async Task GetAllAsync_VerifyMethodIsCalledOnce_VerifiesMethod()
        {
            var getAllOrganisations = new Mock<IOrganisationRepository>();
            getAllOrganisations
                .Setup(r => r.ListOrganisationsAsync())
                .ReturnsAsync(new List<Organisation>());

            var controller = OrganisationControllerBuilder.Create()
                .WithOrganisationRepository(getAllOrganisations.Object)
                .Build();

            await controller.GetAllAsync();

            getAllOrganisations.Verify(r => r.ListOrganisationsAsync());
        }

        [Test]
        public static async Task GetIdByAsync_OrganisationDoesNotExist_ReturnsNotFound()
        {
            var organisation = OrganisationBuilder.Create(1).WithOrganisationId(Guid.NewGuid()).Build();

            var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisation(organisation)
                .Build();

            var result = await controller.GetByIdAsync(Guid.NewGuid());

            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public static async Task GetIdByAsync_OrganisationExists_ReturnsTheOrganisation()
        {
            var organisation = OrganisationBuilder.Create(1).WithAddress(Address1).Build();

            var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisation(organisation)
                .Build();

            var result = await controller.GetByIdAsync(organisation.OrganisationId);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().BeEquivalentTo(
                organisation,
                conf => conf
                    .Excluding(c => c.LastUpdated)
                    .Excluding(c => c.RelatedOrganisations)
                    .Excluding(c => c.ParentRelatedOrganisations));
        }

        [Test]
        public static async Task GetByIdAsync_OrganisationAddressIsNull_ReturnsOrganisationWithNullAddress()
        {
            var organisation = OrganisationBuilder.Create(1).Build();

            var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisation(organisation)
                .Build();

            var result = await controller.GetByIdAsync(organisation.OrganisationId);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().BeEquivalentTo(
                organisation,
                conf => conf
                    .Excluding(c => c.LastUpdated)
                    .Excluding(c => c.RelatedOrganisations)
                    .Excluding(c => c.ParentRelatedOrganisations));
        }

        [Test]
        public static async Task GetIdByAsync_VerifyMethodIsCalledOnce_VerifiesMethod()
        {
            Guid expectedId = Guid.NewGuid();

            var mockGetOrganisation = new Mock<IOrganisationRepository>();
            mockGetOrganisation
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null as Organisation);

            var controller = OrganisationControllerBuilder.Create()
                .WithOrganisationRepository(mockGetOrganisation.Object)
                .Build();

            await controller.GetByIdAsync(expectedId);

            mockGetOrganisation.Verify(r => r.GetByIdAsync(expectedId));
        }

        [Test]
        public static async Task UpdateOrganisationByIdAsync_UpdateOrganisation_ReturnsStatusNoContent()
        {
            var controller = OrganisationControllerBuilder.Create().WithUpdateOrganisation(new Organisation()).Build();

            var response = await controller.UpdateOrganisationByIdAsync(Guid.Empty, new UpdateOrganisationModel());

            response.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public static async Task UpdateOrganisationByIdAsync_UpdateOrganisation_NonExistentOrganisation_ReturnsStatusNotFound()
        {
            var controller = OrganisationControllerBuilder.Create().WithUpdateOrganisation(null).Build();

            var response = await controller.UpdateOrganisationByIdAsync(Guid.Empty, new UpdateOrganisationModel());

            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public static async Task UpdateOrganisationByIdAsync_UpdateOrganisation_UpdatesCatalogueAgreementSigned()
        {
            var organisation = OrganisationBuilder.Create(1).WithCatalogueAgreementSigned(true).Build();

            var controller = OrganisationControllerBuilder.Create().WithUpdateOrganisation(organisation).Build();

            var response = await controller.UpdateOrganisationByIdAsync(organisation.OrganisationId, new UpdateOrganisationModel { CatalogueAgreementSigned = false });

            response.Should().BeOfType<NoContentResult>();

            organisation.CatalogueAgreementSigned.Should().BeFalse();
        }

        [Test]
        public static async Task UpdateOrganisationByIdAsync_OrganisationRepository_UpdateAsync_And_GetByIdAsync_CalledOnce()
        {
            var repositoryMock = new Mock<IOrganisationRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Organisation());
            repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Organisation>()));

            var controller = OrganisationControllerBuilder.Create().WithOrganisationRepository(repositoryMock.Object).Build();

            await controller.UpdateOrganisationByIdAsync(Guid.Empty, new UpdateOrganisationModel());

            repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Organisation>()));

            repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()));
        }

        [Test]
        public static void UpdateOrganisationByIdAsync_NullUpdateViewModel_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var controller = OrganisationControllerBuilder.Create().WithUpdateOrganisation(new Organisation()).Build();
                await controller.UpdateOrganisationByIdAsync(Guid.Empty, null);
            });
        }

        [Test]
        public static async Task CreateOrganisationAsync_ServiceReturnsSuccess_Returns_CreatedAtAction()
        {
            var organisationId = Guid.NewGuid();
            var controller = OrganisationControllerBuilder.Create().WithCreateOrganisationServiceReturningSuccess(organisationId).Build();

            var response = await controller.CreateOrganisationAsync(new CreateOrganisationRequestModel());

            response.Should().BeOfType<ActionResult<CreateOrganisationResponseModel>>();

            var expected = new CreatedAtActionResult(
                nameof(controller.GetByIdAsync).TrimAsync(),
                null,
                new { id = organisationId },
                new CreateOrganisationResponseModel { OrganisationId = organisationId });

            var actual = response.Result;

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static async Task CreateOrganisationAsync_ServiceReturnsFailure_Returns_BadRequest()
        {
            const string errorMessage = "Some Error Message Id";
            var controller = OrganisationControllerBuilder.Create().WithCreateOrganisationServiceReturningFailure(errorMessage).Build();

            var response = await controller.CreateOrganisationAsync(new CreateOrganisationRequestModel());

            response.Should().BeOfType<ActionResult<CreateOrganisationResponseModel>>();

            var expected = new BadRequestObjectResult(new CreateOrganisationResponseModel
            {
                Errors = new[] { new ErrorMessageViewModel(errorMessage) },
            });

            var actual = response.Result;

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static void CreateOrganisationAsync_NullCreateOrganisationRequestViewModel_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var controller = OrganisationControllerBuilder.Create().Build();
                await controller.CreateOrganisationAsync(null);
            });
        }

        [Test]
        public static async Task GetServiceRecipientsAsync_OrganisationIsNull_ReturnsNotFound()
        {
            var organisationId = Guid.NewGuid();

            var organisation = OrganisationBuilder.Create(1).WithOrganisationId(organisationId).Build();

            var invalidOrganisationId = Guid.NewGuid();

            var controller = OrganisationControllerBuilder
                .Create(invalidOrganisationId)
                .WithGetOrganisation(organisation)
                .Build();

            var result = await controller.GetServiceRecipientsAsync(invalidOrganisationId);

            result.Should().BeEquivalentTo(new ActionResult<ServiceRecipientsModel>(new NotFoundResult()));
        }

        [Test]
        public static async Task GetServiceRecipientsAsync_OrganisationExists_ReturnsTheOrganisationServiceRecipients()
        {
            var organisationId = Guid.NewGuid();

            var organisation = OrganisationBuilder.Create(1).WithOrganisationId(organisationId).Build();

            var serviceRecipient1 = ServiceRecipientBuilder
                .Create(1)
                .Build();
            var serviceRecipient2 = ServiceRecipientBuilder
                .Create(2)
                .Build();

            var controller = OrganisationControllerBuilder
                .Create(organisationId)
                .WithGetOrganisation(organisation)
                .WithServiceRecipients(new List<ServiceRecipient> { serviceRecipient2, serviceRecipient1 })
                .Build();

            var response = await controller.GetServiceRecipientsAsync(organisation.OrganisationId);

            var expected = new List<ServiceRecipientsModel>
            {
                new()
                {
                    Name = organisation.Name,
                    OdsCode = organisation.OdsCode,
                },
                new()
                {
                    Name = serviceRecipient1.Name,
                    OdsCode = serviceRecipient1.OrgId,
                },
                new()
                {
                    Name = serviceRecipient2.Name,
                    OdsCode = serviceRecipient2.OrgId,
                },
            };
            expected = expected.OrderBy(m => m.Name).ToList();

            response.Should().BeEquivalentTo(new ActionResult<List<ServiceRecipientsModel>>(expected), config => config.WithStrictOrdering());
        }

        [Test]
        public static async Task GetOrganisationsAsync_OrganisationExists_ReturnsOrganisationAndRelatedOrganisation()
        {
            Guid relatedOrganisationId = Guid.NewGuid();

            var organisation = OrganisationBuilder.Create(1).WithAddress(Address1).WithRelatedOrganisation(relatedOrganisationId).Build();

            var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisationWithRelatedOrganisations(organisation)
                .Build();

            var result = await controller.GetRelatedOrganisationsAsync(organisation.OrganisationId);

            var expected = new List<RelatedOrganisationModel>
            {
                new()
                {
                    OrganisationId = organisation.RelatedOrganisations.First().OrganisationId,
                    Name = organisation.RelatedOrganisations.First().Name,
                    OdsCode = organisation.RelatedOrganisations.First().OdsCode,
                },
            };

            result.Should().BeEquivalentTo(new ActionResult<List<RelatedOrganisationModel>>(expected));
        }

        [Test]
        public static async Task GetRelatedOrganisationsAsync_OrganisationExists_NoRelatedOrganisations_ReturnsEmptyList()
        {
            var organisation = OrganisationBuilder.Create(1).WithAddress(Address1).Build();

            var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisationWithRelatedOrganisations(organisation)
                .Build();

            var result = await controller.GetRelatedOrganisationsAsync(organisation.OrganisationId);

            result.Should().BeEquivalentTo(new ActionResult<List<RelatedOrganisationModel>>(new List<RelatedOrganisationModel>()));
        }

        [Test]
        public static async Task GetRelatedOrganisationsAsync_OrganisationDoesNotExist_ReturnsNotFound()
        {
            var nonExistentOrganisationId = Guid.NewGuid();

            var controller = OrganisationControllerBuilder
                .Create()
                .Build();

            var result = await controller.GetRelatedOrganisationsAsync(nonExistentOrganisationId);

            result.Should().BeEquivalentTo(new ActionResult<List<RelatedOrganisationModel>>(new NotFoundResult()));
        }

        [Test]
        public static async Task CreateRelatedOrganisationAsync_RelatedOrganisationReturnsSuccess_ReturnsRelatedOrganisation()
        {
            var organisation = OrganisationBuilder.Create(1).Build();

            var relatedOrganisation = OrganisationBuilder.Create(2).Build();

            var relatedOrganisationModel = new CreateRelatedOrganisationModel { RelatedOrganisationId = relatedOrganisation.OrganisationId };

            var controller = OrganisationControllerBuilder
                .Create()
                .WithGetByIdWithRelatedAndGetByIdForRelatedAndUpdateAsync(organisation, relatedOrganisation)
                .Build();

            var result = await controller.CreateRelatedOrganisationAsync(organisation.OrganisationId, relatedOrganisationModel);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public static async Task CreateRelatedOrganisationsAsync_OrganisationDoesNotExists_ReturnsNotFound()
        {
            var organisationId = Guid.NewGuid();

            var controller = OrganisationControllerBuilder
                .Create()
                .Build();

            var result = await controller.CreateRelatedOrganisationAsync(organisationId, new CreateRelatedOrganisationModel());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task CreateRelatedOrganisationsAsync_OrganisationExists_RelatedOrganisationDoesNotExist_ReturnsBadRequest()
        {
            var organisation = OrganisationBuilder.Create(1).Build();

            var nonExistingRelatedOrganisationId = Guid.NewGuid();

            var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisationWithRelatedOrganisations(organisation)
                .Build();

            var expected = new BadRequestObjectResult(new ErrorMessageViewModel(FormattableString.Invariant($"The referenced organisation {nonExistingRelatedOrganisationId} cannot be found.")));

            var result = await controller.CreateRelatedOrganisationAsync(organisation.OrganisationId, new CreateRelatedOrganisationModel { RelatedOrganisationId = nonExistingRelatedOrganisationId });

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static async Task CreateRelatedOrganisationsAsync_OrganisationExists_RelatedAlreadyExists_ReturnsBadRequest()
        {
            var relatedOrganisationId = Guid.NewGuid();

            var organisation = OrganisationBuilder.Create(1).WithRelatedOrganisation(relatedOrganisationId).Build();

            var relatedOrganisation = OrganisationBuilder.Create(2).WithOrganisationId(relatedOrganisationId).Build();

            var controller = OrganisationControllerBuilder
                .Create()
                .WithGetByIdWithRelatedAndGetByIdForRelatedAndUpdateAsync(organisation, relatedOrganisation)
                .Build();

            var expected = new BadRequestObjectResult(new ErrorMessageViewModel(FormattableString.Invariant($"The referenced organisation {relatedOrganisation.OrganisationId} is already related to {organisation.OrganisationId}.")));

            var result = await controller.CreateRelatedOrganisationAsync(organisation.OrganisationId, new CreateRelatedOrganisationModel { RelatedOrganisationId = relatedOrganisationId });

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static async Task DeleteRelatedOrganisationAsync_OrganisationExists_RelatedExists_DeletesRelationship_ReturnsNoContent()
        {
            var relatedOrganisationId = Guid.NewGuid();

            var organisation = OrganisationBuilder.Create(1).WithRelatedOrganisation(relatedOrganisationId).Build();

            var relatedOrganisation = OrganisationBuilder.Create(2).WithOrganisationId(relatedOrganisationId).Build();

            var controller = OrganisationControllerBuilder
                .Create()
                .WithGetByIdWithRelatedAndGetByIdForRelatedAndUpdateAsync(organisation, relatedOrganisation)
                .Build();

            var result = await controller.DeleteRelatedOrganisationAsync(organisation.OrganisationId, relatedOrganisationId);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public static async Task DeleteRelatedOrganisationAsync_OrganisationDoesNotExist_ReturnsNotFound()
        {
            var organisationId = Guid.NewGuid();

            var relatedOrganisationId = Guid.NewGuid();

            var controller = OrganisationControllerBuilder.Create().Build();

            var result = await controller.DeleteRelatedOrganisationAsync(organisationId, relatedOrganisationId);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task DeleteRelatedOrganisationAsync_OrganisationExists_HasRelationships_RelatedOrganisationNotRelated_ReturnsNoContent()
        {
            var relatedOrganisationId = Guid.NewGuid();

            var unrelatedOrganisationId = Guid.NewGuid();

            var organisation = OrganisationBuilder.Create(1).WithRelatedOrganisation(relatedOrganisationId).Build();

            var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisationWithRelatedOrganisations(organisation)
                .Build();

            var result = await controller.DeleteRelatedOrganisationAsync(organisation.OrganisationId, unrelatedOrganisationId);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
