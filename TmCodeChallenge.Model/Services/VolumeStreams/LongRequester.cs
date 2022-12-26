using System.Configuration;
using System.Diagnostics;

namespace TmCodeChallenge.Model.Services.VolumeStreams;
internal class HttpClientWrapper : HttpClient { }
public class LongRequester : ILongRequester
{
    const string streamUrl = "https://api.twitter.com/2/tweets/sample/stream";
    internal HttpClientWrapper client = new HttpClientWrapper();
    internal CancellationTokenSource cancelSource = new CancellationTokenSource();
    private bool isCanceling = false;

    public LongRequester()
    {
        var token = ConfigurationManager.AppSettings.Get("AppAccessToken");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
    }

    public void Cancel()
    {
        isCanceling = true;
    }

    public async Task GetData(Action<string> action)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, streamUrl);
            using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancelSource.Token))
            using (var body = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(body))
            {
                while (!reader.EndOfStream)
                {
                    if (isCanceling == true)
                    {
                        return;
                    }
                    var text = reader.ReadLine();

                    if (!string.IsNullOrEmpty(text))
                    {
                        action(text);
                    }
                }
            }
        }
        finally
        {
            isCanceling = false;
        }
    }
}
