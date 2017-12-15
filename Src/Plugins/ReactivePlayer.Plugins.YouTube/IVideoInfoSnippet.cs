using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube
{
    public interface IVideoInfoSnippet
    {
        string Id { get; }
        string Title { get; }
        string Description { get; }
        IReadOnlyList<string> Keywords { get; }
        string ImageThumbnailUrl { get; }
        string ImageMediumResUrl { get; }
        string ImageHighResUrl { get; }
        string ImageStandardResUrl { get; }
        string ImageMaxResUrl { get; }
        long ViewCount { get; }
        long LikeCount { get; }
        long DislikeCount { get; }
        double AverageRating { get; }
    }
}