using System;
using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task combSort(CancellationToken token)
        {
            double shrink = 1.3;
            int gap = _length;

            bool isSorted = false;

            while (!isSorted)
            {
                gap = Convert.ToInt32(Math.Floor(Convert.ToDouble(gap) / shrink));

                if (gap <= 1)
                {
                    isSorted = true;
                    gap = 1;
                }

                for (int i = 0; i < _length - gap; i++)
                {
                    int sm = gap + i;

                    if (_items[i].Value > _items[sm].Value)
                    {
                        await swap(i, sm);
                        isSorted = false;
                    }
                    else
                    {
                        await show(i, sm);
                    }

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;
            }
        }
    }
}
