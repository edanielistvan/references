using System;
using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private int getParent(int i)
        {
            return Convert.ToInt32(Math.Ceiling((double)i / 2.0)) - 1;
        }

        private int getLeftChild(int i)
        {
            return i * 2 + 1;
        }

        private int getRightChild(int i)
        {
            return (i + 1) * 2;
        }

        private async Task heapSort(CancellationToken token)
        {
            int start = getParent(_length - 1);

            while (start >= 0)
            {
                await siftDown(start, _length - 1, token);
                start--;

                if (token.IsCancellationRequested) return;
            }

            int end = _length - 1;

            while (end > 0)
            {
                await swap(end, 0);
                end--;

                await siftDown(0, end, token);

                if (token.IsCancellationRequested) return;
            }
        }

        private async Task siftDown(int start, int end, CancellationToken token)
        {
            int root = start;

            while (getLeftChild(root) <= end)
            {
                int child = getLeftChild(root);
                int temp = root;

                await show(child, temp);

                if (_items[temp].Value < _items[child].Value)
                {
                    temp = child;
                }

                await show(child, temp);

                if (child + 1 <= end && _items[temp].Value < _items[child+1].Value)
                {
                    temp = child + 1;
                    await show(child + 1, temp);
                }              

                if (temp == root)
                {
                    return;
                }
                else
                {
                    await swap(root, temp);
                    root = temp;
                }

                if (token.IsCancellationRequested) return;
            }
        }

        private int getTernaryParent(int i)
        {
            return Convert.ToInt32(Math.Ceiling((double)i / 3.0)) - 1;
        }

        private int getLeftTernaryChild(int i)
        {
            return i * 3 + 1;
        }

        private int getMiddleTernaryChild(int i)
        {
            return i * 3 + 2;
        }

        private int getRightTernaryChild(int i)
        {
            return (i + 1) * 3;
        }

        private async Task ternaryHeapSort(CancellationToken token)
        {
            int start = getTernaryParent(_length - 1);

            while (start >= 0)
            {
                await siftTernaryDown(start, _length - 1, token);
                start--;

                if (token.IsCancellationRequested) return;
            }

            int end = _length - 1;

            while (end > 0)
            {
                await swap(end, 0);
                end--;

                await siftTernaryDown(0, end, token);

                if (token.IsCancellationRequested) return;
            }
        }

        private async Task siftTernaryDown(int start, int end, CancellationToken token)
        {
            int root = start;

            while (getLeftTernaryChild(root) <= end)
            {
                int child = getLeftTernaryChild(root);
                int temp = root;

                await show(child, temp);

                if (_items[temp].Value < _items[child].Value)
                {
                    temp = child;
                }

                await show(child, temp);

                if (child + 1 <= end && _items[temp].Value < _items[child + 1].Value)
                {
                    temp = child + 1;
                    await show(child + 1, temp);
                }

                if (child + 2 <= end && _items[temp].Value < _items[child + 2].Value)
                {
                    temp = child + 2;
                    await show(child + 2, temp);
                }

                if (temp == root)
                {
                    return;
                }
                else
                {
                    await swap(root, temp);
                    root = temp;
                }

                if (token.IsCancellationRequested) return;
            }
        }

        private async Task bottomUpHeapSort(CancellationToken token)
        {
            int start = getParent(_length - 1);

            while (start >= 0)
            {
                await siftBottomUpDown(start, _length - 1, token);
                start--;

                if (token.IsCancellationRequested) return;
            }

            int end = _length - 1;

            while (end > 0)
            {
                await swap(end, 0);
                end--;

                await siftBottomUpDown(0, end, token);

                if (token.IsCancellationRequested) return;
            }
        }

        private async Task<int> leafSearch(int i, int end, CancellationToken token)
        {
            int j = i;

            while (getRightChild(j) <= end)
            {
                int right = getRightChild(j);
                int left = getLeftChild(j);

                if (_items[right].Value > _items[left].Value)
                {
                    await show(j, right);
                    j = right;                    
                }
                else
                {
                    await show(j, left);
                    j = left;
                }

                if (token.IsCancellationRequested) return -1;
            }

            if (getLeftChild(j) <= end) j = getLeftChild(j);
            await show(j, j);

            return j;
        }

        private async Task siftBottomUpDown(int start, int end, CancellationToken token)
        {
            int j = await leafSearch(start, end, token);

            while (_items[start].Value > _items[j].Value)
            {
                j = getParent(j);

                if (token.IsCancellationRequested) return;
            }

            int x = _items[j].Value;
            _items[j].Value = _items[start].Value;
            await show(j, start);

            while (j > start)
            {
                int p = getParent(j);

                int temp = x;
                x = _items[p].Value;
                _items[p].Value = temp;

                await show(p, j);

                j = p;

                if (token.IsCancellationRequested) return;
            }
        }
    }
}
