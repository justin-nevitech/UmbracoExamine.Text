using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace UmbracoExamine.Text
{
    /// <summary>
    /// Registers the ExamineText index, and dependencies.
    /// </summary>
    public class ExamineTextComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder) => builder.AddExamineText();
    }
}