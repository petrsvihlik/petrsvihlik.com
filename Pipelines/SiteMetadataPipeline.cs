using Kentico.Kontent.Delivery.Abstractions;
using Kentico.Kontent.Delivery.Urls.QueryParameters;
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