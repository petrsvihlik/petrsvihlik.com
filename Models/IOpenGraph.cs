using Kontent.Ai.Delivery.Abstractions;
using System.Collections.Generic;

namespace PetrSvihlik.Com.Models
{
    public interface IOpenGraph
    {
        virtual string OgType => "website";
        string OgTitle { get; }
        string OgDescription { get; }
        IEnumerable<IAsset> OgImage { get; }
    }
}
