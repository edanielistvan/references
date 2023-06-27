using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task OddEvenMergeSortInit(CancellationToken token)
        {
            await OddEvenMergeSort(0, _length - 1, token);
        }

        private async Task OddEvenMergeSort(int l, int r, CancellationToken token)
        {
            if (!token.IsCancellationRequested && l < r)
            {
                int m = (l + r) / 2;

                if (token.IsCancellationRequested) return;
                await OddEvenMergeSort(l, m, token);
                if (token.IsCancellationRequested) return;
                await OddEvenMergeSort(m + 1, r, token);

                if (token.IsCancellationRequested) return;
                await OddEvenMerge(l, m, m + 1, r, token);
            }
        }

        private async Task OddEvenMerge(int a1, int a2, int b1, int b2, CancellationToken token)
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
                _items[both].Value = temp[first];
                await show(both, a1 + first);
                first++; both++;

                if (token.IsCancellationRequested) return;

                _items[both].Value = _items[second].Value;
                await show(both, second);
                second++; both++;

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

            await unInsertionSort(a1, Math.Min(b2 + 1, _length), token);
        }
    }
}
