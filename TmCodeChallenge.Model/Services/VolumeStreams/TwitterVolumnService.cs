using System.Collections.Concurrent;
using System.Data;
using System.Timers;
using TmCodeChallenge.Model.Entities;
using Timer = System.Timers.Timer;

namespace TmCodeChallenge.Model.Services.VolumeStreams;

public class TwitterVolumnService : ITwitterVolumeService
{
    internal ILongRequester requester;
    internal readonly ConcurrentDictionary<string, int> hashtags = new ConcurrentDictionary<string, int>();
    internal List<KeyValuePair<string, int>> topHashtags = new List<KeyValuePair<string, int>>();
    private Timer? timer;

    //Events
    public delegate void TweetStreemedHandler(object sender, TweetStreemedEventArgs e);
    public event TweetStreemedHandler? OnTweetStreamed;
    public delegate void TopHashtagsChangedHandler(object sender, TopHashtagsChangedEventArgs e);
    public event TopHashtagsChangedHandler? OnTopHashtagsChanged;
    public event ErrorEventHandler? OnError;

    private void RaiseTopHashtagsChanged() =>
        OnTopHashtagsChanged?.Invoke(this, new TopHashtagsChangedEventArgs(topHashtags));

    private void StartProcessingHashtags()
    {
        timer = new Timer(2000) { AutoReset = true };
        timer.Elapsed += (s, a) => ProcessTopHashtags();
        timer.Enabled = true;
    }

    private void StopProcessingHashtags()
    {
        if (timer != null) timer.Enabled = false;
        timer?.Dispose();
        timer = null;
    }

    public TwitterVolumnService(ILongRequester requester)
    {
        this.requester = requester;
    }

    internal void ProcessTopHashtags()
    {
        var currentTopHashtags = hashtags.OrderBy(x => x.Key).OrderByDescending(x => x.Value).Take(10).ToList();
        if (!currentTopHashtags.Any()) return;

        if (topHashtags.Any())
        {
            var difference = topHashtags.Except(currentTopHashtags).ToList();
            if (difference.Any())
            {
                topHashtags = currentTopHashtags;
                RaiseTopHashtagsChanged();
            }
        }
        else
        {
            topHashtags = currentTopHashtags;
            RaiseTopHashtagsChanged();
        }
    }

    public void ExtractTweet(string text)
    {
        var tweet = Tweet.Parse(text);
        if (tweet == null) return;

        tweet.Hashtags?.ForEach(ht => hashtags.AddOrUpdate(ht, 1, (key, count) => ++count));
        OnTweetStreamed?.Invoke(this, new TweetStreemedEventArgs(tweet));
        //SaveTweet(tweet);
    }

    [Obsolete("This method would add to a queue where a worker would batch process tweets for storage somewhere.")]
#pragma warning disable IDE0051 // Remove unused members
    internal void SaveTweet(Tweet tweet)
#pragma warning restore IDE0051 // Remove unused members
    {
        throw new NotImplementedException();
    }

    public void StopProcessing()
    {
        StopProcessingHashtags();
        requester.Cancel();
    }

    public async Task<bool> StartProcessing()
    {
        await Task.Run(async () =>
        {
            try
            {
                StartProcessingHashtags();
                await requester.GetData(ExtractTweet);
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, new ErrorEventArgs(e));
            }
            finally
            {
                StopProcessingHashtags();
            }
        });
        return true;
    }
}
