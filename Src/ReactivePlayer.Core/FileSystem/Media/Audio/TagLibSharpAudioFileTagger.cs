using ReactivePlayer.Core.Library.Models;
using System;

namespace ReactivePlayer.Core.FileSystem.Media.Audio
{
    public sealed class TagLibSharpAudioFileTagger : IAudioFileTagger
    {
        public AudioFileTags ReadTags(Uri trackLocation)
        {
            throw new NotImplementedException();
        }

        public bool WriteTags(Uri trackLocation, TrackTags tags)
        {
            throw new NotImplementedException();
        }
    }
}