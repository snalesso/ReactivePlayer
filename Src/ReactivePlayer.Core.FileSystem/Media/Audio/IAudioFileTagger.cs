using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.FileSystem.Media.Audio
{
    public interface IAudioFileTagger
    {
        Task<AudioFileTags> ReadTagsAsync(Uri trackLocation);
        Task<bool> WriteTags(Uri trackLocation, TrackTags tags);
    }
}