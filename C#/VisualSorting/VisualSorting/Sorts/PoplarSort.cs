using System;
using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private int hyperFloor(int n)
        {
            return (int)Math.Pow(2, Math.Floor(Math.Log(n) / Math.Log(2)));
        }

        private async Task unInsertionSort(int first, int last, CancellationToken token)
        {
            for (int cur = first + 1; cur != last; ++cur)
            {
                int sift = cur;
                int sift_1 = cur - 1;

                if (_items[sift].Value < _items[sift_1].Value)
                {
                    int tmp = _items[sift].Value;
                    do
                    {
                        await show(sift, sift_1);
                        _items[sift].Value = _items[sift_1].Value;

                        if (token.IsCancellationRequested) return;
                    } while (--sift != first && tmp < _items[--sift_1].Value);

                    _items[sift].Value = tmp;
                    await show(sift, sift_1);
                }

                if (token.IsCancellationRequested) return;
            }
        }

        private async Task chInsertionSort(int first, int last, CancellationToken token)
        {
            if (first == last) return;
            if (token.IsCancellationRequested) return;
            await unInsertionSort(first, last, token);
        }

        private async Task sift(int first, int size, CancellationToken token)
        {
            if (size < 2) return;

            int root = first + (size - 1);
            int child_root1 = root - 1;
            int child_root2 = first + (size / 2 - 1);

            while (true)
            {
                int max_root = root;

                if (token.IsCancellationRequested) return;

                if (_items[max_root].Value < _items[child_root1].Value)
                {
                    await show(max_root, child_root1);
                    max_root = child_root1;               
                }

                if (_items[max_root].Value < _items[child_root2].Value)
                {
                    await show(max_root, child_root2);
                    max_root = child_root2;
                }

                if (token.IsCancellationRequested) return;

                if (max_root == root) return;

                await swap(root, max_root);

                size /= 2;
                if (size < 2) return;

                root = max_root;
                child_root1 = root - 1;
                child_root2 = max_root - (size - size / 2);
            }
        }

        private async Task pop_heap_with_size(int first, int last, int size, CancellationToken token)
        {
            int poplar_size = hyperFloor(size + 1) - 1;
            int last_root = last - 1;
            int bigger = last_root;
            int bigger_size = poplar_size;

            int it = first;
            while (true)
            {
                int root = it + poplar_size - 1;

                if (root == last_root) break;

                await show(bigger, root);

                if (_items[bigger].Value < _items[root].Value)
                {
                    bigger = root;
                    bigger_size = poplar_size;
                }

                if (token.IsCancellationRequested) return;

                it = root + 1;
                size -= poplar_size;
                poplar_size = hyperFloor(size + 1) - 1;
            }

            if (token.IsCancellationRequested) return;

            if (bigger != last_root)
            {
                await swap(bigger, last_root);
                await sift(bigger - (bigger_size - 1), bigger_size, token);
            }
        }

        private async Task make_heap(int first, int last, CancellationToken token)
        {
            int size = last - first;
            if (size < 2) return;

            int small_poplar_size = 15;
            if (size <= small_poplar_size)
            {
                await unInsertionSort(first, last, token);
                return;
            }

            if (token.IsCancellationRequested) return;

            int poplar_level = 1;

            int it = first;
            int next = it + small_poplar_size;
            while (true)
            {
                await unInsertionSort(it, next, token);

                int poplar_size = small_poplar_size;

                for (int i = (poplar_level & (0 - poplar_level)) >> 1; i != 0; i >>= 1)
                {
                    it -= poplar_size;
                    poplar_size = 2 * poplar_size + 1;
                    await sift(it, poplar_size, token);
                    ++next;

                    if (token.IsCancellationRequested) return;
                }

                if ((last - next) <= small_poplar_size)
                {
                    await chInsertionSort(next, last, token);
                    return;
                }

                if (token.IsCancellationRequested) return;

                it = next;
                next += small_poplar_size;
                ++poplar_level;
            }
        }

        private async Task sort_heap(int first, int last, CancellationToken token)
        {
            int size = last - first;
            if (size < 2) return;

            do
            {
                await pop_heap_with_size(first, last, size, token);
                --last;
                --size;

                if (token.IsCancellationRequested) return;
            } while (size > 1);
        }

        public async Task poplarSort(CancellationToken token)
        {
            await make_heap(0, _length, token);
            await sort_heap(0, _length, token);
        }
    }
}
