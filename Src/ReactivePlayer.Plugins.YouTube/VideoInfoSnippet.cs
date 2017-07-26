using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube
{
    public class VideoInfoSnippet //: IVideoInfoSnippet
    {
        internal VideoInfoSnippet(
            string id,
            string title,
            string description,
            IReadOnlyList<string> keywords,
            //string imageThumbnailUrl,
            //string imageMediumResUrl,
            //string imageHighResUrl,
            //string imageStandardResUrl,
            string imageMaxResUrl,
            long viewCount,
            long likeCount,
            long dislikeCount
            //double averageRating
            )
        {
            this.Id = id;
            this.Title = title;
            this.Description = description;
            this.Keywords = keywords;
            //this.ImageThumbnailUrl = imageThumbnailUrl;
            //this.ImageMediumResUrl = imageMediumResUrl;
            //this.ImageStandardResUrl = imageStandardResUrl;
            this.ImageMaxResUrl = imageMaxResUrl;
            this.ViewCount = viewCount;
            this.LikeCount = likeCount;
            this.DislikeCount = dislikeCount;
            //this.AverageRating = averageRating;
        }

        public string Id { get; }
        public string Title { get; }
        public string Description { get; }
        public IReadOnlyList<string> Keywords { get; }
        public string ImageThumbnailUrl { get; }
        public string ImageMediumResUrl { get; }
        public string ImageHighResUrl { get; }
        public string ImageStandardResUrl { get; }
        public string ImageMaxResUrl { get; }
        public long ViewCount { get; }
        public long LikeCount { get; }
        public long DislikeCount { get; }
        //public double AverageRating { get; }
    }
}