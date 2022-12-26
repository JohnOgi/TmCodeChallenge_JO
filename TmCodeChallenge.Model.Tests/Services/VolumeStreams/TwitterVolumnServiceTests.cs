using Moq;
using System.Collections.Concurrent;
using TmCodeChallenge.Model.Services.VolumeStreams;

namespace TmCodeChallenge.Model.Tests.Services.VolumeStreams;

[TestClass]
public class TwitterVolumnServiceTests
{
    private TwitterVolumnService? service;

    Mock<ILongRequester> mockLongRequester = new Mock<ILongRequester>();

    [TestInitialize]
    public void Setup()
    {
        mockLongRequester = new Mock<ILongRequester>();
        service = new TwitterVolumnService(mockLongRequester.Object);
    }

    [TestMethod]
    public void ExtractTweet_should_add_hashtags_to_dictionary()
    {
        var tweetString = $@"{{""data"":{{""edit_history_tweet_ids"":[""1606997204019077121""],""id"":""1606997204019077121"",""text"":""test #hash #testhashtag ""}}}}";

        service!.ExtractTweet(tweetString);

        Assert.AreEqual(2, service.hashtags.Count);
    }

    [TestMethod]
    public void ExtractTweet_should_raise_onTweetStreamed_event()
    {
        var tweetString = $@"{{""data"":{{""edit_history_tweet_ids"":[""1606997204019077121""],""id"":""1606997204019077121"",""text"":""test #testhashtag ""}}}}";
        var eventRaised = false;
        service!.OnTweetStreamed += (o, a) => eventRaised = true;


        service!.ExtractTweet(tweetString);

        Assert.IsTrue(eventRaised);
    }

    [TestMethod]
    public void ProcessTopHashtags_ShouldNotRaiseOnTopHashtagsChangedIfThereAreNoHashtags()
    {
        var eventRaised = false;
        service!.OnTopHashtagsChanged += (s, a) => eventRaised = true;
        service!.ProcessTopHashtags();

        Assert.IsFalse(eventRaised);
    }

    [TestMethod]
    public void ProcessTopHashtags_ShouldNotRaiseOnTopHashtagsChangedIfThereAreNoNewHashtags()
    {
        bool eventRaised = false;

        service!.topHashtags = new List<KeyValuePair<string, int>> { new KeyValuePair<string, int>("#test", 1) };
        service!.hashtags.AddOrUpdate("#test", 1, (key, count) => ++count);
        service!.OnTopHashtagsChanged += (s, a) => eventRaised = true;
        service!.ProcessTopHashtags();

        Assert.IsFalse(eventRaised);
    }

    [TestMethod]
    public void ProcessTopHashtags_ShouldRaiseOnTopHashtagsChangedIfThereAreNewHashtags()
    {
        bool eventRaised = false;

        service!.hashtags.AddOrUpdate("#test", 1, (key, count) => ++count);
        service!.OnTopHashtagsChanged += (s, a) => eventRaised = true;
        service!.ProcessTopHashtags();

        Assert.IsTrue(eventRaised);
    }

    [TestMethod]
    public void ProcessTopHashtags_ShouldRaiseOnTopHashtagsChangedIfThereAreNewHashtagsAndExisting()
    {
        bool eventRaised = false;

        service!.topHashtags.AddHashtag("#test1", 1);
        service!.hashtags.AddHashtag("#test1", 1);
        service!.hashtags.AddHashtag("#test1", 2);
        service!.OnTopHashtagsChanged += (s, a) => eventRaised = true;
        service!.ProcessTopHashtags();

        Assert.IsTrue(eventRaised);
    }
}

public static class DictionaryExtensions
{
    public static void AddHashtag(this List<KeyValuePair<string, int>> list, string key, int count) =>
        list.Add(new KeyValuePair<string, int>(key, count));

    public static void AddHashtag(this ConcurrentDictionary<string, int> dictionary, string key, int count) =>
        dictionary.AddOrUpdate(key, count, (key, count) => ++count);
}


