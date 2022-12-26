using TmCodeChallenge.Model.Entities;

namespace TmCodeChallenge.Model.Services.VolumeStreams;

public class TweetStreemedEventArgs : EventArgs
{
    public TweetStreemedEventArgs(Tweet tweet)
    {
        Tweet = tweet;
    }
    public Tweet Tweet { get; set; }
}
