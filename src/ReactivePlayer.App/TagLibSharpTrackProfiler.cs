using System;

namespace ReactivePlayer.App
{
    public sealed class TagLibSharpTrackProfiler : ITrackProfiler
    {
        public TrackProfile GetTrack(Uri trackLocation)
        {
            // TODO: check Uri.IsWellFormed ... ?

            if (trackLocation == null)
                throw new ArgumentNullException(nameof(trackLocation));

            if (!trackLocation.IsFile)
                throw new System.IO.FileNotFoundException(); // TODO: use a more descriptive exception

            if (!System.IO.File.Exists(trackLocation.LocalPath))
                throw new System.IO.FileNotFoundException();

            TrackProfile trackProfile = null;

            try
            {
                using (var trackFile = TagLib.File.Create(trackLocation.LocalPath))
                {
                    var tags = trackFile.Tag;
                    TimeSpan? duration = ShellObjectHelper.GetMediaFileDuration(trackLocation.LocalPath);

                    trackProfile = new TrackProfile(trackLocation)
                    {
                        Duration = duration,
                        Tags = new Tags()
                        {
                            // TODO: implement
                        }
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