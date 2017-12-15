using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        void Rollback();
        bool Commit();
    }
}