namespace TmCodeChallenge.Model.Services.VolumeStreams;

public class TopHashtagsChangedEventArgs : EventArgs
{
    public TopHashtagsChangedEventArgs(List<KeyValuePair<string, int>> hashtags)
    {
        TopHashtags = hashtags;
    }
    public List<KeyValuePair<string, int>> TopHashtags { get; set; }
}
