using System.Windows;
using TmCodeChallenge.Model.Services.VolumeStreams;
using TmCodeChallenge.ViewModels;

namespace TmCodeChallenge;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public TestViewModel TestViewModel { get; private set; }
    public ITwitterVolumeService TwitterService { get; set; }

    public MainWindow(ITwitterVolumeService twitterVolumeService)
    {
        InitializeComponent();

        TestViewModel = (TestViewModel)DataContext;
        TwitterService = twitterVolumeService;

        TwitterService.OnTweetStreamed += TwitterService_OnTweetStreamed;
        TwitterService.OnTopHashtagsChanged += TwitterService_OnTopHashtagsChanged;
        TwitterService.OnError += TwitterService_OnError;
    }

    private void TwitterService_OnError(object sender, System.IO.ErrorEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            MessageBox.Show(e.GetException().Message, "There has been an error.");
        });
    }

    private void TwitterService_OnTopHashtagsChanged(object sender, TopHashtagsChangedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            TestViewModel.TopHashtags = e.TopHashtags;
        });
    }

    private void TwitterService_OnTweetStreamed(object sender, TweetStreemedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            TestViewModel.Tweets.Add(e.Tweet);
            TestViewModel.TweetCount++;
        });
    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        TestViewModel.IsWorking = true;
        await TwitterService.StartProcessing();
        TestViewModel.IsWorking = false;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        TwitterService.StopProcessing();
        TestViewModel.IsWorking = false;
    }
}
