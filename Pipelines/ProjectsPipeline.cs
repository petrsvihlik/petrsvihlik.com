using PetrSvihlik.Com.Models.ContentTypes;
using PetrSvihlik.Com.Models.ViewModels;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using System.Linq;

namespace PetrSvihlik.Com.Pipelines
{
    public class ProjectsPipeline : Pipeline
    {
        public ProjectsPipeline()
        {
            Dependencies.AddRange(nameof(ProjectsDataPipeline), nameof(HomepagePipeline), nameof(SiteMetadataPipeline));

            ProcessModules = new ModuleList
            {
                new ExecuteConfig(Config.FromContext(ctx =>
                {
                    var projects = ctx.Outputs.FromPipeline(nameof(ProjectsDataPipeline))
                        .OrderBy(d => d.GetInt("order", 999))
                        .Select(d => d.Get<Project>("ProjectModel"))
                        .ToList();
                    var metadata = ctx.Outputs.FromPipeline(nameof(SiteMetadataPipeline))
                        .Select(x => x.Get<SiteMetadata>("SiteMetadata")).FirstOrDefault();
                    var homepage = ctx.Outputs.FromPipeline(nameof(HomepagePipeline))
                        .Select(x => x.Get<Homepage>("Homepage")).FirstOrDefault();
                    var sidebar = new SidebarViewModel(homepage, metadata, false, "projects");
                    var viewModel = new ProjectsViewModel(projects, metadata, sidebar);
                    return new[] { ctx.CreateDocument(new MetadataItems { { "ProjectsViewModel", viewModel } }) };
                })),
                new SetDestination(new NormalizedPath("pages/projects/index.html")),
                new MergeContent(new ReadFiles("_Projects.cshtml")),
                new RenderRazor().WithModel(Config.FromDocument((doc, ctx) =>
                    doc.Get<ProjectsViewModel>("ProjectsViewModel"))),
            };

            OutputModules = new ModuleList
            {
                new WriteFiles(),
            };
        }
    }
}
