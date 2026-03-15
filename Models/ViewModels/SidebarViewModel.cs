using PetrSvihlik.Com.Models.ContentTypes;
using System.Collections.Generic;

namespace PetrSvihlik.Com.Models.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        public IEnumerable<Contact> Contacts { get; private set; }

        public Homepage Homepage { get; private set; }

        public string ActiveMenuItem { get; private set; }

        public bool IsIndex { get; private set; }

        public SidebarViewModel(Homepage homepage, SiteMetadata metadata, bool isIndex, string activeMenuItem) : base(metadata)
        {
            Contacts = metadata?.SiteAuthor?.Contacts ?? new List<Contact>();
            Homepage = homepage;
            IsIndex = isIndex;
            ActiveMenuItem = activeMenuItem;
        }
    }
}
