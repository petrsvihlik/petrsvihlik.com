using PetrSvihlik.Com.Models;
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
                        .Select(d => d.Get<Project>(MetadataKeys.ProjectModel))
                        .ToList();
                    var viewModel = new ProjectsViewModel(projects, ctx.GetSiteMetadata(), ctx.CreateSidebar(activeMenuItem: "projects"));
                    return new[] { ctx.CreateDocument(new MetadataItems { { MetadataKeys.ProjectsViewModel, viewModel } }) };
                })),
                new SetDestination(new NormalizedPath("pages/projects/index.html")),
                new MergeContent(new ReadFiles("_Projects.cshtml")),
                new RenderRazor().WithModel(Config.FromDocument((doc, ctx) =>
                    doc.Get<ProjectsViewModel>(MetadataKeys.ProjectsViewModel))),
            };

            OutputModules = new ModuleList
            {
                new WriteFiles(),
            };
        }
    }
}
