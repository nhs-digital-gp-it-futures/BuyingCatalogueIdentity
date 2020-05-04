using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.BuyingCatalogue.Identity.Api.Infrastructure
{
    [HtmlTargetElement(TagHelperConstants.Anchor, Attributes = TagHelperName)]
    public sealed class BackLinkTagHelper : AnchorTagHelper
    {
        public const string TagHelperName = "nhs-back-link";
        public const string BackLinkLabel = "back-link-label";

        public BackLinkTagHelper(IHtmlGenerator generator) 
            : base(generator)
        {
        }

        [HtmlAttributeName(BackLinkLabel)]
        public string Label { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            output.Attributes.SetAttribute(TagHelperConstants.DataTestId, "go-back-link");
            output.AddClass(TagHelperConstants.NhsBackLinkLink, HtmlEncoder.Default);
            output.Content.AppendHtml(Label);

            var svg = GetSvgBuilder();
            var path = GetPathBuilder();

            svg.InnerHtml.AppendHtml(path);
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(svg);
        }

        private static TagBuilder GetSvgBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Svg);

            builder.AddCssClass(TagHelperConstants.NhsIcon);
            builder.AddCssClass(TagHelperConstants.NhsIconChevronLeft);
            builder.Attributes.Add(TagHelperConstants.Xmlns, "http://www.w3.org/2000/svg");
            builder.Attributes.Add(TagHelperConstants.ViewBox, "0 0 24 24");
            builder.Attributes.Add(TagHelperConstants.AriaHidden, "true");

            return builder;
        }

        private static TagBuilder GetPathBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Path);
            builder.Attributes.Add("d", "M8.5 12c0-.3.1-.5.3-.7l5-5c.4-.4 1-.4 1.4 0s.4 1 0 1.4L10.9 12l4.3 4.3c.4.4.4 1 0 1.4s-1 .4-1.4 0l-5-5c-.2-.2-.3-.4-.3-.7z");

            return builder;
        }
    }
}
