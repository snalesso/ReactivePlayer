using System;

namespace ReactivePlayer.Core.Model.Contracts
{
    public interface IAlbum
    {
        string Name { get; set; }
        DateTime? ReleaseDate { get; set; }
        ushort? TracksCount { get; set; }
        ushort? DiscsCount { get; set; }
    }
}