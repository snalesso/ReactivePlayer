using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube
{
    public enum VideoQuality
    {
        //
        // Summary:
        //     144p low-quality video stream
        Low144 = 0,
        //
        // Summary:
        //     240p low-quality video stream
        Low240 = 1,
        //
        // Summary:
        //     360p medium-quality video stream
        Medium360 = 2,
        //
        // Summary:
        //     480p medium-quality video stream
        Medium480 = 3,
        //
        // Summary:
        //     720p HD video stream
        High720 = 4,
        //
        // Summary:
        //     1080p HD video stream
        High1080 = 5,
        //
        // Summary:
        //     1440p HD video stream
        High1440 = 6,
        //
        // Summary:
        //     2160p HD video stream
        High2160 = 7,
        //
        // Summary:
        //     3072p HD video stream
        High3072 = 8,
        //
        // Summary:
        //     4320p HD video stream
        High4320 = 9
    }
}