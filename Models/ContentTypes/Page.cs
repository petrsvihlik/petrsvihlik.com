using System.Linq;

namespace Kentico.Kontent.Statiq.Lumen.Models
{
    public partial class Page : ITitleProvider
    {
        public string ElementCodename => TitleCodename;

        public bool ShowInMenu => ShowInNavigation.FirstOrDefault().Codename == "true";
    }
}