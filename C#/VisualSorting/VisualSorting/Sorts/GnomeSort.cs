using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task gnomeSort(CancellationToken token)
        {
            int pos = 0;

            while (pos < _length)
            {
                if (pos == 0 || _items[pos].Value >= _items[pos - 1].Value)
                {
                    pos++;
                    await show(pos, pos - 1);
                }
                else
                {
                    await swap(pos, pos - 1);
                    pos--;
                }

                if (token.IsCancellationRequested) return;
            }
        }
    }
}
