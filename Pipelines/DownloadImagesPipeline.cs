using Statiq.Common;
using Statiq.Core;

namespace PetrSvihlik.Com.Pipelines
{
    public class DownloadImagesPipeline : Pipeline
    {
        public DownloadImagesPipeline()
        {
            // No-op: images are now local static assets, no download needed.
        }
    }
}
