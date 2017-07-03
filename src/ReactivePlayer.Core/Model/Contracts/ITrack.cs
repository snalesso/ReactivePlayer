using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ReactivePlayer.Core.Model.Contracts
{
    public interface ITrack
    {
        #region library info

        DateTime AddedToLibraryDateTime { get; }
        IList<DateTime> PlaysHistory { get; set; }

        #endregion

        #region physical info

        string Location { get; }
        TimeSpan Duration { get; set; }
        DateTime LastModifiedDateTime { get; set; }
        ulong FileSize_B { get; set; }
        uint Bitrate_bps { get; set; }
        uint SampleRate_Hz { get; set; }
        uint SampleSize_b { get; set; }

        #endregion

        #region tags

        string Title { get; set; }
        string Lyrics { get; set; }
        IList<IArtist> Performers { get; set; }
        IAlbum Album { get; set; }
        IList<IArtist> Composers { get; set; }
        IList<IArtwork> Artworks { get; set; }

        #endregion
    }
}