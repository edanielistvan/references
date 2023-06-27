using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task quickSortInitWindow(CancellationToken token)
        {
            await quickSortWindow(0, _length - 1, token);
        }

        private async Task quickSortInitBetterWindow(CancellationToken token)
        {
            await quickSortBetterWindow(0, _length - 1, token);
        }

        private async Task quickSortInitPushing(CancellationToken token)
        {
            await quickSortPushing(0, _length - 1, token);
        }

        private async Task quickSortInitLomuto(CancellationToken token)
        {
            await quickSortLomuto(0, _length - 1, token);
        }

        private async Task quickSortInitFind(CancellationToken token)
        {
            await quickSortFind(0, _length - 1, token);
        }

        private async Task quickSortInitHoare(CancellationToken token)
        {
            await quickSortHoare(0, _length - 1, token);
        }

        private async Task quickSortInitPerfectPivot(CancellationToken token)
        {
            await quickSortPerfectPivot(0, _length - 1, token);
        }

        private async Task quickSortWindow(int l, int r, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (l < r)
            {
                int p = await partitionWindow(l, r, token);
                await quickSortWindow(l, p - 1, token);
                await quickSortWindow(p + 1, r, token);
            }
        }

        private async Task quickSortBetterWindow(int l, int r, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (l < r)
            {
                int p = await partitionBetterWindow(l, r, token);
                await quickSortBetterWindow(l, p - 1, token);
                await quickSortBetterWindow(p + 1, r, token);
            }
        }

        private async Task quickSortPushing(int l, int r, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (l < r)
            {
                int p = await partitionPushing(l, r, token);
                await quickSortPushing(l, p - 1, token);
                await quickSortPushing(p + 1, r, token);
            }
        }

        private async Task quickSortLomuto(int l, int r, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (l < r)
            {
                int p = await partitionLomuto(l, r, token);
                await quickSortLomuto(l, p - 1, token);
                await quickSortLomuto(p + 1, r, token);
            }
        }

        private async Task quickSortFind(int l, int r, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (l < r)
            {
                int p = await partitionFind(l, r, token);
                await quickSortFind(l, p - 1, token);
                await quickSortFind(p + 1, r, token);
            }
        }

        private async Task quickSortHoare(int l, int r, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (l < r)
            {
                int p = await partitionHoare(l, r, token);
                await quickSortHoare(l, p - 1, token);
                await quickSortHoare(p + 1, r, token);
            }
        }

        private async Task quickSortPerfectPivot(int l, int r, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (l < r)
            {
                int p = await partitionPerfectPivot(l, r, token);
                await quickSortPerfectPivot(l, p - 1, token);
                await quickSortPerfectPivot(p + 1, r, token);
            }
        }

        private async Task<int> partitionWindow(int l, int r, CancellationToken token)
        {
            int piv = _rnd.Next(l, r + 1);
            await swap(l, piv);

            int lgt = 0;
            piv = l;

            while (piv + lgt < r)
            {
                int nxt = piv + lgt + 1;

                if (token.IsCancellationRequested) return -1;

                if (_items[piv].Value <= _items[nxt].Value)
                {
                    lgt++;
                }
                else
                {
                    while (nxt > piv)
                    {
                        await swap(nxt, nxt - 1);
                        nxt--;

                        if (token.IsCancellationRequested) return -1;
                    }
                    piv++;
                }

                await show(piv, nxt);
            }

            return piv;
        }

        private async Task<int> partitionBetterWindow(int l, int r, CancellationToken token)
        {
            int piv = _rnd.Next(l, r + 1);
            await swap(l, piv);

            int lgt = 0;
            piv = l;

            while (piv + lgt < r)
            {
                int nxt = piv + lgt + 1;

                if (token.IsCancellationRequested) return -1;

                if (_items[piv].Value <= _items[nxt].Value)
                {
                    lgt++;
                }
                else
                {
                    await swap(nxt, piv + 1);
                    await swap(piv, piv + 1);

                    piv++;
                }

                await show(piv, nxt);
            }

            return piv;
        }

        private int sgn(int num)
        {
            return num < 0 ? -1 : num > 0 ? 1 : 0;
        }

        private async Task<int> partitionPushing(int l, int r, CancellationToken token)
        {
            int piv = _rnd.Next(l, r + 1);
            int i = l;

            while (i <= r)
            {
                if (i != piv)
                {
                    int s = sgn(i - piv);

                    if (s * _items[i].Value < s * _items[piv].Value)
                    {
                        await swap(i, piv + s);
                        await swap(piv, piv + s);
                        i--;
                        piv += s;
                    }
                }

                if (token.IsCancellationRequested) return -1;

                i++;
            }

            return piv;
        }

        private async Task<int> partitionLomuto(int l, int r, CancellationToken token)
        {
            int piv = _rnd.Next(l, r + 1);
            await swap(piv, r);

            int i = l;
            for(int j = l; j < r; j++)
            {
                if (_items[j].Value < _items[r].Value)
                {
                    await swap(i, j);
                    i++;
                }
                else
                {
                    await show(i, j);
                }

                if (token.IsCancellationRequested) return -1;
            }
            await swap(i, r);
            return i;
        }

        private async Task<int> partitionFind(int l, int r, CancellationToken token)
        {
            int piv = _rnd.Next(l, r + 1);
            int ind = l - 1;

            for (int i = l; i <= r; i++)
            {
                await show(i, piv);
                if (_items[i].Value <= _items[piv].Value) ind++;
            }

            await swap(piv, ind); piv = ind;

            int a = l; int b = piv + 1;
            while (a < piv && b <= r)
            {
                await show(a, b);

                while (a < piv && _items[a].Value <= _items[piv].Value)
                {
                    await show(a, b);
                    a++;

                    if (token.IsCancellationRequested) return -1;
                }

                while (b <= r && _items[b].Value > _items[piv].Value)
                {
                    await show(a, b);
                    b++;

                    if (token.IsCancellationRequested) return -1;
                }

                if (token.IsCancellationRequested) return -1;

                if (a < piv && b <= r)
                {
                    await swap(a, b);
                }
            }

            return piv;
        }

        private async Task<int> partitionHoare(int l, int r, CancellationToken token)
        {
            int m = _items[l + ((r - l) / 2)].Value;
            int i = l;
            int j = r;

            while (true)
            {
                while (_items[j].Value > m)
                {
                    await show(i, j);
                    j--;

                    if (token.IsCancellationRequested) return -1;
                }

                await show(i, j);

                while (_items[i].Value < m)
                {
                    await show(i, j);
                    i++;

                    if (token.IsCancellationRequested) return -1;
                }

                if (token.IsCancellationRequested) return -1;
                if (i >= j) return j;

                await swap(i, j);
            }
        }

        private async Task<int> partitionPerfectPivot(int l, int r, CancellationToken token)
        {
            int limit = r;

            while (limit > (l + r) / 2)
            {
                await swap(_rnd.Next(l, limit + 1), l);
                int place = limit;

                for (int i = place; i > l; i--)
                {
                    if (_items[i].Value > _items[l].Value)
                    {
                        await swap(i, place);
                        place--;
                    }
                    else
                    {
                        await show(i, place);
                    }
                }

                limit = place;
            }

            await swap(l, limit);

            return limit;
        }
    }
}
