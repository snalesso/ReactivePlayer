using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
//using ReactivePlayer.Core.Entities;
//using System.ComponentModel.DataAnnotations;
//using SQLite;

namespace ReactivePlayer.Core.Model
{
    /// <summary>
    /// The base class for the any representation of an audio track.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Title) + "} - {" + nameof(Album) + "}")]
    public sealed class ObservableTrack : ObservableEntityBase, ITrack
    {
        #region ctors

        //public AudioTrack(uint id, AudioTrack audioTrack)
        //{
        //    var props = typeof(AudioTrack)
        //        .GetProperties(
        //            System.Reflection.BindingFlags.Public
        //            | System.Reflection.BindingFlags.Instance
        //            | System.Reflection.BindingFlags.SetProperty)
        //        .ToArray();

        //    foreach (var prop in props)
        //    {
        //        prop.SetValue(this, prop.GetValue(audioTrack));
        //    }

        //    this.Id = id;
        //}

        ///// <summary>
        ///// Creates a new <see cref="AudioTrack"/> with the specified ID.
        ///// </summary>
        ///// <param name="id">The id of the audio track in the library.</param>
        //public AudioTrack(uint id)
        //{
        //    this.Id = id;
        //}

        /// <summary>
        /// Creates a new instance of <see cref="ObservableTrack"/>.
        /// </summary>
        public ObservableTrack()
        {
        }

        #endregion

        #region props

        // TODO add tag format

        private uint _id;
        /// <summary>
        /// The identifier of the audio track in the library.
        /// </summary>
        public uint Id
        {
            get { return this._id; }
            set
            {
                if (this._id > 0)
                    throw new InvalidOperationException("An" + nameof(ObservableTrack) + "'s " + nameof(Id) + " can not be changed once set.");
                else
                    this._id = value;
            }
        }

        private Uri _location;
        /// <summary>
        /// The path to where the track is located.
        /// </summary>
        public Uri Location
        {
            get { return this._location; }
            set { this.SetAndRaiseIfChanged(ref this._location, value); }
        }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get { return this._duration; }
            set { this.SetAndRaiseIfChanged(ref this._duration, value); }
        }

        #region ID3 Tags

        private IEnumerable<IEnumerable<byte>> _Pictures;
        /// <summary>
        /// The keys of the cached pictures associated with this track.
        /// </summary>
        public IEnumerable<IEnumerable<byte>> Pictures
        {
            get { return this._Pictures; }
            set { this.SetAndRaiseIfChanged(ref this._Pictures, value); }
        }

        private IEnumerable<string> _Performers;
        /// <summary>
        /// The names of the artists performing in this audio.
        /// </summary>
        public IEnumerable<string> Performers
        {
            get { return this._Performers; }
            set { this.SetAndRaiseIfChanged(ref this._Performers, value); }
        }

        private string _Album;
        /// <summary>
        /// The name of the album.
        /// </summary>
        public string Album
        {
            get { return this._Album; }
            set { this.SetAndRaiseIfChanged(ref this._Album, value); }
        }

        private string _Title;
        /// <summary>
        /// The title of the audio.
        /// </summary>
        public string Title
        {
            get { return this._Title; }
            set { this.SetAndRaiseIfChanged(ref this._Title, value); }
        }

        private IEnumerable<string> _AlbumArtists;
        /// <summary>
        /// The artists authors of the album.
        /// </summary>
        public IEnumerable<string> AlbumArtists
        {
            get { return this._AlbumArtists; }
            set { this.SetAndRaiseIfChanged(ref this._AlbumArtists, value); }
        }

        private IEnumerable<string> _Composers;
        /// <summary>
        /// The artists that composed the track.
        /// </summary>
        public IEnumerable<string> Composers
        {
            get { return this._Composers; }
            set { this.SetAndRaiseIfChanged(ref this._Composers, value); }
        }

        private uint _TrackNumber;
        /// <summary>
        /// The number of the track in the album.
        /// </summary>
        public uint TrackNumber
        {
            get { return this._TrackNumber; }
            set { this.SetAndRaiseIfChanged(ref this._TrackNumber, value); }
        }

        private uint _TracksCount;
        /// <summary>
        /// The number of tracks in the album.
        /// </summary>
        public uint TracksCount
        {
            get { return this._TracksCount; }
            set { this.SetAndRaiseIfChanged(ref this._TracksCount, value); }
        }

        private uint _Year;
        /// <summary>
        /// The year the track was published.
        /// </summary>
        public uint Year
        {
            get { return this._Year; }
            set { this.SetAndRaiseIfChanged(ref this._Year, value); }
        }

        private string _Lyrics;
        /// <summary>
        /// The text of the words contained in this track.
        /// </summary>
        public string Lyrics
        {
            get { return this._Lyrics; }
            set { this.SetAndRaiseIfChanged(ref this._Lyrics, value); }
        }

        private string _Comment;
        /// <summary>
        /// The comment attached to the track.
        /// </summary>
        public string Comment
        {
            get { return this._Comment; }
            set { this.SetAndRaiseIfChanged(ref this._Comment, value); }
        }

        public DateTime AddedToLibraryDateTime => throw new NotImplementedException();

        public IList<DateTime> PlaysHistory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        string ITrack.Location => throw new NotImplementedException();

        public DateTime LastModifiedDateTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ulong FileSize_B { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint Bitrate_bps { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint SampleRate_Hz { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint SampleSize_b { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IList<string> ITrack.Performers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IList<string> ITrack.Composers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string AlbumName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort AlbumYear { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort AlbumTrackNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort AlbumTracksCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IList<string> AlbumPublisherArtists { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #endregion
    }
}