using Statiq.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetrSvihlik.Com.Modules
{
    /// <summary>
    /// Clones each input document with a batch of metadata items computed from the document,
    /// so several related entries can be set in a single pass.
    /// </summary>
    public class SetMetadataItems : Module
    {
        private readonly Func<IDocument, IExecutionContext, Task<MetadataItems>> _getMetadata;

        public SetMetadataItems(Func<IDocument, IExecutionContext, Task<MetadataItems>> getMetadata)
        {
            _getMetadata = getMetadata;
        }

        protected override async Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            var metadata = await _getMetadata(input, context);

            return metadata == null
                ? input.Yield()
                : input.Clone(metadata).Yield();
        }
    }
}
