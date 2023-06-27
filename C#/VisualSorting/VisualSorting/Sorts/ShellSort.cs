using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task shellSort(CancellationToken token)
        {
            int[] gaps = new int[] { 701, 301, 132, 57, 23, 10, 4, 1 };

            foreach(var gap in gaps)
            {
                for (int i = gap; i < _length; i++)
                {
                    int temp = _items[i].Value;
                    await show(i, i);

                    int j = i;
                    for (j = i; (j >= gap) && (_items[j - gap].Value > temp); j -= gap)
                    {
                        _items[j].Value = _items[j - gap].Value;
                        await show(j, j - gap);

                        if (token.IsCancellationRequested) return;
                    }

                    if (token.IsCancellationRequested) return;

                    _items[j].Value = temp;
                    await show(j, i);
                }
            }
        }
    }
}
