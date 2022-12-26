using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TmCodeChallenge.Model.Entities;

namespace TmCodeChallenge.ViewModels
{
    public class TestViewModel : INotifyPropertyChanged
    {
        public TestViewModel()
        {
            Tweets = new CyclingObsesrvableCollection<Tweet>(50);
        }

        private bool isWorking;
        private int tweetCount;

        public bool IsWorking
        {
            get { return isWorking; }
            set
            {
                if (isWorking == value) return;
                isWorking = value;
                OnPropertyChanged(nameof(IsWorking));
            }
        }

        public int TweetCount
        {
            get { return tweetCount; }
            set
            {
                if (tweetCount == value) return;
                tweetCount = value;
                OnPropertyChanged(nameof(TweetCount));
            }
        }


        public CyclingObsesrvableCollection<Tweet> Tweets { get; set; }

        private List<KeyValuePair<string, int>> topHashtags = new List<KeyValuePair<string, int>>();
        public List<KeyValuePair<string, int>> TopHashtags
        {
            get { return topHashtags; }
            set
            {
                topHashtags = value;
                OnPropertyChanged(nameof(TopHashtags));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
