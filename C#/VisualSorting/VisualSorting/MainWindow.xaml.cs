using System;
using System.Windows;
using System.Windows.Media;

namespace VisualSorting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataManager _context;

        public MainWindow()
        {        
            InitializeComponent();
            _context = new DataManager();
            DataContext = _context;

            _context.NotSorted += HandleNotSorted;

            RenderOptions.SetCachingHint(GraphControl, CachingHint.Cache);

            RenderOptions.SetCacheInvalidationThresholdMinimum(GraphControl, 0.5);
            RenderOptions.SetCacheInvalidationThresholdMaximum(GraphControl, 2.0);
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _context.CanvasSizeChanged(sender, e);
        }
        
        private void HandleNotSorted(object? sender, EventArgs e)
        {
            MessageBox.Show("Something is not working correctly.", "Array not sorted", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }
    }
}
