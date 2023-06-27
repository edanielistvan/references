using System.Threading;
using System.Threading.Tasks;


namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task circleSort(CancellationToken token)
        {
            bool done = false;

            while(!done) 
            {
                done = !await doCircle(0, _length - 1, token);

                if (token.IsCancellationRequested) return;
            }
        }

        private async Task<bool> doCircle(int left, int right, CancellationToken token)
        {
            bool swapped = false;

            if (left == right) return swapped;

            int l = left; int r = right;

            while (l < r)
            {
                if (_items[l].Value > _items[r].Value)
                {
                    await swap(l, r);
                    swapped = true;               
                }
                else
                {
                    await show(l, r);
                }

                if (token.IsCancellationRequested) return false;

                l++;
                r--;
            }

            if (l == r)
            {
                if (_items[l].Value > _items[r+1].Value)
                {
                    await swap(l, r + 1);
                    swapped = true;
                }
                else
                {
                    await show(l, r + 1);
                }
            }

            if (token.IsCancellationRequested) return false;

            int mid = (right - left) / 2;
            bool firstHalf = await doCircle(left, left + mid, token);
            bool secondHalf = await doCircle(left + mid + 1, right, token);

            return swapped || firstHalf || secondHalf;
        }
    }
}

