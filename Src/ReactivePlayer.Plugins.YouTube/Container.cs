using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube
{
    public enum Container
    {
        //
        // Summary:
        //     MPEG-4 Part 14 (.mp4)
        Mp4 = 0,
        //
        // Summary:
        //     MPEG-4 Part 14 audio-only (.m4a)
        M4A = 1,
        //
        // Summary:
        //     Web Media (.webm)
        WebM = 2,
        //
        // Summary:
        //     3rd Generation Partnership Project (.3gpp)
        Tgpp = 3,
        //
        // Summary:
        //     Flash Video (.flv)
        Flv = 4
    }
}