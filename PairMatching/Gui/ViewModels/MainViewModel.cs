using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui.ViewModels
{
    public class MainViewModel : NotifyPropertyChanged
    {
        public NavigationBarViewModel NavigationBar { get; set; }

        public MainViewModelBase CurrentViewModel
        {
            get => NavigationBar.SelectedViewModel;
        }

        public MainViewModel()
        {
            NavigationBar = new NavigationBarViewModel()
            {
                SelectedViewModel = new StudentsListViewModel()
            };

            NavigationBar.ViewChanged += NavigationBar_ViewChanged;
        }

        private void NavigationBar_ViewChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
