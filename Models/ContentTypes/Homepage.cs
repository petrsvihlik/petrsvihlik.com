using System.Collections.Generic;
using System.Linq;

namespace PetrSvihlik.Com.Models.ContentTypes
{
    public partial class Homepage
    {
        public IEnumerable<Page> MenuItems => Subpages.OfType<Page>();
    }
}