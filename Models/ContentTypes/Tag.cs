using PetrSvihlik.Com.Models;

namespace PetrSvihlik.Com.Models.ContentTypes
{
    public partial class Tag : ITitleProvider
    {
        public string ElementCodename => TitleCodename;
    }
}