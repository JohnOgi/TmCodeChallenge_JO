namespace TmCodeChallenge.Model.Services.VolumeStreams;

public interface ILongRequester
{
    void Cancel();
    Task GetData(Action<string> action);
}