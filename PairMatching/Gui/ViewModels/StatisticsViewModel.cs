using LogicLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gui.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        private ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        public ObservableCollection<Record> Records { get; set; }

        public StatisticsViewModel()
        {
            Random rand = new Random();
            List<BO.Bar> barValues = logicLayer.GetStatistics();
            int max = barValues.Max(b => b.Value);
            Records = new ObservableCollection<Record>();
            foreach (BO.Bar barVal in barValues)
            {
                var color = Color.FromRgb((byte)rand.Next(120, 180),
                    (byte)rand.Next(100, 160),
                    (byte)rand.Next(110, 170));
                Records.Add(new Record
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
            return (150 * val) / max;
        }

    }

    public class Record : INotifyPropertyChanged
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
