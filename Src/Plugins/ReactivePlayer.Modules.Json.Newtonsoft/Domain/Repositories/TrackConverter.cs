namespace ReactivePlayer.Modules.Json.Newtonsoft.Domain.Repositories
{
    public sealed class TrackConverter //: JsonConverter
    {
        //    public override bool CanConvert(Type objectType)
        //    {
        //        return objectType != null && objectType == typeof(Album);
        //    }

        //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        //    {
        //        var jo = JObject.Load(reader);

        //        var jAuthors = jo[nameof(Album.Authors)];
        //        var authors = jAuthors.Select(ja => ja.ToArtist()).ToArray();

        //        return new Album(
        //            (string)jo[nameof(Album.Name)],
        //            authors,
        //            (DateTime?)jo[nameof(Album.ReleaseDate)],
        //            (uint?)jo[nameof(Album.TracksCount)],
        //            (uint?)jo[nameof(Album.DiscsCount)]);
        //    }

        //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        //    {
        //        var album = (value as Album) ?? throw new InvalidCastException(); // TODO: localize

        //        var jAlbum = new JObject();
        //        jAlbum[nameof(Album.Name)] = album.Name;
        //        var jAuthors = album.Authors.Select(a => a.ToJObject()).ToArray();
        //        jAlbum[nameof(Album.Authors)] = new JArray(jAuthors);
        //        jAlbum[nameof(Album.ReleaseDate)] = album.ReleaseDate;
        //        jAlbum[nameof(Album.TracksCount)] = album.TracksCount;
        //        jAlbum[nameof(Album.DiscsCount)] = album.DiscsCount;
        //        serializer.Serialize(writer, jAlbum);
        //    }
    }
}