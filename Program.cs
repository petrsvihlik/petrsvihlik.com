using Kentico.Kontent.Delivery.Abstractions;
using Kentico.Kontent.Delivery.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Microsoft.Extensions.Configuration;
using Statiq.Web;
using PetrSvihlik.Com.Models.ContentTypes;
using PetrSvihlik.Com.Resolvers;

namespace PetrSvihlik.Com
{
    public class Program
    {
        public static async Task<int> Main(string[] args) =>
            await Bootstrapper
                .Factory
                .CreateDefault(args)
                .BuildConfiguration(cfg => cfg.AddUserSecrets<Program>(true))
                .ConfigureServices((services, settings) =>
                {
                    services.AddSingleton<IContentLinkUrlResolver, CustomContentLinkUrlResolver>();
                    services.AddSingleton<ITypeProvider, CustomTypeProvider>();
                    services.AddDeliveryClient((IConfiguration)settings);
                })
                .AddHostingCommands()
                .RunAsync();
    }
}
