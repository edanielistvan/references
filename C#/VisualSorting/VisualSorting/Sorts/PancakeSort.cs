using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task pancakeSort(CancellationToken token)
        {
            for (int size = _length; size > 1; size--)
            {
                int max = 0;

                for (int i = 1; i < size; i++)
                {
                    if (_items[i].Value > _items[max].Value) max = i;

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;

                await show(max, max);

                if (max != size - 1)
                {
                    await flip(0, max);
                    await flip(0, size - 1);
                }
            }
        }
    }
}