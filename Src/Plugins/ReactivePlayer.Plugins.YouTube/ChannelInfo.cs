using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube.YouTubeExplode.Models
{
    public class ChannelInfo 
    {
        internal ChannelInfo(
            string id,
            string url,
            string name,
            string title
            //bool isPaid,
            //string logoUrl,
            //string bannerUrl
            )
        {
            this.Id = id;
            this.Url = url;
            this.Name = name;
            this.Title = title;
            //this.IsPaid = isPaid;
            //this.LogoUrl = logoUrl;
            //this.BannerUrl = bannerUrl;
        }

        public string Id { get; }
        public string Url { get; }
        public string Name { get; }
        public string Title { get; }
        //public bool IsPaid { get; }
        //public string LogoUrl { get; }
        //public string BannerUrl { get; }
    }
}