using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task selectionSortMin(CancellationToken token)
        {
            for (int i = 0; i < _length - 1; i++)
            {
                int min = i;

                for (int j = i + 1; j < _length; j++)
                {
                    undo();
                    if (_items[min].Value > _items[j].Value) min = j;
                    await show(min, j);

                    if (token.IsCancellationRequested) return;
                }

                if (min != i)
                {
                    await swap(i, min);
                }
            }
        }

        private async Task selectionSortMax(CancellationToken token)
        {
            for (int i = 0; i < _length - 1; i++)
            {
                int max = i;

                for (int j = i + 1; j < _length; j++)
                {
                    undo();
                    if (_items[max].Value < _items[j].Value) max = j;
                    await show(max, j);

                    if (token.IsCancellationRequested) return;
                }

                if (max != i)
                {
                    await swap(i, max);
                }
            }

            await flip(0, _length - 1);
        }

        private async Task selectionSortMixed(CancellationToken token)
        {
            int l = 0;
            int r = _length - 1;

            while (l < r)
            {
                int max = r;
                int min = l;

                for (int i = l; i <= r; i++)
                {
                    undo();
                    if (_items[max].Value < _items[i].Value) max = i;
                    await show(max, i);
                    if (_items[min].Value > _items[i].Value) min = i;
                    await show(min, i);

                    if (token.IsCancellationRequested) return;
                }

                if (max != l)
                {
                    await swap(min, l);
                    await swap(max, r);
                }
                else if (min != r)
                {
                    await swap(max, r);
                    await swap(min, l);
                }
                else
                {
                    await swap(min, max);
                }

                l++;
                r--;
            }
        }
    }
}
