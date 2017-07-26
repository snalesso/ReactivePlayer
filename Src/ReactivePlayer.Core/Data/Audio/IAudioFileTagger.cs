using ReactivePlayer.Domain.Entities;
using System;

namespace ReactivePlayer.Core.Data.Audio
{
    public interface IAudioFileTagger
    {
        AudioFileTags ReadTags(Uri trackLocation);
        bool WriteTags(Uri trackLocation, Tags tags);
    }
}