using PetrSvihlik.Com.Models.ContentTypes;

namespace PetrSvihlik.Com.Models.ViewModels
{
    public class PostViewModel : ViewModelBase
    {
        public Article Article { get; private set; }
        public SidebarViewModel Sidebar { get; private set; }

        public PostViewModel(Article article, SiteMetadata metadata, SidebarViewModel sidebar) : base(metadata)
        {
            Article = article;
            Sidebar = sidebar;
        }
    }
}
