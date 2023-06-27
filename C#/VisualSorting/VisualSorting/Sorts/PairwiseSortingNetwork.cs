using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task pairwiseSortingNetworkInit(CancellationToken token)
        {
            await pairwiseSortingNetwork(0, _length, 1, token);
        }


        private async Task pairwiseSortingNetwork(int l, int r, int gap, CancellationToken token)
        {
            if (l == r - gap) return;

            int b = l + gap;
            while (b < r)
            {
                if (_items[b - gap].Value > _items[b].Value)
                {
                    await swap(b, b - gap);
                }    
                else
                {
                    await show(b, b - gap);
                }

                if (token.IsCancellationRequested) return;

                b += (2 * gap);
            }

            if (token.IsCancellationRequested) return;

            if (((r - l) / gap) % 2 == 0)
            {
                await pairwiseSortingNetwork(l, r, gap * 2, token);
                await pairwiseSortingNetwork(l + gap, r + gap, gap * 2, token);
            }
            else
            {
                await pairwiseSortingNetwork(l, r + gap, gap * 2, token);
                await pairwiseSortingNetwork(l + gap, r, gap * 2, token);
            }

            int a = 1;

            while (a < ((r - l) / gap))
            {
                a = (a * 2) + 1;

                if (token.IsCancellationRequested) return;
            }

            b = l + gap;
            while (b + gap < r)
            {
                int c = a;
                while (c > 1)
                {
                    c /= 2;
                    if (b + (c * gap) < r)
                    {
                        if (_items[b].Value > _items[b + (c * gap)].Value)
                        {
                            await swap(b, b + (c * gap));
                        }
                        else
                        {
                            await show(b, b + (c * gap));
                        }
                    }

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;

                b += (2 * gap);
            }
        }
    }
}