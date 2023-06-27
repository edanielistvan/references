using System.Threading;
using System.Threading.Tasks;


namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task compareAndSwap(int i, int j, bool dir)
        {
            bool k = _items[i].Value > _items[j].Value ? true : false;

            if (dir == k)
            {
                await swap(i, j);
            }
            else
            {
                await show(i, j);
            }
        }

        private int greatestPowerOfTwoLessThan(int n)
        {
            int k = 1;

            while (k < n)
            {
                k = k << 1;
            }

            return k >> 1;
        }

        private async Task bitonicMerge(int low, int count, bool dir, CancellationToken token)
        {
            if (count > 1)
            {
                int k = greatestPowerOfTwoLessThan(count);

                for (int i = low; i < low + count - k; i++)
                {
                    await compareAndSwap(i, i + k, dir);

                    if (token.IsCancellationRequested) return;
                }

                await bitonicMerge(low, k, dir, token);
                await bitonicMerge(low + k, count - k, dir, token);
            }
        }

        private async Task bitonicSort(int low, int count, bool dir, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (count > 1)
            {
                int k = count / 2;

                await bitonicSort(low, k, !dir, token);
                await bitonicSort(low + k, count - k, dir, token);

                await bitonicMerge(low, count, dir, token);
            }
        }

        private async Task bitonicInitSort(CancellationToken token)
        {
            await bitonicSort(0, _length, true, token);
        }
    }
}

