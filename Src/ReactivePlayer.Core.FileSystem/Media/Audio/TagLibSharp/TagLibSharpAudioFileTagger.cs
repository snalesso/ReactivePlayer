using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.FileSystem.Media.Audio.TagLibSharp
{
    public sealed class TagLibSharpAudioFileTagger : IAudioFileTagger
    {
        public TagLibSharpAudioFileTagger()
        {
        }

        public async Task<AudioFileTags> ReadTagsAsync(Uri trackLocation)
        {
            var tagLibFile = await Task.Run(() => TagLib.File.Create(trackLocation.LocalPath));
            var tagLibTags = tagLibFile.Tag;

            if (tagLibTags == null)
                return null;

            var aft = new AudioFileTags(
                tagLibTags.Title,
                tagLibTags.Performers,
                tagLibTags.Composers,
                tagLibTags.Year,

                tagLibTags.Album,
                tagLibTags.AlbumArtists,
                tagLibTags.Track,
                tagLibTags.TrackCount,
                tagLibTags.Disc,
                tagLibTags.DiscCount);

            return aft;
        }

        public Task<bool> WriteTags(Uri trackLocation, TrackTags tags)
        {
            throw new NotImplementedException();
        }
    }
}