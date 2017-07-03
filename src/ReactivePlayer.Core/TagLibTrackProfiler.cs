using ReactivePlayer.Core.DTOs;
using System;

namespace ReactivePlayer.Core.Services
{
    public sealed class TagLibTrackProfiler : ITrackProfiler
    {
        public TrackDto GetTrack(Uri trackLocation)
        {
            // TODO: check Uri.IsWellFormed ... ?

            if (trackLocation == null)
                throw new ArgumentNullException(nameof(trackLocation));

            if (!trackLocation.IsFile)
                throw new System.IO.FileNotFoundException(); // TODO: use a more descriptive exception

            if (!System.IO.File.Exists(trackLocation.LocalPath))
                throw new System.IO.FileNotFoundException();

            TrackDto trackProfile = null;

            try
            {
                using (var trackFile = TagLib.File.Create(trackLocation.LocalPath))
                {
                    var tags = trackFile.Tag;
                    TimeSpan? duration = ShellObjectHelper.GetMediaFileDuration(trackLocation.LocalPath);

                    trackProfile = new TrackDto(trackLocation)
                    {
                        Duration = duration,
                        Tags = new TagsDto(trackFile.Tag)
                    };
                }
            }
            catch (Exception)
            {
                trackProfile = null;
            }

            return trackProfile;
        }
    }
}