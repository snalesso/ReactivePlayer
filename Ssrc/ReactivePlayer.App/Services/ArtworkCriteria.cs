using ReactivePlayer.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Services
{
    public class ArtworkCriteria : Criteria
    {
        public ArtworkCriteria(uint pageIndex, uint pageSize, SortDirection sortDirection)
            : base(pageIndex, pageSize, sortDirection)
        {
        }
    }
}