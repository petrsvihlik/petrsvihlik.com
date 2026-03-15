using System.Collections.Generic;
using PetrSvihlik.Com.Models.ContentTypes;

namespace PetrSvihlik.Com.Models.ViewModels
{
    public class ProjectsViewModel : ViewModelBase
    {
        public IEnumerable<Project> Projects { get; }
        public SidebarViewModel Sidebar { get; }

        public ProjectsViewModel(IEnumerable<Project> projects, SiteMetadata metadata, SidebarViewModel sidebar)
            : base(metadata)
        {
            Projects = projects;
            Sidebar = sidebar;
        }
    }
}
