using ReactivePlayer.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Services
{
    public abstract class Criteria
    {
        protected Criteria(
            uint pageIndex,
            uint pageSize,
            SortDirection sortDirection)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.SortDirection = sortDirection;
        }

        public uint PageIndex { get; }
        public uint PageSize { get; }
        public SortDirection SortDirection { get; }
    }
}