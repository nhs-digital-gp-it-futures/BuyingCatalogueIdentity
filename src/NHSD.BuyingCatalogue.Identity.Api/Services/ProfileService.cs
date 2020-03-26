using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using NHSD.BuyingCatalogue.Identity.Api.Constants;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    [HtmlTargetElement("div", Attributes = MyValidationForAttributeName)]
    public class MyValidationTagHelper : TagHelper
    {
        private const string MyValidationForAttributeName = "asp-myvalidation-summary";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ViewContext.ViewData.ModelState.IsValid)
            {
                return;
            }
            var errorSummary = new TagBuilder("div");
            errorSummary.Attributes["class"] = "nhsuk-error-summary";
            errorSummary.Attributes["role"] = "alert";
            errorSummary.Attributes["aria-labelledby"] = "error-summary-title";

            var header = new TagBuilder("h2");
            header.Attributes["class"] = "nhsuk-error-summary__title";
            header.Attributes["id"] = "error-summary-title";
            header.InnerHtml.Append("There is a problem");
            errorSummary.InnerHtml.AppendHtml(header);

            var errorList = new TagBuilder("ul");
            errorList.Attributes["class"] = "nhsuk-list nhsuk-error-summary__list";
            errorSummary.InnerHtml.AppendHtml(errorList);

            foreach (var error in ViewContext.ViewData.ModelState)
            {
                var listItem = new TagBuilder("li");

                var errorElement = new TagBuilder("a");
                errorElement.Attributes.Add("href", $"#{error.Key}");
                errorElement.InnerHtml.Append(error.Value.Errors.First().ErrorMessage);
                listItem.InnerHtml.AppendHtml(errorElement);
                errorList.InnerHtml.AppendHtml(listItem);
            }

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(errorSummary);
        }
    }

    public class MyHtmlGenerator : DefaultHtmlGenerator
    {
        private HtmlEncoder encoder;
        public MyHtmlGenerator(ValidationHtmlAttributeProvider validationAttributeProvider, IAntiforgery antiForgery,
            IOptions<MvcViewOptions> optionsAccessor, IModelMetadataProvider metadataProvider,
            IUrlHelperFactory urlHelperFactory, HtmlEncoder htmlEncoder) : base(antiForgery, optionsAccessor,
            metadataProvider, urlHelperFactory, htmlEncoder, validationAttributeProvider)
        {
            encoder = htmlEncoder;
        }

        public override TagBuilder GenerateValidationSummary(ViewContext viewContext, bool excludePropertyErrors, string message,
            string headerTag, object htmlAttributes)
        {
            var baseSummary =  base.GenerateValidationSummary(viewContext, excludePropertyErrors, message, headerTag, htmlAttributes);
            return baseSummary;
        }
    }

    public sealed class ProfileService : IProfileService
    {
        private readonly IUserRepository _applicationUserRepository;

        private static readonly IDictionary<string, IEnumerable<Claim>> _organisationFunctionClaims =
            new Dictionary<string, IEnumerable<Claim>>
            {
                { "Authority", new List<Claim> { new Claim(ApplicationClaimTypes.Organisation, "Manage") } }
            };

        public ProfileService(IUserRepository applicationUserRepository)
        {
            _applicationUserRepository = applicationUserRepository ?? throw new ArgumentNullException(nameof(applicationUserRepository));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.ThrowIfNull(nameof(context));

            ApplicationUser user = await GetApplicationUserAsync(context.Subject);
            if (user is object)
            {
                context.IssuedClaims = GetClaimsFromUser(user);
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.ThrowIfNull(nameof(context));

            ApplicationUser user = await GetApplicationUserAsync(context.Subject);

            context.IsActive = user is object;
        }

        private async Task<ApplicationUser> GetApplicationUserAsync(ClaimsPrincipal subject)
        {
            subject.ThrowIfNull(nameof(subject));

            string subjectId = subject.GetSubjectId();
            return await _applicationUserRepository.FindByIdAsync(subjectId);
        }

        private List<Claim> GetClaimsFromUser(ApplicationUser user)
        {
            var claims = new List<Claim>();

            claims.AddRange(GetIdClaims(user));
            claims.AddRange(GetNameClaims(user));
            claims.AddRange(GetEmailClaims(user));
            claims.AddRange(GetOrganisationClaims(user));

            return claims;
        }

        private static IEnumerable<Claim> GetEmailClaims(ApplicationUser user)
        {
            string email = user.Email;
            if (!string.IsNullOrWhiteSpace(email))
            {
                yield return new Claim(JwtClaimTypes.Email, email);
                yield return new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean);
            }
        }

        private static IEnumerable<Claim> GetIdClaims(ApplicationUser user)
        {
            string userId = user.Id;
            if (!string.IsNullOrWhiteSpace(userId))
                yield return new Claim(JwtClaimTypes.Subject, userId);

            string username = user.UserName;
            if (!string.IsNullOrWhiteSpace(username))
            {
                yield return new Claim(JwtClaimTypes.PreferredUserName, username);
                yield return new Claim(JwtRegisteredClaimNames.UniqueName, username);
            }
        }

        private static IEnumerable<Claim> GetNameClaims(ApplicationUser user)
        {
            string firstName = user.FirstName;
            if (!string.IsNullOrWhiteSpace(firstName))
                yield return new Claim(JwtClaimTypes.GivenName, firstName);

            string lastName = user.LastName;
            if (!string.IsNullOrWhiteSpace(lastName))
                yield return new Claim(JwtClaimTypes.FamilyName, lastName);

            string name = $"{firstName?.Trim()} {lastName?.Trim()}".Trim();
            if (!string.IsNullOrWhiteSpace(name))
                yield return new Claim(JwtClaimTypes.Name, name);
        }

        private static IEnumerable<Claim> GetOrganisationClaims(ApplicationUser user)
        {
            yield return new Claim(ApplicationClaimTypes.PrimaryOrganisationId, user.PrimaryOrganisationId.ToString());

            string organisationFunction = user.OrganisationFunction;
            if (!string.IsNullOrWhiteSpace(organisationFunction))
            {
                yield return new Claim(ApplicationClaimTypes.OrganisationFunction, organisationFunction);

                if (_organisationFunctionClaims.TryGetValue(organisationFunction, out IEnumerable<Claim> organisationClaims))
                {
                    foreach (Claim claim in organisationClaims)
                    {
                        yield return claim;
                    }
                }
            }
        }
    }
}
