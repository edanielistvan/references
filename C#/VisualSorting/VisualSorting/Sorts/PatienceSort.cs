using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VisualSorting
{
    public partial class DataManager
    {
        private async Task patienceSort(CancellationToken token)
        {
            int n = _length;
            int length = 1;
            List<int> piletops = new List<int> { 0 };

            for (int i = 1; i < n; i++)
            {
                int current = _items[i].Value;

                int j = 0;
                while (j < length && _items[piletops[j]].Value < current) 
                {
                    await show(piletops[j], i);
                    j++;

                    if (token.IsCancellationRequested) return;
                }

                int min = j; j++;
				while(j < length)
                {
					if (_items[piletops[j]].Value >= current)
					{
						if (_items[piletops[min]].Value > _items[piletops[j]].Value) { min = j; }
                        await show(min, j);
                    }
                    await show(piletops[j], i);
					j++;

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;

                if (min >= length)
				{
					piletops.Add(i); length++;
				}
				else
				{
					for (int a = min; a<length; a++)
					{
						piletops[a]++;

                        if (token.IsCancellationRequested) return;
                    }

                    int s = _items[i].Value; 
                    j = i;
                    await show(i, i);

                    while (j > piletops[min])
                    {
                        _items[j].Value = _items[j - 1].Value;
                        await show(j, j - 1);
                        j--;

                        if (token.IsCancellationRequested) return;
                    }

                    _items[j].Value = s;
                    await show(j, j);
				}

                if (token.IsCancellationRequested) return;
            }

			int l = 0;
            while (l < n)
            {
                int min = 0;
                for (int i = 1; i < length; i++)
                {
                    if (_items[piletops[min]].Value > _items[piletops[i]].Value) min = i;
                    await show(piletops[i], piletops[min]);

                    if (token.IsCancellationRequested) return;
                }

                int curr = piletops[min];

                for (int a = min - 1; a > -1; a--)
                {
                    piletops[a]++;

                    if (token.IsCancellationRequested) return;
                }

                if ((min > 0 && piletops[min] == piletops[min - 1]) || (min == 0 && piletops[min] == l))
                {
                    piletops.RemoveAt(min);
                    length--;
                }

                if (token.IsCancellationRequested) return;

                int s = _items[curr].Value; int j = curr;
                await show(j, j);

                while (j > l)
                {
                    _items[j].Value = _items[j - 1].Value;
                    await show(j, j - 1);
                    j--;

                    if (token.IsCancellationRequested) return;
                }

                if (token.IsCancellationRequested) return;

                _items[j].Value = s;
                await show(j, j);
                l++;
            }
		}
    }
}
