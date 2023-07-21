using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Changes;
using Umbraco.Cms.Core.Sync;
using Umbraco.Extensions;

namespace UmbracoExamine.Text
{
    public class TextCacheNotificationHandler : INotificationHandler<MediaCacheRefresherNotification>
    {
        private readonly IMediaService _mediaService;
        private readonly TextIndexPopulator _textIndexPopulator;
        private readonly IRuntimeState _runtimeState;

        public TextCacheNotificationHandler(IMediaService mediaService, TextIndexPopulator textIndexPopulator, IRuntimeState runtimeState)
        {
            _mediaService = mediaService;
            _textIndexPopulator = textIndexPopulator;
            _runtimeState = runtimeState;
        }

        /// <summary>
        /// Handles the cache refresher event and updates the index.
        /// </summary>
        /// <param name="notification"></param>
        /// <exception cref="NotSupportedException"></exception>
        public void Handle(MediaCacheRefresherNotification notification)
        {
            // Only handle events when the site is running.
            if (_runtimeState.Level != RuntimeLevel.Run)
            {
                return;
            }

            if (notification.MessageType != MessageType.RefreshByPayload)
            {
                throw new NotSupportedException();
            }

            foreach (var payload in (MediaCacheRefresher.JsonPayload[])notification.MessageObject)
            {
                if (payload.ChangeTypes.HasType(TreeChangeTypes.Remove))
                {
                    _textIndexPopulator.RemoveFromIndex(payload.Id);
                }
                else if (payload.ChangeTypes.HasType(TreeChangeTypes.RefreshAll))
                {
                    // ExamineEvents does not support RefreshAll
                    // just ignore that payload
                    // so what?!
                }
                else // RefreshNode or RefreshBranch (maybe trashed)
                {
                    var media = _mediaService.GetById(payload.Id);

                    if (media is null)
                    {
                        // Gone fishing, remove entirely
                        _textIndexPopulator.RemoveFromIndex(payload.Id);
                        continue;
                    }

                    if (media.Trashed)
                    {
                        _textIndexPopulator.RemoveFromIndex(payload.Id);
                    }
                    else
                    {
                        _textIndexPopulator.AddToIndex(media);
                    }

                    // Branch
                    if (payload.ChangeTypes.HasType(TreeChangeTypes.RefreshBranch))
                    {
                        const int pageSize = 500;
                        var page = 0;
                        var total = long.MaxValue;
                        while (page * pageSize < total)
                        {
                            var descendants = _mediaService.GetPagedDescendants(media.Id, page++, pageSize, out total);
                            foreach (var descendant in descendants)
                            {
                                if (descendant.Trashed)
                                    _textIndexPopulator.RemoveFromIndex(descendant);
                                else
                                    _textIndexPopulator.AddToIndex(descendant);
                            }
                        }
                    }
                }
            }
        }
    }
}