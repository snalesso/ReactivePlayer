using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Models
{
    internal sealed class CustomJsonArtistConverter : CustomJsonConverter<Artist>
    {
        public override void ToJson(Artist realObject, StringBuilder stringBuilder)
        {
            if (stringBuilder == null)
                throw new ArgumentNullException(nameof(stringBuilder));

            
        }

        public override Artist ToObject(string jsonObject)
        {
            throw new NotImplementedException();
        }
    }
}