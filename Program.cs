using Microsoft.Extensions.Configuration;
using PetrSvihlik.Com.Generation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrSvihlik.Com
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootPath = Directory.GetCurrentDirectory();

            if (args.FirstOrDefault()?.Equals("new", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NewDraft(rootPath, string.Join(' ', args.Skip(1)).Trim());
            }
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(rootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();
            if (args.Contains("drafts", StringComparer.OrdinalIgnoreCase))
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string> { ["Drafts"] = "true" });
            }
            var configuration = configurationBuilder.Build();

            var builder = new SiteBuilder(rootPath, configuration);
            await builder.BuildAsync();

            if (args.Contains("preview", StringComparer.OrdinalIgnoreCase))
            {
                PreviewServer.Run(Path.Combine(rootPath, "output"));
            }

            return 0;
        }

        /// <summary>Scaffolds input/posts/&lt;slug&gt;.md as a draft with front matter prefilled.</summary>
        private static int NewDraft(string rootPath, string title)
        {
            var slug = Slugify(title);
            if (slug.Length == 0)
            {
                Console.Error.WriteLine("Usage: dotnet run -- new \"Post Title\"");
                return 1;
            }

            var relativePath = Path.Combine("input", "posts", $"{slug}.md");
            var path = Path.Combine(rootPath, relativePath);
            if (File.Exists(path))
            {
                Console.Error.WriteLine($"{relativePath} already exists — refusing to overwrite.");
                return 1;
            }

            var content = $"""
                ---
                title: "{title.Replace("\"", "\\\"")}"
                description: ""
                slug: {slug}
                date: {DateTime.Now:yyyy-MM-dd}
                category:
                tags: []
                draft: true
                comments: true
                ---


                """;
            File.WriteAllText(path, content);
            Console.WriteLine($"Created {relativePath} (draft). Preview it with: dotnet run -- preview drafts");
            return 0;
        }

        /// <summary>Turns a title into a URL slug: lowercase ASCII, diacritics folded, hyphens between words.</summary>
        private static string Slugify(string title)
        {
            var builder = new StringBuilder();
            foreach (var c in title.Normalize(NormalizationForm.FormD))
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }
                if (char.IsAsciiLetterOrDigit(c))
                {
                    builder.Append(char.ToLowerInvariant(c));
                }
                else if (builder.Length > 0 && builder[^1] != '-')
                {
                    builder.Append('-');
                }
            }
            return builder.ToString().Trim('-');
        }
    }
}
