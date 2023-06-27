using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task insertionSort(CancellationToken token)
        {
            int i = 0;
            while (i < _length)
            {
                int j = i;
                while (j > 0 && _items[j-1].Value > _items[j].Value) 
                {
                    await swap(j - 1, j);
                    j--;

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;

                i++;
            }

        }

        private async Task insertionSortOptimized(CancellationToken token)
        {
            int i = 0;
            while (i < _length)
            {
                int x = _items[i].Value;
                int j = i - 1;

                while (j >= 0 && _items[j].Value > x)
                {
                    _items[j+1].Value = _items[j].Value;
                    await show(j, j + 1);
                    j--;

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;

                _items[j + 1].Value = x;
                await show(j + 1, i);
                i++;
            }
        }
    }
}
