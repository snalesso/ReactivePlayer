using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ReactivePlayer.Domain.Models
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class iTunesPlaylist
#pragma warning restore IDE1006 // Naming Styles
    {
        public const char iTunesFeaturingArtistsSplitter = '/';
        // TODO: handle AC/DC splitting
        //private readonly Hashtable<IReadOnlyList<string>, string>

        public PlaylistBase ToSimplePlaylist(Func<uint> newPlaylistIdGenerator, uint? parentPlaylistId, IDictionary<uint, Track> tracksMapper)
        {
            PlaylistBase playlist = null;

            try
            {
                playlist = new SimplePlaylist(
                    newPlaylistIdGenerator.Invoke(),
                    // library entry
                    parentPlaylistId,
                    this.Name,
                    tracksMapper == null || this.Playlist_Items == null
                        ? ImmutableList<uint>.Empty
                        : this.Playlist_Items.Select(x => tracksMapper[x].Id).ToImmutableList());
            }
            catch //(Exception ex)
            {
                playlist = null;
            }

            return playlist;
        }

        public PlaylistBase ToFolderPlaylist(Func<uint> newPlaylistIdGenerator, uint? parentPlaylistId, IEnumerable<iTunesPlaylist> iTunesPlaylists, IDictionary<uint, Track> tracksMapper)
        {
            PlaylistBase playlist = null;

            try
            {
                var playlistId = newPlaylistIdGenerator.Invoke();

                playlist = new FolderPlaylist(
                    playlistId,
                    // library entry
                    parentPlaylistId,
                    this.Name,
                    iTunesPlaylists == null
                        ? ImmutableList<PlaylistBase>.Empty
                        : iTunesPlaylists
                            .Where(x => x.Parent_Persistent_ID == this.Playlist_Persistent_ID)
                            .Select(x => x.ToPlaylist(newPlaylistIdGenerator, playlistId, iTunesPlaylists, tracksMapper))
                            .RemoveNulls()
                            .ToImmutableList());
            }
            catch //(Exception ex)
            {
                playlist = null;
            }

            return playlist;
        }

        public PlaylistBase ToPlaylist(Func<uint> newPlaylistIdGenerator, uint? parentPlaylistId, IEnumerable<iTunesPlaylist> iTunesPlaylists, IDictionary<uint, Track> tracksMapper)
        {
            if (this.Folder)
                return this.ToFolderPlaylist(newPlaylistIdGenerator, parentPlaylistId, iTunesPlaylists, tracksMapper);

            else //if (this.Smart_Criteria == null && this.Smart_Info == null)
                return this.ToSimplePlaylist(newPlaylistIdGenerator, parentPlaylistId, tracksMapper);

            //return null;
            //else
            //    throw new NotSupportedException();
        }

        public int Playlist_ID { get; set; }

        public string Playlist_Persistent_ID { get; set; }
        public string Parent_Persistent_ID { get; set; }

        public string Name { get; set; }

        public bool All_Items { get; set; }
        public bool Master { get; set; }
        public bool Folder { get; set; }
        public bool Visible { get; set; }

        public bool Audiobooks { get; set; }
        public bool Movies { get; set; }
        public bool Music { get; set; }
        public bool Podcasts { get; set; }
        public bool TV_Shows { get; set; }

        public int Distinguished_Kind { get; set; }

        public string Smart_Criteria { get; set; }
        public string Smart_Info { get; set; }

        public IReadOnlyList<uint> Playlist_Items { get; set; }

        //Folder: 1
        //Name: 51
        //Smart Criteria: 29
        //Audiobooks: 1
        //All Items: 1
        //Playlist Persistent ID: 55
        //Parent Persistent ID: 9
        //Playlist Items: 49
        //Distinguished Kind: 8
        //Music: 1
        //Podcasts: 1
        //Movies: 1
        //Visible: 1
        //TV Shows: 1
        //Playlist ID: 55
        //Master: 1
        //Smart Info: 5

        //<key>Playlist ID</key><integer>18216</integer>
        //<key>Playlist Persistent ID</key><string>321E1285672DF81A</string>
        //<key>All Items</key><true/>
        //<key>Name</key><string>Electro/Dance</string>
        //<key>Playlist Items</key>
        //<array>
        //	<dict>
        //		<key>Track ID</key><integer>3497</integer>
    }
}