using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {        
        private async Task exchangeSort(CancellationToken token)
        {
            for (int i = 0; i < _length - 1; i++)
            {
                for (int j = i + 1; j < _length; j++)
                {
                    undo();
                    if (_items[i].Value > _items[j].Value) await swap(i, j);
                    else await show(i, j);

                    if (token.IsCancellationRequested) return;
                }
            }
        }
    }
}
