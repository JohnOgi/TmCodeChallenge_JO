using TmCodeChallenge.Model.Entities;

namespace TmCodeChallenge.Model.Tests.Entities
{
    [TestClass]
    public class TweetTests
    {
        [TestMethod]
        public void Parse_ShouldReturnATweet()
        {
            var tweetString = $@"{{""data"":{{""text"":""test""}}}}";

            var tweet = Tweet.Parse(tweetString);

            Assert.IsNotNull(tweet);
        }

        [TestMethod]
        public void Parse_ShouldSetHashtagsWhenTheyExist()
        {
            var tweetString = $@"{{""data"":{{""text"":""test #hash1 #hash2 ""}}}}";

            var tweet = Tweet.Parse(tweetString);

            Assert.AreEqual(tweet?.Hashtags?.Count, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Rate limit hit.")]
        public void Parse_ShouldThrowWhenRateLimitHit()
        {
            var tweetString = $@"{{""title"":""ConnectionException""}}";

            var tweet = Tweet.Parse(tweetString);

            Assert.AreEqual(tweet?.Hashtags?.Count, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Invalid tweet.")]
        public void Parse_ShouldThrowWhenInvalidTweet()
        {
            var tweetString = $@"{{}}";

            var tweet = Tweet.Parse(tweetString);

            Assert.AreEqual(tweet?.Hashtags?.Count, 2);
        }
    }
}
