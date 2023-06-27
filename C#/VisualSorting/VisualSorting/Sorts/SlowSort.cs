using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task slowSortInit(CancellationToken token)
        {
            if (token.IsCancellationRequested) return;
            await slowSort(0, _length - 1, token);
        }

        private async Task slowSort(int i, int j, CancellationToken token)
        {
            if (i >= j || token.IsCancellationRequested) return;

            int m = (i + j) / 2;
            await slowSort(i, m, token);
            await slowSort(m + 1, j, token);

            if (_items[j].Value < _items[m].Value)
            {
                await swap(j, m);
            }
            else
            {
                await show(j, m);
            }

            await slowSort(i, j - 1, token);
        }
    }
}
