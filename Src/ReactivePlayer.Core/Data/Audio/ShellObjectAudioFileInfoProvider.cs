using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Data.Audio
{
    public sealed class ShellObjectAudioFileInfoProvider : IAudioFileInfoProvider
    {
        public Task<AudioFileInfo> ExtractAudioFileInfo(Uri trackLocation)
        {
            // TODO: check Uri.IsWellFormed ... ?

            if (trackLocation == null)
                throw new ArgumentNullException(nameof(trackLocation));

            if (!trackLocation.IsFile)
                throw new System.IO.FileNotFoundException(); // TODO: use a more descriptive exception

            if (!System.IO.File.Exists(trackLocation.LocalPath))
                throw new System.IO.FileNotFoundException();

            AudioFileInfo trackProfile = null;

            try
            {
                using (var trackFile = TagLib.File.Create(trackLocation.LocalPath))
                {
                    var tags = trackFile.Tag;
                    TimeSpan? duration = ShellObjectHelper.GetMediaFileDuration(trackLocation.LocalPath);

                    trackProfile = new AudioFileInfo(trackLocation);
                }
            }
            catch (Exception)
            {
                trackProfile = null;
            }

            return trackProfile;
        }

        public bool IsHostSupported(Uri location)
        {
            throw new NotImplementedException();
        }
    }
}