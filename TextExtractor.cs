using System.Text;

namespace UmbracoExamine.Text
{
    public class TextExtractor : ITextExtractor
    {
        public string GetText(Stream fileStream)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                var result = new StringBuilder();

                return reader.ReadToEnd();
            }
        }
    }
}