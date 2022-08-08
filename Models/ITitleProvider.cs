using Kontent.Ai.Delivery.Abstractions;

namespace PetrSvihlik.Com.Models
{
    public interface ITitleProvider
    {
        string Title { get; }

        IContentItemSystemAttributes System { get; }

        string ElementCodename { get; }
    }
}
