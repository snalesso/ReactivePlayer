using ReactivePlayer.Domain.Models;
using System;

namespace ReactivePlayer.Core.Data.IO.Audio
{
    public interface IAudioFileTagger
    {
        AudioFileTags ReadTags(Uri trackLocation);
        bool WriteTags(Uri trackLocation, Tags tags);
    }
}