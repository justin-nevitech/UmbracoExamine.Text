using Examine;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Infrastructure.Examine;

namespace UmbracoExamine.Text
{
    public interface ITextIndexValueSetBuilder : IValueSetBuilder<IMedia> { }

    /// <summary>
    /// Builds a ValueSet for Text Documents
    /// </summary>
    public class TextIndexValueSetBuilder : ITextIndexValueSetBuilder
    {
        private TextService _textService;
        private readonly ILogger<TextIndexValueSetBuilder> _logger;

        public TextIndexValueSetBuilder(TextService textService, ILogger<TextIndexValueSetBuilder> logger)
        {
            _textService = textService;
            _logger = logger;
        }
        public IEnumerable<ValueSet> GetValueSets(params IMedia[] content)
        {
            foreach (var item in content)
            {
                var umbracoFile = item.GetValue<string>(Constants.Conventions.Media.File);
                if (string.IsNullOrWhiteSpace(umbracoFile)) continue;

                string fileTextContent;
                try
                {
                    fileTextContent = ExtractTextFromFile(umbracoFile);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not read the file {MediaFile}", umbracoFile);
                    continue;
                }
                var indexValues = new Dictionary<string, object>
                {
                    ["nodeName"] = item.Name!,
                    ["id"] = item.Id,
                    ["path"] = item.Path,
                    [TextIndexConstants.TextContentFieldName] = fileTextContent
                };

                var valueSet = new ValueSet(item.Id.ToString(), TextIndexConstants.TextCategory, item.ContentType.Alias, indexValues);

                yield return valueSet;
            }
        }

        private string ExtractTextFromFile(string filePath)
        {
            try
            {
                return _textService.ExtractText(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not extract text from file {TextFilePath}", filePath);
                return String.Empty;
            }
        }
    }
}