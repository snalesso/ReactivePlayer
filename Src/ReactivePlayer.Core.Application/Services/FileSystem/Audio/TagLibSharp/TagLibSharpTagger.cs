using ReactivePlayer.Core.Domain.Library.Models;
using System;

namespace ReactivePlayer.Core.Application.Services.FileSystem.Audio.TagLibSharp
{
    public sealed class TagLibSharpTagger : IAudioFileTagger
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