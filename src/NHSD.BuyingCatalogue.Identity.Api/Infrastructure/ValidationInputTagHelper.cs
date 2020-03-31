using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.BuyingCatalogue.Identity.Api.Infrastructure
{
    [HtmlTargetElement(TagHelperConstants.Div, Attributes = TagHelperName)]
    public sealed class ValidationInputTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-validation-input";
        public const string InputDataTestIdName = "input-data-test-id";
        public const string FieldDataTestIdName = "field-data-test-id";
        public const string ErrorDataTestIdName = "error-data-test-id";

        private readonly IHtmlGenerator _htmlGenerator;

        public ValidationInputTagHelper(IHtmlGenerator htmlGenerator)
        {
            _htmlGenerator = htmlGenerator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(FieldDataTestIdName)]
        public string FieldDataTestId { get; set; }

        [HtmlAttributeName(InputDataTestIdName)]
        public string InputDataTestId { get; set; }

        [HtmlAttributeName(ErrorDataTestIdName)]
        public string ErrorDataTestId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.ThrowIfNull();

            output.Content.Clear();

            var outerDivBuilder = new TagBuilder(TagHelperConstants.Div);

            if (ViewContext.ViewData.ModelState.ContainsKey(For.Name))
            {
                outerDivBuilder.AddCssClass(TagHelperConstants.NhsFormGroupError);
            }

            outerDivBuilder.Attributes[TagHelperConstants.DataTestId] = FieldDataTestId ?? $"{For.Name}-field";

            var formGroup = new TagBuilder(TagHelperConstants.Div);
            formGroup.AddCssClass(TagHelperConstants.NhsFormGroup);

            var labelBuilder = _htmlGenerator.GenerateLabel(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                null, 
                null);

            labelBuilder.AddCssClass(TagHelperConstants.NhsLabel);

            var validationBuilder = _htmlGenerator.GenerateValidationMessage(ViewContext,
                For.ModelExplorer,
                For.Name,
                null,
                TagHelperConstants.Span,
                null);

            validationBuilder.AddCssClass(TagHelperConstants.NhsErrorMessage);
            validationBuilder.Attributes[TagHelperConstants.DataTestId] = ErrorDataTestId ?? $"{For.Name}-error";
            
            var inputBuilder = _htmlGenerator.GenerateTextBox(ViewContext,
                For.ModelExplorer,
                For.Name,
                null,
                null,
                null);

            if (this.ViewContext.ViewData.ModelMetadata.Properties.First(x => x.Name == For.Name).DataTypeName == "Password")
            {
                inputBuilder.Attributes[TagHelperConstants.Type] = "password";
            }

            inputBuilder.AddCssClass(TagHelperConstants.NhsInput);
            inputBuilder.Attributes[TagHelperConstants.DataTestId] = InputDataTestId ?? $"{For.Name}-input";

            formGroup.InnerHtml.AppendHtml(labelBuilder);
            formGroup.InnerHtml.AppendHtml(validationBuilder);
            formGroup.InnerHtml.AppendHtml(inputBuilder);
            outerDivBuilder.InnerHtml.AppendHtml(formGroup);

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(outerDivBuilder);
        }
    }
}
