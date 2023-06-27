using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task radixSortLSDBase2(CancellationToken token)
        {
            await radixSortLSD(2, token);
        }

        private async Task radixSortLSDBase3(CancellationToken token)
        {
            await radixSortLSD(3, token);
        }

        private async Task radixSortLSDBase8(CancellationToken token)
        {
            await radixSortLSD(8, token);
        }

        private async Task radixSortLSDBase10(CancellationToken token)
        {
            await radixSortLSD(10, token);
        }

        private async Task radixSortLSDBase16(CancellationToken token)
        {
            await radixSortLSD(16, token);
        }

        private async Task radixSortMSDBase2(CancellationToken token)
        {
            await radixSortMSD(2, token);
        }

        private async Task radixSortMSDBase3(CancellationToken token)
        {
            await radixSortMSD(3, token);
        }

        private async Task radixSortMSDBase8(CancellationToken token)
        {
            await radixSortMSD(8, token);
        }

        private async Task radixSortMSDBase10(CancellationToken token)
        {
            await radixSortMSD(10, token);
        }

        private async Task radixSortMSDBase16(CancellationToken token)
        {
            await radixSortMSD(16, token);
        }

        private int Pow(int n, int k, CancellationToken token)
        {
            int s = 1;

            while (k > 0)
            {
                if (k % 2 == 0)
                {
                    n *= n;
                    k /= 2;
                }
                else
                {
                    s *= n;
                    k--;
                }

                if (token.IsCancellationRequested) return 0;
            }

            return s;
        }

        private int getLastDigitIndex(int number, int nbase, CancellationToken token)
        {
            int k = 0;

            while (Pow(nbase, k + 1, token) <= number) k++;

            return k;

        }

        private int getMaxDigitIndex(int nbase, CancellationToken token)
        {
            int max = 0;

            for (int i = 0; i < _length; i++)
            {
                int di = getLastDigitIndex(_items[i].Value, nbase, token);
                if (di > max) max = di;
            }

            return max;
        }

        private int getNthDigit(int number, int nbase, int n, CancellationToken token)
        {
            return (int)(Math.Floor((decimal)number / (decimal)Pow(nbase, n, token))) % nbase;
        }

        private async Task radixSortLSD(int nbase, CancellationToken token)
        {
            int maxDI = getMaxDigitIndex(nbase, token);

            List<int>[] buckets = new List<int>[nbase];

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new List<int>();

                if (token.IsCancellationRequested) return;
            }

            for(int digit = 0; digit <= maxDI; digit++)
            {
                for (int i = 0; i < buckets.Length; i++)
                {
                    buckets[i].Clear();

                    if (token.IsCancellationRequested) return;
                }

                for (int i = 0; i < _length; i++)
                {
                    int bid = getNthDigit(_items[i].Value, nbase, digit, token);

                    buckets[bid].Add(_items[i].Value);
                    await show(i, i);

                    if (token.IsCancellationRequested) return;
                }

                List<int> values = buckets.SelectMany(x => x).ToList();

                for (int i = 0; i < _length; i++)
                {
                    _items[i].Value = values[i];
                    await show(i, i);

                    if (token.IsCancellationRequested) return;
                }              
            }
        }

        private async Task radixSortMSD(int nbase, CancellationToken token)
        {
            int maxDI = getMaxDigitIndex(nbase, token);

            await doRadixMSD(0, _length - 1, nbase, maxDI, token);
        }

        private async Task doRadixMSD(int l, int r, int nbase, int digit, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (digit >= 0)
            {
                List<int>[] buckets = new List<int>[nbase];

                for (int i = 0; i < buckets.Length; i++)
                {
                    buckets[i] = new List<int>();

                    if (token.IsCancellationRequested) return;
                }

                for (int i = l; i <= r; i++)
                {
                    int bid = getNthDigit(_items[i].Value, nbase, digit, token);

                    buckets[bid].Add(_items[i].Value);
                    await show(i, i);

                    if (token.IsCancellationRequested) return;
                }

                List<int> values = buckets.SelectMany(x => x).ToList();

                for (int i = l; i <= r; i++)
                {
                    _items[i].Value = values[i - l];
                    await show(i, i);

                    if (token.IsCancellationRequested) return;
                }

                int start = l; int end = l + buckets[0].Count - 1;

                for (int i = 0; i < buckets.Length; i++)
                {
                    if (buckets[i].Count > 0)
                    {
                        await doRadixMSD(start, end, nbase, digit - 1, token);
                    }

                    if (i != buckets.Length - 1)
                    {
                        start = end + 1;
                        end += buckets[i + 1].Count;
                    }

                    if (token.IsCancellationRequested) return;
                }

                await doRadixMSD(end + 1, r, nbase, digit - 1, token);
            }
        }
    }
}
