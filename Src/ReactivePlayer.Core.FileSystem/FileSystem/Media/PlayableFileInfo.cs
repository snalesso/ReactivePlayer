using ReactivePlayer.Core.FileSystem.FileSystem.FileSystem.Media.Audio;
using System;

namespace ReactivePlayer.Core.FileSystem.FileSystem.FileSystem.Media
{
    // TODO: carry codec here?
    public class PlayableFileInfo : SimpleFileInfo
    {
        public PlayableFileInfo(
            Uri location,
            DateTime? lastModifiedDateTime,
            ulong? fileSizeBytes,
            TimeSpan? duration/*,
            uint bitsPerSecond,
            ushort? bitsPerSample,
            AudioChannels? channels*/)
            : base(location, lastModifiedDateTime, fileSizeBytes)
        {
            this.Duration = duration;
            //this.BitsPerSecond = bitsPerSecond;
            //this.BitsPerSample = bitsPerSample;
            //this.Channels = channels;
        }

        public PlayableFileInfo(
            SimpleFileInfo fileInfo,
            TimeSpan? duration,
            uint bitsPerSecond,
            ushort? bitsPerSample
            //, AudioChannels? channels
            )
            : this(fileInfo.Location, fileInfo.LastModifiedDateTime, fileInfo.SizeBytes, duration/*, bitsPerSecond, bitsPerSample, channels*/)
        {
        }

        public TimeSpan? Duration { get; }
        //public uint? BitsPerSecond { get; }
        //public ushort? BitsPerSample { get; }
        //public AudioChannels? Channels { get; }
    }
}