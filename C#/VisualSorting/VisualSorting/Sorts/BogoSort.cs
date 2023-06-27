using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private bool sorted()
        {
            int i = 1;
            while (i < _length && _items[i].Value >= _items[i - 1].Value) i++;
            return i >= _length;
        }

        private async Task bogoSort(CancellationToken token)
        {
            while (!sorted())
            {
                for (int i = 0; i < _length; i++)
                {
                    await swap(i, _rnd.Next(0, _length));

                    if (token.IsCancellationRequested) return;
                }
            }
        }

        private async Task bozoSort(CancellationToken token)
        {
            while (!sorted())
            {
                await swap(_rnd.Next(0, _length), _rnd.Next(0, _length));

                if (token.IsCancellationRequested) return;
            }
        }

        private async Task swapBogoSort(CancellationToken token)
        {
            while (!sorted())
            {
                int a = _rnd.Next(0, _length);
                int b = _rnd.Next(0, _length);

                int ab = (a < b) ? 1 : -1;
                int values = (_items[a].Value < _items[b].Value) ? 1 : -1;

                if (ab + values == 0)
                {
                    await swap(a, b);
                }
                else
                {
                    await show(a, b);
                }

                if (token.IsCancellationRequested) return;
            }
        }

        private async Task bogoBubbleSort(CancellationToken token)
        {
            while (!sorted())
            {
                int a = _rnd.Next(0, _length);

                if (a < _length - 1)
                {
                    if (_items[a].Value > _items[a + 1].Value)
                    {
                        await swap(a, a + 1);
                    }
                    else
                    {
                        await show(a, a + 1);
                    }
                }
                
                if (token.IsCancellationRequested) return;
            }
        }
    }
}
