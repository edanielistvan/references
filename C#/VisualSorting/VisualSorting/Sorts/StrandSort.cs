using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task strandMerge(int a1, int b1, int a2, int b2, CancellationToken token)
        {
            int n1 = b1 - a1 + 1;

            int[] l = new int[n1];

            for (int i = 0; i < n1; i++)
            {
                l[i] = _items[a1 + i].Value;
            }

            int a = 0, b = a2;
            int s = a1;

            while (a < n1 && b < b2)
            {
                if (l[a] <= _items[b].Value)
                {
                    _items[s].Value = l[a];
                    await show(s, a1 + a);
                    a++;
                }
                else
                {
                    _items[s].Value = _items[b].Value;
                    await show(s, b);
                    b++;
                }

                if (token.IsCancellationRequested) return;

                s++;
            }

            while (a < n1)
            {
                _items[s].Value = l[a];
                await show(s, a1 + a);
                a++;
                s++;

                if (token.IsCancellationRequested) return;
            }

            while (b < b2)
            {
                _items[s].Value = _items[b].Value;
                await show(s, b);
                b++;
                s++;

                if (token.IsCancellationRequested) return;
            }
        }

        private async Task strandSort(CancellationToken token)
        {
            int r = 0; int n = 1;

            while (n < _length)
            {
                int val = _items[r].Value;
                await show(r, n);

                for (int i = n; i < _length; i++)
                {
                    if (_items[i].Value >= val)
                    {
                        await swap(i, n);
                        val = _items[n].Value;
                        n++;
                    }
                    else
                    {
                        await show(i, r);
                    }

                    if (token.IsCancellationRequested) return;
                }

                if (r > 0) await strandMerge(0, r - 1, r, n, token);

                if (token.IsCancellationRequested) return;

                r = n;
                n++;
            }

            if (r == _length - 1) await strandMerge(0, r - 1, r, n, token);
        }
    }
}
