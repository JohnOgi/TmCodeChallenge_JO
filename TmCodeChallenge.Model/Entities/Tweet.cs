using System.Text.Json;
using System.Text.RegularExpressions;

namespace TmCodeChallenge.Model.Entities
{
    public class Tweet
    {
        public string? text { get; set; }
        public List<string>? Hashtags { get; set; }

        public static Tweet? Parse(string source)
        {
            var options = new JsonSerializerOptions();
            TweetWrapper? tweetWrapper = JsonSerializer.Deserialize<TweetWrapper>(source);

            if (tweetWrapper?.title == "ConnectionException")
            {
                throw new Exception("Rate limit hit.");
            }
            else if(tweetWrapper?.data?.text == null)
            {
                throw new Exception("Invalid tweet.");
            }

            tweetWrapper.data.Hashtags = new Regex("(^|\\s)#(\\w+)", RegexOptions.IgnoreCase).Matches(tweetWrapper.data.text)
                .Select(x => x.Value.Trim())
                .ToList();

            return tweetWrapper.data;
        }
    }
}
