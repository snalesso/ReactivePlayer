using ReactivePlayer.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Services
{
    public interface IArtistsService
    {
        Task<Artist> GetArtistsAsync(Artist artist, string newName);
        Task<Artist> RenameArtistAsync(Artist artist, string newName);
    }
}