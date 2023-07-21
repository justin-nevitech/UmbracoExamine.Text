using System.Linq;
using Examine;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Extensions;

namespace UmbracoExamine.Text
{
    public static class BuilderExtensions
    {
        public static IUmbracoBuilder AddExamineText(this IUmbracoBuilder builder)
        {
            if (builder.Services.Any(x => x.ServiceType == typeof(ITextExtractor)))
            {
                // Assume that Examine.Text is already composed if any implementation of ITextExtractor is registered.
                return builder;
            }

            //Register the services used to make this all work
            builder.Services.AddUnique<ITextExtractor, TextExtractor>();
            builder.Services.AddSingleton<TextService>();
            builder.Services.AddUnique<ITextIndexValueSetBuilder, TextIndexValueSetBuilder>();
            builder.Services.AddSingleton<IIndexPopulator, TextIndexPopulator>();
            builder.Services.AddSingleton<TextIndexPopulator>();

            builder.Services
                .AddExamineLuceneIndex<TextLuceneIndex, ConfigurationEnabledDirectoryFactory>(TextIndexConstants.TextIndexName)
                .ConfigureOptions<ConfigureTextIndexOptions>();

            builder.AddNotificationHandler<MediaCacheRefresherNotification, TextCacheNotificationHandler>();

            return builder;
        }
    }
}
