using Kameleonok.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VisualSorting
{
    public partial class DataManager : INotifyPropertyChanged
    {
        private ObservableCollection<string> _sorts;
        private Func<CancellationToken, Task>[] _sortMethods;
        private ObservableCollection<RectItem> _items;
        private Random _rnd;
        private Stopwatch _sw;

        private int _selected;
        private int[] _prevSelect;
        private int _delay = 1;
        private int _length = 0;

        private double _width = 0;
        private double _height = 0;

        CancellationToken _token;
        CancellationTokenSource _source;

        public ObservableCollection<string> Sorts { get => _sorts; }
        public ObservableCollection<RectItem> RectItems { get => _items; }
        public int SelectedSort { get => _selected; set => _selected = value; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? NotSorted;

        public DelegateCommand StopCommand { get; private set; }
        public DelegateCommand ShuffleCommand { get; private set; }
        public DelegateCommand SortCommand { get; private set; }

        public DataManager()
        {
            _selected = 0;
            _sorts = new ObservableCollection<string> { 
                "Exchange Sort", "Selection Sort (Min)", "Selection Sort (Max)", "Selection Sort (Mixed)", "Cycle Sort",
                "Quicksort (Window)", "Quicksort (Better window)", "Quicksort (Pushing)", "Quicksort (Find)", 
                "Quicksort (Perfect Pivot)", "Quicksort (Lomuto)", "Quicksort (Hoare)", "Quicksort (Dual-pivot)", 
                "Quicksort (Multi-pivot)", "Comb Sort", "Insertion Sort", "Insertion Sort (Optimized)", "Bubble Sort", 
                "Bubble Sort (Optimized)", "Cocktail Shaker Sort", "Odd-Even Sort", "Strand Sort", "Gnome Sort", "Shell Sort", 
                "Merge Sort", "Heap Sort", "Ternary Heap Sort", "Bottom-up Heap Sort", "Poplar Sort", "Smooth Sort", "Tim Sort",
                "Circle Sort", "Patience Sort", "Bead Sort", "Radix Sort (LSD, Base 2)", "Radix Sort (LSD, Base 3)",
                "Radix Sort (LSD, Base 8)", "Radix Sort (LSD, Base 10)", "Radix Sort (LSD, Base 16)", 
                "Radix Sort (MSD, Base 2)", "Radix Sort (MSD, Base 3)", "Radix Sort (MSD, Base 8)", 
                "Radix Sort (MSD, Base 10)", "Radix Sort (MSD, Base 16)", "Batcher's Bitonic Sort", "Pancake Sort",
                "Pairwise Sorting Network", "Bogo Sort", "Bozo Sort", "Swap Bogo Sort", "Bubble Bogo Sort",
                "Slow Sort", "Odd-Even Mergesort"
            };

            _sortMethods = new Func<CancellationToken, Task>[] {
                exchangeSort, selectionSortMin, selectionSortMax, selectionSortMixed, cycleSort, quickSortInitWindow, 
                quickSortInitBetterWindow, quickSortInitPushing, quickSortInitFind, quickSortInitPerfectPivot, 
                quickSortInitLomuto, quickSortInitHoare, quickSortInitDualPivot, quickSortInitMultiPivot, combSort, 
                insertionSort, insertionSortOptimized, bubbleSort, bubbleSortOptimized, cocktailShakerSort, oddEvenSort, 
                strandSort, gnomeSort, shellSort, mergeInitSort, heapSort, ternaryHeapSort, bottomUpHeapSort, poplarSort, 
                smoothSort, timSort, circleSort, patienceSort, beadSort, radixSortLSDBase2, radixSortLSDBase3,
                radixSortLSDBase8, radixSortLSDBase10, radixSortLSDBase16, radixSortMSDBase2, radixSortMSDBase3,
                radixSortMSDBase8, radixSortMSDBase10, radixSortMSDBase16, bitonicInitSort, pancakeSort, 
                pairwiseSortingNetworkInit, bogoSort, bozoSort, swapBogoSort, bogoBubbleSort, slowSortInit,
                OddEvenMergeSortInit
            };

            _items = new ObservableCollection<RectItem>();
            _rnd = new Random();
            _prevSelect = new int[0];
            _sw = new Stopwatch();

            _source = new CancellationTokenSource();
            _token = _source.Token;

            ShuffleCommand = new DelegateCommand(param => Task.Run(shuffle));
            SortCommand = new DelegateCommand(param => Task.Run(sort));
            StopCommand = new DelegateCommand(param => Task.Run(doStop));

            createRects(1000);
        }

        private void doStop()
        {
            _source.Cancel();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] String? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async Task sort()
        {
            await _sortMethods[SelectedSort](_token);

            if (!_token.IsCancellationRequested && !sorted())
            {
                NotSorted?.Invoke(this, new EventArgs());
            }

            _source = new CancellationTokenSource();
            _token = _source.Token;

            undo();
        }

        public void CanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _width = e.NewSize.Width / _length;
            _height = e.NewSize.Height / _length;

            for (int i = 0; i < _length; i++)
            {
                _items[i].Height = _height;
                _items[i].Width = _width;
            }

            OnPropertyChanged("RectItems");
        }

        private async Task shuffle()
        {
            for (int i = 0; i < _length; i++)
            {
                int j = i;

                while (j == i)
                {
                    j = _rnd.Next(0, _length);
                }

                await swap(i, j);
            }

            undo();
        }

        private void select(int[] values)
        {
            _prevSelect = values;

            for (int i = 0; i < values.Length; i++)
            {
                _items[values[i]].FillColor = Brushes.Red;
            }
        }

        private void undo()
        {
            for (int i = 0; i < _prevSelect.Length; i++)
            {
                _items[_prevSelect[i]].FillColor = Brushes.Green;
            }

            _prevSelect = new int[0];
        }

        private async Task flip(int l, int r)
        {
            while (l < r)
            {
                await swap(l, r);
                l++;
                r--;
            }
        }

        private async Task swap(int i, int j)
        {
            if (_prevSelect.Length > 0) undo();

            if (i == j) { await show(i, j); return; }

            int temp = _items[i].Value;
            _items[i].Value = _items[j].Value;
            _items[j].Value = temp;

            select(new int[] { i, j });

            await Task.Run(() => wait(_delay));
        }

        private async Task show(int i, int j)
        {
            if (_prevSelect.Length > 0) undo();
            
            select(new int[] { i, j });

            await Task.Run(() => wait(_delay));
        }

        private void createRects(int number)
        {
            _items.Clear();

            for (int i = 0; i < number; i++)
            {
                _items.Add(new RectItem(i, i + 1, _width, _height, Brushes.Green));
            }

            _length = number;

            OnPropertyChanged("RectItems");
        }

        private void wait(int ms)
        {
            _sw = Stopwatch.StartNew();

            while (_sw.ElapsedMilliseconds < ms) {}

            _sw.Stop();
        }
    }
}
