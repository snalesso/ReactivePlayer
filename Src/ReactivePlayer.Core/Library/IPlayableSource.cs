using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library
{
    public interface IPlayableSource
    {
        Uri Location { get; }
    }
}