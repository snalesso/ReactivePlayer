using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Servicing
{
    public interface IConnectableService
    {
        bool IsConnected { get; }
        IObservable<bool> WhenIsConnectedChanged { get; }
        Task<bool> Connect();
    }
}