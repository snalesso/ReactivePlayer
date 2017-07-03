using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ReactivePlayer.Core.Entities
{
    public interface ITrack
    {
        #region properties

        #region library info

        long Id { get; set; }
        long AddedDateTimeTicks { get; set; }
        bool IsLoved { get; set; }

        #endregion

        #region physical info

        string Location { get; set; }
        long DurationTicks { get; set; }
        long SyncDateTimeTicks { get; set; }
        //long FileSize_B { get; set; }
        //uint Bitrate_bps { get; set; }
        //uint SampleRate_Hz { get; set; }
        //uint SampleSize_b { get; set; }

        #endregion

        #region tags

        string Title { get; set; }
        long? ArtistId { get; set; }
        long? AlbumId { get; set; }
        string Lyrics { get; set; }
        short? AlbumTrackNumber { get; set; }
        short? AlbumDiscNumber { get; set; }

        #endregion

        #endregion

        #region methods

        //ITrack Create(string location);

        #endregion
    }
}