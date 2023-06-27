using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        public async Task quickSortInitDualPivot(CancellationToken token)
        {
            await quickSortDualPivot(0, _length - 1, token);
        }

        public async Task quickSortInitMultiPivot(CancellationToken token)
        {
            await quickSortMultiPivot(0, _length - 1, token);
        }

        public async Task quickSortDualPivot(int l, int r, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (l < r)
            {
                int[] pivs = await partitionDualPivot(l, r, token);
                await quickSortDualPivot(l, pivs[0] - 1, token);
                await quickSortDualPivot(pivs[0] + 1, pivs[1] - 1, token);
                await quickSortDualPivot(pivs[1] + 1, r, token);
            }
        }

        public int detPivAmount(int len)
        {
            int a = 1; int k = 2;

            while (2 * k < len)
            {
                a++;
                k *= 2;
            }

            return a;
        }

        public async Task quickSortMultiPivot(int l, int r, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (l < r)
            {
                int numPivs = detPivAmount(r - l);
                int[] pivs = await partitionMultiPivot(l, r, numPivs, token);

                await quickSortMultiPivot(l, pivs[0] - 1, token);
                for (int i = 1; i < numPivs; i++)
                {
                    await quickSortMultiPivot(pivs[i - 1] + 1, pivs[i] - 1, token);
                }
                await quickSortMultiPivot(pivs[numPivs - 1] + 1, r, token);
            }
        }

        public async Task<int[]> partitionDualPivot(int l, int r, CancellationToken token)
        {
            int lpiv = _rnd.Next(l, r + 1);
            await swap(lpiv, l);

            int rpiv = _rnd.Next(l + 1, r + 1);
            await swap(rpiv, r);

            if (_items[l].Value > _items[r].Value) await swap(l, r);

            int j = l + 1; int g = r - 1; int k = l + 1;

            while (k <= g)
            {
                if (_items[k].Value < _items[l].Value)
                {
                    await swap(k, j);
                    j++;
                }
                else if (_items[k].Value >= _items[r].Value)
                {
                    while (_items[g].Value > _items[r].Value && k < g) 
                    {
                        g--; 
                        await show(k, g);

                        if (token.IsCancellationRequested) return new int[] { -1 };
                    }

                    await swap(k, g);
                    g--;

                    if (_items[k].Value < _items[l].Value)
                    {
                        await swap(k, j);
                        j++;
                    }
                }

                if (token.IsCancellationRequested) return new int[] { -1 };

                await show(k, g);
                k++;
            }
            j--;
            g++;

            await swap(l, j);
            await swap(r, g);

            return new int[2] { j, g };
        }

        public async Task<int[]> partitionMultiPivot(int l, int r, int numPivs, CancellationToken token)
        {
            int[] pivs = new int[numPivs];

            int i = l;

            for (int j = 0; j < numPivs; j++)
            {
                int pv = _rnd.Next(i, r + 1);
                await swap(pv, i);
                pivs[j] = i;
                i++;

                if (token.IsCancellationRequested) return new int[] { -1 };
            }

            i--;

            if (numPivs > 1)
            {
                await quickSortMultiPivot(l, i, token);
            }
            

            int last = r;
            while (i >= l)
            {
                int j = i + 1;
                while (j < last)
                {
                    while (j < last && _items[last].Value >= _items[i].Value)
                    {
                        await show(last, i);
                        last--;

                        if (token.IsCancellationRequested) return new int[] { -1 };
                    }

                    while (j < last && _items[j].Value < _items[i].Value)
                    {
                        await show(i, j);
                        j++;

                        if (token.IsCancellationRequested) return new int[] { -1 };
                    }

                    if (j < last) await swap(j, last);

                    if (token.IsCancellationRequested) return new int[] { -1 };
                }

                while (last > i && _items[last].Value >= _items[i].Value)
                { 
                    last--;

                    if (token.IsCancellationRequested) return new int[] { -1 };
                }

                if (token.IsCancellationRequested) return new int[] { -1 };

                await swap(i, last);
                pivs[i - l] = last; i--;
            }

            return pivs;
        }
    }
}
