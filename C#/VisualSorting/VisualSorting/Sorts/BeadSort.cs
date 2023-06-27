using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private int Sum(int[][] beads, int i, CancellationToken token)
        {
            int sum = 0;

            for (int j = 0; j < beads[i].Length; j++)
            {
                sum += beads[i][j];

                if (token.IsCancellationRequested) return 0;
            }

            return sum;
        }

        private async Task beadSort(CancellationToken token)
        {
            int min = 0, max = 0;

            for (int i = 1; i < _length; i++)
            {
                await show(min, max);

                if (_items[i].Value < _items[min].Value) min = i;
                if (_items[i].Value > _items[max].Value) max = i;

                if (token.IsCancellationRequested) return;

                await show(i, max);
            }

            int max_v = _items[max].Value;
            int min_v = _items[min].Value;

            int[][] beads = new int[_length][];

            for (int i = 0; i < _length; i++)
            {
                beads[i] = new int[max_v - min_v + 1];

                for (int j = 0; j < _items[i].Value; j++)
                {
                    beads[i][j] = 1;

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;

                await show(i, i);
            }

            for (int j = 0; j < max_v - min_v + 1; j++)
            {
                int sum = 0;
                for (int i = 0; i < _length; i++)
                {
                    sum += beads[i][j];
                    beads[i][j] = 0;

                    if (token.IsCancellationRequested) return;
                }

                for (int i = _length - 1; i >= _length - sum; i--)
                {
                    beads[i][j] = 1;

                    if (token.IsCancellationRequested) return;
                }

                for (int i = 0; i < _length; i++)
                {
                    int ssum = Sum(beads, i, token);
                    _items[i].Value = ssum;

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;

                await show(j, j);
            }
        }
    }
}
