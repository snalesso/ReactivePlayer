using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Services
{
    public abstract class CriteriaBase
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}