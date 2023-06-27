using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace VisualSorting
{
    public class RectItem : INotifyPropertyChanged
    {
        private int _value;
        private int _index;

        private double _width;
        private double _height;
        private SolidColorBrush? _fillColor;

        public int Index { get => _index; private set { _index = value; OnPropertyChanged(nameof(X)); } }
        public int Value { get => _value; set { _value = value; OnPropertyChanged(nameof(Height)); } }
        public double X { get => _index * _width; }
        public double Y { get => 0; }
        public double Width { get => _width; set { _width = value; OnPropertyChanged(nameof(Width)); OnPropertyChanged(nameof(X)); } }
        public double Height { get => _height * _value; set { _height = value; OnPropertyChanged(nameof(Height)); } }

        public SolidColorBrush? FillColor { get => _fillColor; set { _fillColor = value; OnPropertyChanged(nameof(FillColor)); } }

        public event PropertyChangedEventHandler? PropertyChanged;

        public RectItem(int index, int value, double width, double height, SolidColorBrush fillColor)
        {
            Index = index;
            Value = value;
            Width = width;
            Height = height;
            FillColor = fillColor;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] String? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
