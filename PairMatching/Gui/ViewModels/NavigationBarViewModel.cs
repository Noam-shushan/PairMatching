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
    public enum NavigationCurrentView 
    { 
        Students, 
        Pairs,
        ActivePairs,
        NotActivePairs,
        Statistics, 
        StudentFromIsrael, 
        StudentFormWorld,
        StudentWithoutPair
    }

    public class NavigationBarViewModel : INotifyPropertyChanged
    {
        private MainViewModelBase _selectedViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public event Action ViewChanged;
        public MainViewModelBase SelectedViewModel 
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

        private StudentsListViewModel studentsVM;
        private PairsListViewModel pairsVM;
        private StatisticsViewModel statisticsVM;

        public NavigationBarViewModel()
        {
            BadEmailNotification = 
                new ObservableCollection<NotValidEmailNotification>(Notifications.Instance.NotValidEmailAdrress);

            studentsVM = new StudentsListViewModel();
            pairsVM = new PairsListViewModel();
            statisticsVM = new StatisticsViewModel();

            ChangeMainView = new ChangeMainViewCommand();
            ChangeMainView.ChangeMainView += ChangeMainView_ChangeMainView;
        }

        private void ChangeMainView_ChangeMainView(NavigationCurrentView? view)
        {
            switch (view)
            {
                case NavigationCurrentView.Students:
                    studentsVM.ListFilter = s => true;
                    SelectedViewModel = studentsVM;
                    break;
                case NavigationCurrentView.Pairs:
                    SelectedViewModel = pairsVM;
                    break;
                case NavigationCurrentView.Statistics:
                    SelectedViewModel = statisticsVM;
                    break;
                case NavigationCurrentView.StudentWithoutPair:
                    studentsVM.ListFilter = s => (s as StudentViewModel).IsOpenToMatch;
                    SelectedViewModel = studentsVM;
                    break;
                case NavigationCurrentView.StudentFromIsrael:
                    studentsVM.ListFilter = s => (s as StudentViewModel).IsFromIsrael;
                    SelectedViewModel = studentsVM;
                    break;
                case NavigationCurrentView.StudentFormWorld:
                    studentsVM.ListFilter = s => !(s as StudentViewModel).IsFromIsrael;
                    SelectedViewModel = studentsVM;
                    break;
            }
        }
    }
}
