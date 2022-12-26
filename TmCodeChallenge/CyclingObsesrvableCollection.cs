using System.Collections.ObjectModel;

namespace TmCodeChallenge;

public class CyclingObsesrvableCollection<T> : ObservableCollection<T>
{
    private int maxCount;
    private int currentIndex;

    public CyclingObsesrvableCollection(int maxCount)
    {
        this.maxCount = maxCount;
    }

    public new void Add(T item)
    {
        if (Count == maxCount)
        {
            RemoveAt(currentIndex);
        }
        Insert(currentIndex, item);

        currentIndex = currentIndex == maxCount - 1 ? 0 : ++currentIndex;
    }
}
