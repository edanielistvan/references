using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task cycleSort(CancellationToken token)
        {
            for (int i = 0; i < _length - 1; i++)
            {
                int pos = -1;

                do
                {
                    var item = _items[i].Value;

                    pos = i;
                    for (int j = i + 1; j < _length; j++)
                    {
                        undo();
                        if (_items[j].Value < item) pos++;
                        await show(i, j);

                        if (token.IsCancellationRequested) return;
                    }

                    if (pos == i) continue;

                    while (item == _items[pos].Value)
                    {
                        undo();
                        pos++;
                        await show(i, pos);

                        if (token.IsCancellationRequested) return;
                    }

                    await swap(pos, i);
                } while (pos != i);

                if (token.IsCancellationRequested) return;
            }
        }
    }
}
