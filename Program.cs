using System.Threading.Tasks;
using Statiq.App;
using Statiq.Web;

namespace PetrSvihlik.Com
{
    public class Program
    {
        public static async Task<int> Main(string[] args) =>
            await Bootstrapper
                .Factory
                .CreateDefault(args)
                .AddWeb()
                .AddHostingCommands()
                .AddSetting(WebKeys.ContentFiles, new[] { "**/{!_,}*.{html,cshtml,md}", "!projects/**" })
                .RunAsync();
    }
}
