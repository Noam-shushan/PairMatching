using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui.ViewModels
{
    public abstract class MainViewModelBase : NotifyPropertyChanged
    {
        protected static Predicate<ViewModelBase> BaseFilter { get; } = m => true;

        Predicate<ViewModelBase> _listFilter = BaseFilter;
        public Predicate<ViewModelBase> ListFilter
        {
            get => _listFilter;
            set
            {
                _listFilter = value;
                OnPropertyChanged(nameof(ListFilter));
            }
        }

        public abstract void Search(string v);

        public SearchViewModel SearchVM { get; set; }
    }
}
