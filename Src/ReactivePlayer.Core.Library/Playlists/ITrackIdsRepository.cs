using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Playlists
{
    //public interface IIdentitiesRepository<TIdentity>
    //    where TIdentity : IEquatable<TIdentity>
    //{
    //    Task<IReadOnlyList<TIdentity>> GetAll();

    //    Task Add(TIdentity identity);
    //    Task Remove(TIdentity identity);

    //    IObservable<IReadOnlyList<TIdentity>> Added { get; }
    //}

    public interface ITrackIdsRepository //: IIdentitiesRepository<uint>
    {
        Task<IReadOnlyList<uint>> GetAll();

        Task Add(uint trackId);
        Task Remove(uint trackId);

        IObservable<IReadOnlyList<uint>> TrackIdsAdded { get; }
        IObservable<IReadOnlyList<uint>> TrackIdsRemoved { get; }
    }
}