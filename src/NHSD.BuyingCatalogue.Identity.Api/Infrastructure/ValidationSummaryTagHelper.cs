using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.BuyingCatalogue.Identity.Api.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "asp-nhs-validation-summary")]
    public class ValidationSummaryTagHelper : TagHelper
    {
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
            var viewType = ViewContext.ViewData.Model.GetType();

            viewType.ThrowIfNull();
            errorSummary.AddCssClass("nhsuk-error-summary");
            errorSummary.Attributes["role"] = "alert";
            errorSummary.Attributes["aria-labelledby"] = "error-summary-title";

            var header = new TagBuilder("h2");
            header.AddCssClass("nhsuk-error-summary__title");
            header.Attributes["id"] = "error-summary-title";
            header.InnerHtml.Append("There is a problem");
            errorSummary.InnerHtml.AppendHtml(header);

            var errorList = new TagBuilder("ul");
            errorList.AddCssClass("nhsuk-list");
            errorList.AddCssClass(" nhsuk-error-summary__list");
            errorSummary.InnerHtml.AppendHtml(errorList);

            foreach (var model in ViewContext.ViewData.ModelState)
            {
                if (!model.Value.Errors.Any())
                {
                    continue;
                }

                foreach (var error in model.Value.Errors)
                {
                    var listItem = new TagBuilder("li");
                    var errorElement = new TagBuilder("a");
                    errorElement.Attributes.Add("href", $"#{model.Key}");
                    errorElement.InnerHtml.Append(error.ErrorMessage);
                    listItem.InnerHtml.AppendHtml(errorElement);
                    errorList.InnerHtml.AppendHtml(listItem);
                }
            }

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(errorSummary);
        }
    }
}
