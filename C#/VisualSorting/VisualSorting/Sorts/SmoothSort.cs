using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private int[] LP = { 1, 1, 3, 5, 9, 15, 25, 41, 67, 109,
            177, 287, 465, 753, 1219, 1973, 3193, 5167, 8361, 13529, 21891,
            35421, 57313, 92735, 150049, 242785, 392835, 635621, 1028457,
            1664079, 2692537, 4356617, 7049155, 11405773, 18454929, 29860703,
            48315633, 78176337, 126491971, 204668309, 331160281, 535828591,
            866988873 };

        private async Task smoothSift(int pshift, int head, CancellationToken token)
        {
            int val = _items[head].Value;

            await show(head, head);

            if (token.IsCancellationRequested) return;

            while (pshift > 1)
            {
                int rt = head - 1;
                int lf = head - 1 - LP[pshift - 2];

                await show(lf, rt);
                if (val >= _items[lf].Value && val >= _items[rt].Value)
                {
                    break;
                }             

                if (_items[lf].Value >= _items[rt].Value)
                {
                    _items[head].Value = _items[lf].Value;
                    await show(lf, head);

                    head = lf;
                    pshift -= 1;
                }
                else
                {
                    _items[head].Value = _items[rt].Value;
                    await show(lf, head);
                    
                    head = rt;
                    pshift -= 2;
                }

                if (token.IsCancellationRequested) return;
            }

            _items[head].Value = val;

            await show(head, head);
        }

        private async Task trinkle(int p, int pshift, int head, bool isTrusty, CancellationToken token)
        {
            int val = _items[head].Value;

            await show(head, head);

            if (token.IsCancellationRequested) return;

            while (p != 1)
            {
                int stepson = head - LP[pshift];

                if (_items[stepson].Value <= val)
                    break; 

                if (!isTrusty && pshift > 1)
                {
                    int rt = head - 1;
                    int lf = head - 1 - LP[pshift - 2];

                    await show(rt, lf);

                    if (_items[rt].Value >= _items[stepson].Value || _items[lf].Value >= _items[stepson].Value)
                        break;
                }

                if (token.IsCancellationRequested) return;

                await show(head, stepson);
                _items[head].Value = _items[stepson].Value;

                head = stepson;
                int trail = BitOperations.TrailingZeroCount(p & ~1);
                p >>= trail;
                pshift += trail;
                isTrusty = false;
            }

            if (token.IsCancellationRequested) return;

            if (!isTrusty)
            {
                await show(head, head);
                _items[head].Value = val;

                await smoothSift(pshift, head, token);
            }
        }

        private async Task doSort(int lo, int hi, bool fullSort, CancellationToken token)
        {
            int[] LP = {1, 1, 3, 5, 9, 15, 25, 41, 67, 109,
            177, 287, 465, 753, 1219, 1973, 3193, 5167, 8361, 13529, 21891,
            35421, 57313, 92735, 150049, 242785, 392835, 635621, 1028457,
            1664079, 2692537, 4356617, 7049155, 11405773, 18454929, 29860703,
            48315633, 78176337, 126491971, 204668309, 331160281, 535828591,
            866988873 };

            int head = lo; 

            int p = 1;
            int pshift = 1;

            while (head < hi)
            {
                if ((p & 3) == 3)
                {
                    await smoothSift(pshift, head, token);
                    p >>= 2;
                    pshift += 2;
                }
                else
                {
                    if (LP[pshift - 1] >= hi - head)
                    {
                        await trinkle(p, pshift, head, false, token);
                    }
                    else
                    {
                        await smoothSift(pshift, head, token);
                    }

                    if (pshift == 1)
                    {
                        p <<= 1;
                        pshift--;
                    }
                    else
                    {
                        p <<= (pshift - 1);
                        pshift = 1;
                    }
                }

                if (token.IsCancellationRequested) return;

                p |= 1;
                head++;
            }

            if (token.IsCancellationRequested) return;

            if (fullSort)
            {
                await trinkle(p, pshift, head, false, token);

                while (pshift != 1 || p != 1)
                {
                    if (pshift <= 1)
                    {
                        int trail = BitOperations.TrailingZeroCount(p & ~1);
                        p >>= trail;
                        pshift += trail;
                    }
                    else
                    {
                        p <<= 2;
                        p ^= 7;
                        pshift -= 2;

                        await trinkle(p >> 1, pshift + 1, head - LP[pshift] - 1, true, token);
                        await trinkle(p, pshift, head - 1, true, token);
                    }

                    if (token.IsCancellationRequested) return;

                    head--;
                }
            }
        }

        public async Task smoothSort(CancellationToken token)
        {
            await doSort(0, _length - 1, true, token);
        }
    }
}
