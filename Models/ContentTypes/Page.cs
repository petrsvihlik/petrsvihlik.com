using PetrSvihlik.Com.Models;
using System.Linq;

namespace PetrSvihlik.Com.Models.ContentTypes
{
    public partial class Page : ITitleProvider
    {
        public string ElementCodename => TitleCodename;

        public bool ShowInMenu => ShowInNavigation.FirstOrDefault().Codename == "true";
    }
}