using PetrSvihlik.Com.Models.ContentTypes;
using System.Collections.Generic;

namespace PetrSvihlik.Com.Models.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public Page Page { get; }

        public PagedContent<Article> Articles { get; }

        /// <summary>
        /// Full, unpaginated article list — powers the homepage client-side
        /// filter so it can search the whole archive (not just the current
        /// page). Null on non-homepage views.
        /// </summary>
        public IReadOnlyList<Article> AllArticles { get; init; }

        public SidebarViewModel Sidebar { get; }

        public ITitleProvider TitleProvider { get; }

        public HomeViewModel(PagedContent<Article> articles, SidebarViewModel sidebar, ITitleProvider titleProvider = null) : this(sidebar, titleProvider)
        {
            Articles = articles;
        }

        public HomeViewModel(Page page, SidebarViewModel sidebar) : this(sidebar, page)
        {
            Page = page;
        }

        private HomeViewModel(SidebarViewModel sidebar, ITitleProvider titleProvider) : base(sidebar.Metadata)
        {
            Sidebar = sidebar;
            TitleProvider = titleProvider;
        }
    }
}
