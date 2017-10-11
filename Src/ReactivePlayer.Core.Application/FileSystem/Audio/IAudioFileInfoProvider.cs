using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Application.FileSystem.Audio
{
    public interface IAudioFileInfoProvider // TODO: review name: find something that means both reader & writer
    {
        bool IsHostSupported(Uri location);
        Task<AudioFileInfo> ExtractAudioFileInfo(Uri trackLocation);
    }
}