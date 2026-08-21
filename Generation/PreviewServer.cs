using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace PetrSvihlik.Com.Generation
{
    /// <summary>
    /// Serves the generated output/ folder locally, mimicking GitHub Pages' extensionless
    /// URL resolution (/posts/slug -> posts/slug.html).
    /// </summary>
    public static class PreviewServer
    {
        public static void Run(string outputPath, string url = "http://localhost:5080")
        {
            var builder = WebApplication.CreateBuilder();
            builder.Logging.ClearProviders();
            var app = builder.Build();

            var fileProvider = new PhysicalFileProvider(outputPath);

            app.Use(async (context, next) =>
            {
                var requestPath = context.Request.Path.Value ?? "/";
                if (!Path.HasExtension(requestPath) && !requestPath.EndsWith('/')
                    && File.Exists(Path.Combine(outputPath, requestPath.TrimStart('/') + ".html")))
                {
                    context.Request.Path = requestPath + ".html";
                }
                await next();
            });

            app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = fileProvider });
            app.UseStaticFiles(new StaticFileOptions { FileProvider = fileProvider, ServeUnknownFileTypes = true });

            Console.WriteLine($"Serving {outputPath} at {url} (Ctrl+C to stop)");
            app.Run(url);
        }
    }
}
