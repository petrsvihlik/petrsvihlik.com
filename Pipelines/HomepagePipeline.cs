using PetrSvihlik.Com.Models.ContentTypes;
using Statiq.Common;
using Statiq.Core;
using System.Collections.Generic;

namespace PetrSvihlik.Com.Pipelines
{
    public class HomepagePipeline : Pipeline
    {
        public HomepagePipeline()
        {
            InputModules = new ModuleList
            {
                new ExecuteConfig(Config.FromContext(ctx =>
                {
                    var homepage = new Homepage
                    {
                        NavigationPages = new List<Page>
                        {
                            new Page { Title = "Articles", Url = "/", ShowInNavigation = true },
                            new Page { Title = "Projects", Url = "projects", ShowInNavigation = true },
                            new Page { Title = "About me", Url = "about-me", ShowInNavigation = true },
                        }
                    };
                    return new[] { ctx.CreateDocument(new MetadataItems { { "Homepage", homepage } }) };
                }))
            };
        }
    }
}
