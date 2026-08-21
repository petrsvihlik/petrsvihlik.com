using Microsoft.Extensions.Configuration;
using PetrSvihlik.Com.Generation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PetrSvihlik.Com
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(rootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new SiteBuilder(rootPath, configuration);
            await builder.BuildAsync();

            if (args.Contains("preview", StringComparer.OrdinalIgnoreCase))
            {
                PreviewServer.Run(Path.Combine(rootPath, "output"));
            }

            return 0;
        }
    }
}
