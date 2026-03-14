using PetrSvihlik.Com.Models.ContentTypes;
using Statiq.Common;
using Statiq.Core;
using System.Collections.Generic;

namespace PetrSvihlik.Com.Pipelines
{
    public class SiteMetadataPipeline : Pipeline
    {
        public SiteMetadataPipeline()
        {
            InputModules = new ModuleList
            {
                new ExecuteConfig(Config.FromContext(ctx =>
                {
                    var metadata = new SiteMetadata
                    {
                        Title = "Petr Švihlík",
                        Subtitle = "Posts about DevRel, DX, OS, and .NET.",
                        Copyright = "",
                        SiteAuthor = new Author
                        {
                            Name = "Petr Švihlík",
                            Bio = "VP Engineering @ Kentico, ex-Microsoft, Open-So(u)rcerer, .NET freak, explorer of the planet earth.",
                            Contacts = new List<Contact>
                            {
                                new Contact { Name = "GitHub", Url = "https://github.com/petrsvihlik", Icon = "icon-github-circled" },
                                new Contact { Name = "Stack Overflow", Url = "https://stackoverflow.com/users/1332034/rocky", Icon = "icon-stackoverflow" },
                                new Contact { Name = "Dev.to", Url = "https://dev.to/petrsvihlik", Icon = "icon-devto" },
                                new Contact { Name = "Medium", Url = "https://medium.com/@PetrSvihlik", Icon = "icon-medium" },
                                new Contact { Name = "LinkedIn", Url = "https://www.linkedin.com/in/svihlik/", Icon = "icon-linkedin" },
                                new Contact { Name = "RSS", Url = "/feed.rss", Icon = "icon-rss" },
                            }
                        }
                    };
                    return new[] { ctx.CreateDocument(new MetadataItems { { "SiteMetadata", metadata } }) };
                }))
            };
        }
    }
}
