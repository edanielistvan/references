using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task bubbleSort(CancellationToken token)
        {
            bool swapped = true;

            while (swapped)
            {
                swapped = false;
                for (int i = 1; i < _length; i++)
                {
                    if (_items[i-1].Value > _items[i].Value)
                    {
                        await swap(i - 1, i);
                        swapped = true;
                    }
                    else
                    {
                        await show(i - 1, i);
                    }

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;
            }
        }

        private async Task bubbleSortOptimized(CancellationToken token)
        {
            int n = _length;

            while (n >= 1)
            {
                int newn = 0;
                for (int i = 1; i < n; i++)
                {
                    if (_items[i - 1].Value > _items[i].Value)
                    {
                        await swap(i - 1, i);
                        newn = i;
                    }
                    else
                    {
                        await show(i - 1, i);
                    }

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;

                n = newn;
            }
        }

        private async Task cocktailShakerSort(CancellationToken token)
        {
            int begin = 0;
            int end = _length - 1;

            while (begin <= end)
            {
                int newBegin = end;
                int newEnd = begin;

                for (int i = begin; i < end; i++)
                {
                    if (_items[i].Value > _items[i+1].Value)
                    {
                        await swap(i, i + 1);
                        newEnd = i;
                    }
                    else
                    {
                        await show(i, i + 1);
                    }

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;

                end = newEnd;

                for (int i = end - 1; i >= begin; i--)
                {
                    if (_items[i].Value > _items[i + 1].Value)
                    {
                        await swap(i, i + 1);
                        newBegin = i;
                    }
                    else
                    {
                        await show(i, i + 1);
                    }

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;

                begin = newBegin + 1;
            }
        }

        private async Task oddEvenSort(CancellationToken token)
        {
            bool sorted = false;

            while(!sorted)
            {
                sorted = true;

                for (int i = 1; i < _length - 1; i += 2)
                {
                    if (_items[i].Value > _items[i+1].Value)
                    {
                        await swap(i, i + 1);
                        sorted = false;
                    }
                    else
                    {
                        await show(i, i + 1);
                    }

                    if (token.IsCancellationRequested) return;

                }

                if (token.IsCancellationRequested) return;

                for (int i = 0; i < _length - 1; i += 2)
                {
                    if (_items[i].Value > _items[i + 1].Value)
                    {
                        await swap(i, i + 1);
                        sorted = false;
                    }
                    else
                    {
                        await show(i, i + 1);
                    }

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;
            }
        }
    }
}
