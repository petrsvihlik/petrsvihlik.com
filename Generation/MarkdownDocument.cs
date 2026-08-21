using Markdig;
using System;
using System.Collections.Generic;
using System.IO;
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
        public string Category { get; set; }
        public List<string> Tags { get; set; } = new();
        public string CanonicalUrl { get; set; }
        public string Repo { get; set; }
        public string Logo { get; set; }
        public int? Order { get; set; }
    }

    /// <summary>A Markdown content file: parsed front matter plus the body rendered to HTML.</summary>
    public sealed class MarkdownDocument
    {
        private static readonly MarkdownPipeline Pipeline =
            new MarkdownPipelineBuilder().Configure("advanced").Build();

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
