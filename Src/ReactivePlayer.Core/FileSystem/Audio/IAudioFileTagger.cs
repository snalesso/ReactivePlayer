using ReactivePlayer.Domain.Models;
using System;

namespace ReactivePlayer.Core.Data.FileSystem.Audio
{
    public interface IAudioFileTagger
    {
        AudioFileTags ReadTags(Uri trackLocation);
        bool WriteTags(Uri trackLocation, TrackTags tags);
    }
}