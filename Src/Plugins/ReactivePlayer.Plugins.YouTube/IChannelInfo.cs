using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube
{
    public interface IChannelInfo
    {
        string Id { get; }
        string Url { get; }
        string Name { get; }
        string Title { get; }
        bool IsPaid { get; }
        string LogoUrl { get; }
        string BannerUrl { get; }
    }
}