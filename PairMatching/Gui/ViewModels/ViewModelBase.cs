using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui.ViewModels
{
    public abstract class ViewModelBase : NotifyPropertyChanged
    {
        public bool IsChanged { get; private set; }

        bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        protected override void OnPropertyChanged(string propName)
        {
            base.OnPropertyChanged(propName);
            IsChanged = true;
        }
    }
}
