namespace ReactivePlayer.App
{
    public interface ITrackTagger
    {
        Tags ReadTags(string filePath);

        bool WriteTags(string filePath);
    }
}