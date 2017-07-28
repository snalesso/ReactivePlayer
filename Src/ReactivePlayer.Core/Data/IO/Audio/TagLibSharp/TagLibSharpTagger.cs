using System;
using ReactivePlayer.Domain.Models;

namespace ReactivePlayer.Core.Data.IO.Audio.TagLibSharp
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