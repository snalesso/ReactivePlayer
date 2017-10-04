using ReactivePlayer.Core.Domain.Library.Models;
using System;

namespace ReactivePlayer.Core.Application.Services.FileSystem.Audio
{
    public interface IAudioFileTagger
    {
        AudioFileTags ReadTags(Uri trackLocation);
        bool WriteTags(Uri trackLocation, TrackTags tags);
    }
}