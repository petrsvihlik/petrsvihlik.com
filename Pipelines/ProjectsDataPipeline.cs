using PetrSvihlik.Com.Models;
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
                new SetMetadata(MetadataKeys.ProjectModel, Config.FromDocument(async doc => (object)new Project
                {
                    Title = doc.GetString("title"),
                    Repo = doc.GetString("repo"),
                    Logo = doc.GetString("logo"),
                    Description = doc.GetString("description"),
                    ContentHtml = await doc.GetContentStringAsync(),
                    Order = doc.GetInt("order", 999),
                })),
            };
        }
    }
}
