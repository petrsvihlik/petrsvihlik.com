using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace PetrSvihlik.Com.TagHelpers
{
    [HtmlTargetElement("gist", Attributes = "id,username")]
    public class GistTagHelper : TagHelper
    {
        [HtmlAttributeName("id")]
        public Guid Id { get; set; }

        [HtmlAttributeName("username")]
        public string Username { get; set; }

        public GistTagHelper()
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var id = Id.ToString("N");
            var username = !string.IsNullOrWhiteSpace(Username) ? Username + "/" : string.Empty;
            var content = $"<script src=\"https://gist.github.com/{username}{id}.js\"></script>";
            output.Content.SetHtmlContent(content);
        }
    }
}
