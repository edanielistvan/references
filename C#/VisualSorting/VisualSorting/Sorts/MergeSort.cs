using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task mergeInitSort(CancellationToken token)
        {
            await mergeSort(0, _length - 1, token);
        }

        private async Task merge(int a1, int a2, int b1, int b2, CancellationToken token)
        {
            int[] temp = new int[a2 - a1 + 1];

            for (int i = a1; i <= a2; i++)
            {
                temp[i - a1] = _items[i].Value;
                await show(a1, i);

                if (token.IsCancellationRequested) return;
            }

            int first = 0; int second = b1; int both = a1;

            while (first < temp.Length && second <= b2)
            {
                if (temp[first] > _items[second].Value)
                {
                    _items[both].Value = _items[second].Value;
                    await show(both, second);
                    second++; both++;
                }
                else if (temp[first] < _items[second].Value)
                {
                    _items[both].Value = temp[first];
                    await show(both, a1 + first);
                    first++; both++;
                }
                else
                {
                    _items[both].Value = _items[second].Value;
                    await show(both, second);
                    second++; both++;
                    _items[both].Value = temp[first];
                    await show(both, a1 + first);
                    first++; both++;
                }

                if (token.IsCancellationRequested) return;
            }

            while (first < temp.Length)
            {
                _items[both].Value = temp[first];
                await show(both, a1 + first);
                first++; both++;

                if (token.IsCancellationRequested) return;
            }

            while (second <= b2)
            {
                _items[both].Value = _items[second].Value;
                await show(both, second);
                second++; both++;

                if (token.IsCancellationRequested) return;
            }
        }

        private async Task mergeSort(int l, int r, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (r > l)
            {
                int mid = (r + l) / 2;

                await show(l, mid);
                await mergeSort(l, mid, token);
                await show(mid + 1, r);
                await mergeSort(mid + 1, r, token);

                await merge(l, mid, mid + 1, r, token);
            }
        }
    }
}
