using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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

            var outerDiv = GetOuterDivBuilder();
            var formGroup = GetFormGroupBuilder();
            var label = GetLabelBuilder();
            var validation = GetValidationBuilder();
            var input = GetInputBuilder();

            formGroup.InnerHtml.AppendHtml(label);
            formGroup.InnerHtml.AppendHtml(validation);
            formGroup.InnerHtml.AppendHtml(input);
            outerDiv.InnerHtml.AppendHtml(formGroup);

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(outerDiv);
        }

        private TagBuilder GetOuterDivBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.Attributes[TagHelperConstants.DataTestId] = FieldDataTestId ?? $"{For.Name}-field";

            var modelState = ViewContext.ViewData.ModelState;
            if (!modelState.ContainsKey(For.Name))
            {
                return builder;
            }

            if(modelState[For.Name].Errors.Any())
            {
                builder.AddCssClass(TagHelperConstants.NhsFormGroupError);
            }

            return builder;
        }

        private static TagBuilder GetFormGroupBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(TagHelperConstants.NhsFormGroup);
            return builder;
        }

        private TagBuilder GetLabelBuilder()
        {
            var builder = _htmlGenerator.GenerateLabel(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                null,
                null);

            builder.AddCssClass(TagHelperConstants.NhsLabel);

            return builder;
        }

        private TagBuilder GetValidationBuilder()
        {
            var builder = _htmlGenerator.GenerateValidationMessage(ViewContext,
                For.ModelExplorer,
                For.Name,
                null,
                TagHelperConstants.Span,
                null);

            builder.AddCssClass(TagHelperConstants.NhsErrorMessage);
            builder.Attributes[TagHelperConstants.DataTestId] = ErrorDataTestId ?? $"{For.Name}-error";

            return builder;
        }

        private TagBuilder GetInputBuilder()
        {
            var builder = _htmlGenerator.GenerateTextBox(ViewContext,
                For.ModelExplorer,
                For.Name,
                null,
                null,
                null);

            var dataTypeAttributes = For?.Metadata?
                .ContainerType?
                .GetProperty(For.Name)?
                .GetCustomAttributes<DataTypeAttribute>();

            if (dataTypeAttributes?.Any(x => x.DataType == DataType.Password) == true)
            {
                builder.Attributes[TagHelperConstants.Type] = "password";
            }

            builder.AddCssClass(TagHelperConstants.NhsInput);
            builder.Attributes[TagHelperConstants.DataTestId] = InputDataTestId ?? $"{For.Name}-input";

            return builder;
        }
    }
}
