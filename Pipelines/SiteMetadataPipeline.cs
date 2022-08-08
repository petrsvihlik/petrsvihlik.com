using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Urls.Delivery.QueryParameters;
using PetrSvihlik.Com.Models.ContentTypes;

namespace PetrSvihlik.Com.Pipelines
{
    public class SiteMetadataPipeline : LoadDataPipeLine<SiteMetadata>
    {
        public SiteMetadataPipeline(IDeliveryClient deliveryClient) : base(deliveryClient, new DepthParameter(2))
        {
        }
    }
}