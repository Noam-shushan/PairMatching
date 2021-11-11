using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;
using System.Linq;

namespace Gui
{
    class RecordCollection : ObservableCollection<Record>
    {
        public RecordCollection(List<BO.Bar> barValues)
        {
            Random rand = new Random();

            int max = barValues.Max(b => b.Value);
            foreach (BO.Bar barVal in barValues)
            {
                var color = Color.FromRgb((byte)rand.Next(120, 180), 
                    (byte)rand.Next(100, 160), 
                    (byte)rand.Next(110, 170));
                Add(new Record 
                {
                    Color = new SolidColorBrush(color),
                    Data = barVal.Value,
                    Name = barVal.BarName,
                    Percentage = GetPercentage(barVal.Value, max)
                });
            }
        }

        private double GetPercentage(int val, int max)
        {
            return (200 * val) / max;
        }

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

        public double Percentage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
