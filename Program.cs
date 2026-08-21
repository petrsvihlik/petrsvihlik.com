using Microsoft.Extensions.Configuration;
using PetrSvihlik.Com.Generation;
using System;
using System.Collections.Generic;
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
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(rootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();
            if (args.Contains("drafts", StringComparer.OrdinalIgnoreCase))
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string> { ["Drafts"] = "true" });
            }
            var configuration = configurationBuilder.Build();

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
