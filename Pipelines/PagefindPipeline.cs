using Microsoft.Extensions.Logging;
using Statiq.Common;
using Statiq.Core;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PetrSvihlik.Com.Pipelines
{
    /// <summary>
    /// Runs Pagefind (https://pagefind.app/) after all output files have been written,
    /// generating a full-text search index under output/pagefind/.
    /// Requires Node.js / npx to be available on the PATH.
    /// </summary>
    public class PagefindPipeline : Pipeline
    {
        public PagefindPipeline()
        {
            Dependencies.AddRange(
                nameof(PostsPipeline),
                nameof(PagesPipeline),
                nameof(HomePipeline),
                nameof(CategoriesPipeline),
                nameof(TagsPipeline),
                nameof(FeedsPipeline),
                nameof(SiteMapPipeline),
                nameof(StyleSheetsPipeline),
                nameof(CopyAssetsPipeline)
            );

            OutputModules = new ModuleList
            {
                new ExecuteConfig(Config.FromContext(ctx =>
                {
                    var log = (ILogger)ctx;
                    var outputPath = ctx.FileSystem.OutputPath.FullPath;
                    var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

                    var psi = new ProcessStartInfo
                    {
                        FileName = isWindows ? "cmd" : "npx",
                        Arguments = isWindows
                            ? $"/c npx pagefind --site \"{outputPath}\""
                            : $"pagefind --site \"{outputPath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = ctx.FileSystem.RootPath.FullPath,
                    };

                    log.LogInformation("Running Pagefind to build search index...");
                    using var process = new Process { StartInfo = psi };
                    process.Start();

                    var stdout = process.StandardOutput.ReadToEnd();
                    var stderr = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrWhiteSpace(stdout))
                        log.LogInformation(stdout.Trim());

                    if (process.ExitCode != 0)
                    {
                        if (!string.IsNullOrWhiteSpace(stderr))
                            log.LogError(stderr.Trim());
                        throw new Exception($"Pagefind failed with exit code {process.ExitCode}");
                    }

                    log.LogInformation("Pagefind index built successfully.");
                    return Array.Empty<IDocument>();
                }))
            };
        }
    }
}
