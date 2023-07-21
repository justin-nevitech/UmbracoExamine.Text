using Examine.Lucene;
using Examine.Lucene.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Infrastructure.Examine;

namespace UmbracoExamine.Text
{
    public class TextLuceneIndex : LuceneIndex, IIndexDiagnostics
    {
        public TextLuceneIndex(ILoggerFactory loggerFactory, string name, IOptionsMonitor<LuceneDirectoryIndexOptions> indexOptions, IHostingEnvironment hostingEnvironment)
            : base(loggerFactory, name, indexOptions)
        {
            _diagnostics = new TextIndexDiagnostics(this, loggerFactory.CreateLogger<LuceneIndexDiagnostics>(), hostingEnvironment, indexOptions);
        }

        #region IIndexDiagnostics

        private readonly IIndexDiagnostics _diagnostics;

        public long DocumentCount => _diagnostics.GetDocumentCount();

        public int FieldCount => _diagnostics.GetFieldNames().Count();

        public Attempt<string?> IsHealthy() => _diagnostics.IsHealthy();

        public virtual IReadOnlyDictionary<string, object?> Metadata => _diagnostics.Metadata;

        #endregion
    }
}