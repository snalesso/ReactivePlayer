using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.TaskFlow
{
    public interface ILibraryTask<TIn, TOut>
    {
        Task<TOut> Execute(TIn arg);
    }
}