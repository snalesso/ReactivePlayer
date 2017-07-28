using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Services
{
    public interface IRenameArtistService
    {
        Task<Artist> RenameArtistAsync(Artist newName);
    }
}