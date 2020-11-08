using Statiq.Common;
using Statiq.Core;
using Statiq.Sass;

namespace PetrSvihlik.Com.Pipelines
{
    public class StyleSheetsPipeline : Pipeline
    {
        public StyleSheetsPipeline()
        {
            InputModules = new ModuleList {
                new ReadFiles(pattern: "assets/scss/**/{!_,}*.scss"),
                
                #if DEBUG
                new CompileSass().WithCompactOutputStyle(),
                #else
                new CompileSass().WithCompressedOutputStyle(),
                #endif
 
                new SetDestination(".css"),
                new WriteFiles()
            };
        }
    }
}

