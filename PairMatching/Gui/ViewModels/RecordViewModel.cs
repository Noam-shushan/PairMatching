using System.ComponentModel;
using System.Windows.Media;

namespace Gui.ViewModels
{
    public class RecordViewModel : ViewModelBase
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
                    OnPropertyChanged(nameof(Data));
                }
            }
        }

        public double Percentage { get; set; }
    }
}
