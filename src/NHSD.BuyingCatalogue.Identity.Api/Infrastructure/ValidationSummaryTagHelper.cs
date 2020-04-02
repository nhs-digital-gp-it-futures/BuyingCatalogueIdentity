using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.BuyingCatalogue.Identity.Api.Infrastructure
{
    [HtmlTargetElement(TagHelperConstants.Div, Attributes = TagHelperName)]
    public sealed class ValidationSummaryTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-validation-summary";
        public const string TitleName = "title";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TitleName)]
        public string Title { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.ThrowIfNull();
            if (ViewContext.ViewData.ModelState.IsValid)
            {
                output.Content.Clear();
                return;
            }

            var errorSummary = GetErrorSummaryBuilder();
            var header = GetHeaderBuilder();
            var errorList = GetErrorListBuilder();

            errorSummary.InnerHtml.AppendHtml(header);
            errorSummary.InnerHtml.AppendHtml(errorList);

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(errorSummary);
        }

        private static TagBuilder GetErrorSummaryBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(TagHelperConstants.NhsValidationSummary);
            builder.Attributes[TagHelperConstants.Role] = TagHelperConstants.RoleAlert;
            builder.Attributes[TagHelperConstants.LabelledBy] = TagHelperConstants.ErrorSummaryTitle;

            return builder;
        }

        private TagBuilder GetHeaderBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.SubHeader);
            builder.AddCssClass(TagHelperConstants.NhsValidationSummaryTitle);
            builder.Attributes[TagHelperConstants.Id] = TagHelperConstants.ErrorSummaryTitle;
            builder.InnerHtml.Append(Title);

            return builder;
        }

        private TagBuilder GetErrorListBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.UnorderedList);
            builder.AddCssClass(TagHelperConstants.NhsList);
            builder.AddCssClass(TagHelperConstants.NhsValidationSummaryList);

            var viewType = ViewContext.ViewData.Model.GetType();
            viewType.ThrowIfNull();

            var propertyNames = viewType.GetProperties().Select(x => x.Name).ToList();
            var orderedStates = ViewContext.ViewData.ModelState
                .OrderBy(d => propertyNames.IndexOf(d.Key))
                .ToList();

            foreach (var model in orderedStates)
            {
                var redirectAttribute = ViewContext.ViewData.Model.GetType().GetProperty(model.Key)
                    ?.GetCustomAttributes<SummaryRedirectAttribute>().FirstOrDefault();
                
                foreach (var error in model.Value.Errors)
                {
                    var redirect = model.Key;
                    if (redirectAttribute != null)
                    {
                        redirect = redirectAttribute.Link;
                    }

                    var listItem = GetListItemBuilder(redirect, error.ErrorMessage);
                    builder.InnerHtml.AppendHtml(listItem);
                }
            }

            return builder;
        }

        private static TagBuilder GetListItemBuilder(string linkElement, string errorMessage)
        {
            var listItemBuilder = new TagBuilder(TagHelperConstants.ListItem);
            var linkBuilder = new TagBuilder(TagHelperConstants.Anchor);
            linkBuilder.Attributes.Add(TagHelperConstants.Link, $"#{linkElement}");
            linkBuilder.InnerHtml.Append(errorMessage);
            listItemBuilder.InnerHtml.AppendHtml(linkBuilder);

            return listItemBuilder;
        }
    }
}
