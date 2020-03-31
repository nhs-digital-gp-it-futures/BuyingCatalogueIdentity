using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.BuyingCatalogue.Identity.Api.Infrastructure
{
    [HtmlTargetElement(TagHelperConstants.Div, Attributes = TagHelperName)]
    public class ValidationSummaryTagHelper : TagHelper
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

            var errorSummary = new TagBuilder(TagHelperConstants.Div);
            errorSummary.AddCssClass(TagHelperConstants.NhsValidationSummary);
            errorSummary.Attributes[TagHelperConstants.Role] = TagHelperConstants.RoleAlert;
            errorSummary.Attributes[TagHelperConstants.LabelledBy] = TagHelperConstants.ErrorSummaryTitle;

            var header = new TagBuilder(TagHelperConstants.SubHeader);
            header.AddCssClass(TagHelperConstants.NhsValidationSummaryTitle);
            header.Attributes[TagHelperConstants.Id] = TagHelperConstants.ErrorSummaryTitle;
            header.InnerHtml.Append(Title);
            errorSummary.InnerHtml.AppendHtml(header);

            var errorList = new TagBuilder(TagHelperConstants.UnorderedList);
            errorList.AddCssClass(TagHelperConstants.NhsList);
            errorList.AddCssClass(TagHelperConstants.NhsValidationSummaryList);
            errorSummary.InnerHtml.AppendHtml(errorList);

            foreach (var model in ViewContext.ViewData.ModelState)
            {
                if (!model.Value.Errors.Any())
                {
                    continue;
                }

                foreach (var error in model.Value.Errors)
                {
                    var listItem = new TagBuilder(TagHelperConstants.ListItem);
                    var errorElement = new TagBuilder(TagHelperConstants.Anchor);
                    errorElement.Attributes.Add(TagHelperConstants.Link, $"#{model.Key}");
                    errorElement.InnerHtml.Append(error.ErrorMessage);
                    listItem.InnerHtml.AppendHtml(errorElement);
                    errorList.InnerHtml.AppendHtml(listItem);
                }
            }

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(errorSummary);
        }
    }
}
