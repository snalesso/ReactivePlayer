using System;

namespace ReactivePlayer.Core.FileSystem.Media.Audio
{
    public interface IAudioFileTagger
    {
        AudioFileTags ReadTags(Uri trackLocation);
        bool WriteTags(Uri trackLocation, TrackTags tags);
    }
}