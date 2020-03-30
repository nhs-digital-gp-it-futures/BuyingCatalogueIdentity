using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.BuyingCatalogue.Identity.Api.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "nhs-validation-input")]
    public class ValidationInputTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _htmlGenerator;

        public ValidationInputTagHelper(IHtmlGenerator htmlGenerator)
        {
            _htmlGenerator = htmlGenerator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.ThrowIfNull();

            output.Content.Clear();
            var emailField = new TagBuilder("div");

            if (ViewContext.ViewData.ContainsKey(For.Name))
            {
                emailField.AddCssClass("nhsuk-form-group");
            }

            emailField.Attributes["data-test-id"] = $"{For.Name}-field";
            var fieldInput = new TagBuilder("div");
            var formGroup = new TagBuilder("div");
            formGroup.AddCssClass("nhsuk-form-group");

            var tagBuilder = _htmlGenerator.GenerateLabel(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                labelText: null,
                htmlAttributes: new { @class = "nhsuk-label" });

            var spanBuilder = _htmlGenerator.GenerateValidationMessage(ViewContext,
                For.ModelExplorer,
                For.Name,
                null,
                "span",
                new {@class = "nhsuk-error-message"}
            );

            spanBuilder.Attributes["data-test-id"] = $"{For.Name}-error";

            var inputBuilder = _htmlGenerator.GenerateTextBox(ViewContext,
                For.ModelExplorer,
                For.Name,
                null,
                null,
                htmlAttributes: new {@class = "nhsuk-input"}
            );
            inputBuilder.Attributes["data-test-id"] = $"{For.Name}-input";

            formGroup.InnerHtml.AppendHtml(tagBuilder);
            formGroup.InnerHtml.AppendHtml(spanBuilder);
            formGroup.InnerHtml.AppendHtml(inputBuilder);
            fieldInput.InnerHtml.AppendHtml(formGroup);
            emailField.InnerHtml.AppendHtml(fieldInput);
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(emailField);
            //<input asp-for="EmailAddress" class="nhsuk-input" data-test-id="email-address" />
        }
    }
}
