using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.IO;

namespace UmbracoExamine.Text
{
    /// <summary>
    /// Extracts the text from a Text document
    /// </summary>
    public class TextService
    {
        private readonly ITextExtractor _textExtractor;
        private readonly MediaFileManager _mediaFileSystem;
        private readonly ILogger<TextService> _logger;

        public TextService(
            ITextExtractor textExtractor,
            MediaFileManager mediaFileSystem,
            ILogger<TextService> logger)
        {
            _textExtractor = textExtractor;
            _mediaFileSystem = mediaFileSystem;
            _logger = logger;
        }

        /// <summary>
        /// Extract text from a Text file at the given path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string ExtractText(string filePath)
        {
            using (var fileStream = _mediaFileSystem.FileSystem.OpenFile(filePath))
            {
                if (fileStream != null)
                {
                    return _textExtractor.GetText(fileStream);
                }
                else
                {
                    _logger.LogError(new Exception($"Unable to open file {filePath}"), "Unable to open file");
                    return String.Empty;
                }
            }
        }
    }
}