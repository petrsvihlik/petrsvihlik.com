using PetrSvihlik.Com.Models;

namespace PetrSvihlik.Com.Models.ContentTypes
{
    public partial class Category : ITitleProvider
    {
        public string ElementCodename => TitleCodename;
    }
}