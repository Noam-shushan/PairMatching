using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {


        private bool _isMainUI;
        public bool IsMainUI 
        { 
            get => _isMainUI;
            set
            {
                _isMainUI = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsMainUI"));
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
