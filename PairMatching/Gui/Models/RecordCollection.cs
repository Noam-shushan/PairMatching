using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;

namespace Gui
{
    class RecordCollection : ObservableCollection<Record>
    {
        public RecordCollection(List<Bar> barValues)
        {
            Random rand = new Random();

            foreach (Bar barVal in barValues)
            {
                var color = Color.FromRgb((byte)rand.Next(120, 180), 
                    (byte)rand.Next(100, 160), 
                    (byte)rand.Next(110, 170));
                Add(new Record(barVal.Value, new SolidColorBrush(color), barVal.BarName));
            }
        }

    }

    class Bar
    {
        public string BarName { set; get; }

        public int Value { set; get; }
    }

    class Record : INotifyPropertyChanged
    {
        public Brush Color { set; get; }

        public string Name { set; get; }

        private int _data;
        public int Data
        {
            get => _data;
            set
            {
                if (_data != value)
                {
                    _data = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Record(int value, Brush color, string name)
        {
            Data = value;
            Color = color;
            Name = name;
        }
    }
}
