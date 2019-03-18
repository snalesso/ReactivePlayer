using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Services
{
    public interface IConnectableService
    {
        bool IsConnected { get; }
        IObservable<bool> WhenIsConnectedChanged { get; }
        Task<bool> Connect();
    }
}