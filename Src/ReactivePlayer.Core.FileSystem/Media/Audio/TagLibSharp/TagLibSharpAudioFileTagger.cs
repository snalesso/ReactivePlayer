using System;

namespace ReactivePlayer.Core.FileSystem.Media.Audio.TagLibSharp
{
    public sealed class TagLibSharpAudioFileTagger : IAudioFileTagger
    {
        public TagLibSharpAudioFileTagger()
        {
        }

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