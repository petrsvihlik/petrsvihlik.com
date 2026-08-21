using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PetrSvihlik.Com.Generation
{
    /// <summary>YAML front matter of a content file. Property names map from snake_case keys.</summary>
    public sealed class FrontMatter
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public DateTime? Date { get; set; }

        /// <summary>When the content was last substantively revised; the original Date stays the publication date.</summary>
        public DateTime? Updated { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; } = new();
        public string CanonicalUrl { get; set; }
        public string Repo { get; set; }
        public string Logo { get; set; }
        public int? Order { get; set; }

        /// <summary>Render the page but keep it out of the sitemap and ask crawlers not to index it.</summary>
        public bool Unlisted { get; set; }

        /// <summary>Skip the post in production builds; rendered (noindexed) only when the Drafts setting is on.</summary>
        public bool Draft { get; set; }

        /// <summary>Render a giscus comment thread (backed by a GitHub Discussion) under the content.</summary>
        public bool Comments { get; set; }

        /// <summary>Render the newsletter signup form under a page's content (posts get it always).</summary>
        public bool Newsletter { get; set; }
    }

    /// <summary>A Markdown content file: parsed front matter plus the body rendered to HTML.</summary>
    public sealed class MarkdownDocument
    {
        private static readonly MarkdownPipeline Pipeline = CreatePipeline();

        private static MarkdownPipeline CreatePipeline()
        {
            var builder = new MarkdownPipelineBuilder().Configure("advanced");
            builder.DocumentProcessed += document =>
            {
                // native lazy loading for every content image
                foreach (var image in document.Descendants<LinkInline>().Where(link => link.IsImage))
                {
                    var attributes = image.GetAttributes();
                    attributes.AddProperty("loading", "lazy");
                    attributes.AddProperty("decoding", "async");
                }
            };
            return builder.Build();
        }

        private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        public FrontMatter FrontMatter { get; private init; }
        public string BodyHtml { get; private init; }
        public string FileName { get; private init; }

        public string Slug => FrontMatter.Slug ?? FileName;

        public static MarkdownDocument Load(string path)
        {
            var text = File.ReadAllText(path);
            var (yaml, body) = SplitFrontMatter(text);
            return new MarkdownDocument
            {
                FrontMatter = yaml is null ? new FrontMatter() : YamlDeserializer.Deserialize<FrontMatter>(yaml),
                BodyHtml = Markdown.ToHtml(body, Pipeline),
                FileName = Path.GetFileNameWithoutExtension(path),
            };
        }

        private static (string Yaml, string Body) SplitFrontMatter(string text)
        {
            var content = text.TrimStart('﻿');
            if (!content.StartsWith("---", StringComparison.Ordinal))
            {
                return (null, content);
            }

            var closing = content.IndexOf("\n---", 3, StringComparison.Ordinal);
            if (closing < 0)
            {
                return (null, content);
            }

            var yaml = content[3..closing];
            var body = content[(closing + 4)..].TrimStart('\r').TrimStart('\n');
            return (yaml, body);
        }
    }
}
