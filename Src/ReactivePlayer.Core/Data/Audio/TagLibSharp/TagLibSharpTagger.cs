using System;
using ReactivePlayer.Domain.Entities;

namespace ReactivePlayer.Core.Data.Audio.TagLibSharp
{
    public sealed class TagLibSharpTagger : IAudioFileTagger
    {
        public AudioFileTags ReadTags(Uri trackLocation)
        {
            throw new NotImplementedException();
        }

        public bool WriteTags(Uri trackLocation, Tags tags)
        {
            throw new NotImplementedException();
        }
    }
}