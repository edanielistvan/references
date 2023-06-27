using System;
using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task timSort(CancellationToken token)
        {
            int RUN = 15;

            for (int i = 0; i < _length; i+=RUN)
            {
                await unInsertionSort(i, Math.Min(i + RUN, _length), token);

                if (token.IsCancellationRequested) return;
            }

            for (int size = RUN; size < _length; size = 2*size)
            {
                for(int left = 0; left < _length; left += 2*size)
                {
                    int mid = left + size - 1;
                    int right = Math.Min(left + 2 * size - 1, _length - 1);

                    if (mid < right)
                    {
                        await merge(left, mid, mid + 1, right, token);
                    }

                    if (token.IsCancellationRequested) return;
                }
            }
        }
    }
}
