using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicLayer;
using Gui.Commands;
using System.ComponentModel;

namespace Gui.ViewModels
{
    public enum NavigationCurrentView { Students, Pairs, Statistics}

    public class NavigationBarViewModel : INotifyPropertyChanged
    {
        private ViewModelBase _selectedViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public event Action ViewChanged;
        public ViewModelBase SelectedViewModel 
        { 
            get => _selectedViewModel;
            set
            {
                _selectedViewModel = value;
                ViewChanged?.Invoke();
            }
        }

        public ObservableCollection<NotValidEmailNotification> BadEmailNotification { get; set; }

        public ChangeMainViewCommand ChangeMainView { get; set; }

        public NavigationBarViewModel()
        {
            BadEmailNotification = 
                new ObservableCollection<NotValidEmailNotification>(Notifications.Instance.NotValidEmailAdrress);

            ChangeMainView = new ChangeMainViewCommand();
            ChangeMainView.ChangeMainView += ChangeMainView_ChangeMainView;
        }

        private void ChangeMainView_ChangeMainView(NavigationCurrentView? view)
        {
            switch (view)
            {
                case NavigationCurrentView.Students:
                    SelectedViewModel = new StudentsListViewModel();
                    break;
                case NavigationCurrentView.Pairs:
                    SelectedViewModel = new PairsListViewModel();
                    break;
                case NavigationCurrentView.Statistics:
                    SelectedViewModel = new StatisticsViewModel();
                    break;
            }
        }
    }
}
