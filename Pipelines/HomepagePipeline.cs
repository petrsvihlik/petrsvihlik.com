using Kontent.Ai.Delivery.Abstractions;
using PetrSvihlik.Com.Models.ContentTypes;

namespace PetrSvihlik.Com.Pipelines
{
    public class HomepagePipeline : LoadDataPipeLine<Homepage>
    {
        public HomepagePipeline(IDeliveryClient deliveryClient) : base(deliveryClient)
        {
        }
    }
}