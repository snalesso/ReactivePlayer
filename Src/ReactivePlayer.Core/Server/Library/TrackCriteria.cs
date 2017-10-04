using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Data
{
    public class TrackCriteria
    {
        public string Title { get; set; }
        public string PerformerName { get; set; }
        public string AlbumName { get; set; }

        public bool IsRespectedBy(Track track) => true;
    }
}