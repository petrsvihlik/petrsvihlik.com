using PetrSvihlik.Com.Models.ContentTypes;
using Statiq.Common;
using Statiq.Core;
using Statiq.Markdown;
using Statiq.Yaml;

namespace PetrSvihlik.Com.Pipelines
{
    public class ProjectsDataPipeline : Pipeline
    {
        public ProjectsDataPipeline()
        {
            InputModules = new ModuleList
            {
                new ReadFiles("projects/_*.md"),
                new ExtractFrontMatter(new ParseYaml()),
                new RenderMarkdown().UseExtensions(),
                new SetMetadata("ProjectModel", Config.FromDocument((doc, ctx) => new Project
                {
                    Title = doc.GetString("title"),
                    Repo = doc.GetString("repo"),
                    Logo = doc.GetString("logo"),
                    Description = doc.GetString("description"),
                    ContentHtml = doc.GetContentStringAsync().GetAwaiter().GetResult(),
                    Order = doc.GetInt("order", 999),
                })),
            };
        }
    }
}
