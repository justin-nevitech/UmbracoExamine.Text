namespace UmbracoExamine.Text
{
    public interface ITextExtractor
    {
        string GetText(Stream fileStream);
    }
}