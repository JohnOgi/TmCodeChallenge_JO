namespace TmCodeChallenge.Model.Services.VolumeStreams;

public interface ITwitterVolumeService
{
    event ErrorEventHandler OnError;
    event TwitterVolumnService.TopHashtagsChangedHandler? OnTopHashtagsChanged;
    event TwitterVolumnService.TweetStreemedHandler? OnTweetStreamed;

    void ExtractTweet(string text);
    Task<bool> StartProcessing();
    void StopProcessing();
}
