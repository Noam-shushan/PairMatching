using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using LogicLayer;
using BO;
using System.Globalization;
using System.ComponentModel;
using Gui.Controlers;
using Gui.ViewModels;

namespace Gui
{
    /// <summary>
    /// Main window of the app
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ILogicLayer logicLayer;

        #region Dependency Properties
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isPairsUi;
        public bool IsPairsUi
        {
            get => _isPairsUi;
            set
            {
                _isPairsUi = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPairsUi"));
                if (_isPairsUi)
                {
                    IsStudentsUi = false;
                    IsStatisticsUi = false;
                }
            }
        }

        private bool _isStudentsUi;
        public bool IsStudentsUi
        {
            get => _isStudentsUi;
            set
            {
                _isStudentsUi = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsStudentsUi"));
                if (_isStudentsUi)
                {
                    IsPairsUi = false;
                    IsStatisticsUi = false;
                }
                else
                {
                    studentControl.Visibility = Visibility.Collapsed;
                }
            }

        }

        private bool _isStudentWitoutPairUi;
        public bool IsStudentWitoutPairUi
        {
            get => _isStudentWitoutPairUi;
            set
            {
                _isStudentWitoutPairUi = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsStudentWitoutPairUi"));
                if (_isStudentWitoutPairUi)
                {
                    IsStudentsUi = true;
                }
            }
        }

        bool _isLoadedData;
        public bool IsLoadedData
        {
            get => _isLoadedData;
            set
            {
                _isLoadedData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLoadedData"));
                if (_isLoadedData)
                {
                    IsStudentsUi = false;
                    IsPairsUi = false;
                    IsStatisticsUi = false;
                }
            }
        }

        bool _isStatisticsUi;
        public bool IsStatisticsUi
        {
            get => _isStatisticsUi;
            set
            {
                _isStatisticsUi = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsStatisticsUi"));
                if (_isStatisticsUi)
                {
                    IsStudentsUi = false;
                    IsPairsUi = false;
                }
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            logicLayer = LogicFactory.GetLogicFactory();
            await Initialize();
        }

        #region Update data
        static bool isUpdate = false;
        private async Task Initialize()
        {
            bool isNeedToUpdate = false;
            IsLoadedData = true;
            try
            {
                isNeedToUpdate = await logicLayer.ReadDataFromSpredsheetAsync();
            }
            catch (Exception ex2)
            {
                Messages.MessageBoxError(ex2.Message);
            }
            try
            {
                if (!isNeedToUpdate && isUpdate)
                {
                    IsLoadedData = false;
                    return;
                }

                await logicLayer.UpdateAsync();
                isUpdate = true;
                lvNotifcations.ItemsSource = Notifications.Instance.NotValidEmailAdrress;
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
            IsLoadedData = false;
        }

        private async void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            await Initialize();
        }
        #endregion

        #region UI Visibility
        private void statisticsBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStatisticsUi = true;
            statistics.DataContext = new RecordCollection(logicLayer.GetStatistics());
            //new Views.MainView().Show(); in test
        }

        #region Students UI
        private void allStudentBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStudentWitoutPairUi = false;
            IsStudentsUi = true;
            logicLayer.StudentListFilter = s => !s.IsDeleted;
            studentsListControl.SetItemsSource();
        }

        private void allStudentWithoutPairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStudentWitoutPairUi = true;
            logicLayer.StudentListFilter = s => s.IsOpenToMatch;
            studentsListControl.SetItemsSource();
        }

        public void ShowStudent(Student student)
        {
            IsStudentsUi = true;
            studentControl.DataContext = student;
            studentControl.Visibility = Visibility.Visible;
        }

        private void allStudentFromWorldBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStudentsUi = true;
            logicLayer.StudentListFilter = s => !s.IsFromIsrael;
            studentsListControl.SetItemsSource();
        }

        private void allStudentFromIsraelBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStudentsUi = true;
            logicLayer.StudentListFilter = s => s.IsFromIsrael;
            studentsListControl.SetItemsSource();
        }

        public void RefreshMyStudentsView()
        {
            if (IsStudentWitoutPairUi)
            {
                logicLayer.StudentListFilter = s => s.IsOpenToMatch;
            }
            else
            {
                logicLayer.StudentListFilter = s => !s.IsDeleted;
            }
            studentsListControl.SetItemsSource();
            studentControl.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Pairs UI
        private void allPairsBtn_Click(object sender, RoutedEventArgs e)
        {
            IsPairsUi = true;
            logicLayer.PairListFilter = p => !p.IsDeleted;
            pairListControl.SetItemSource();
        }

        private void allActivePairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsPairsUi = true;
            logicLayer.PairListFilter = p => p.IsActive;
            pairListControl.SetItemSource();
        }

        private void allStandbyPairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsPairsUi = true;
            logicLayer.PairListFilter = p => !p.IsActive;
            pairListControl.SetItemSource();
        }

        public void RefreshMyPairView()
        {
            logicLayer.PairListFilter = p => !p.IsDeleted;
            pairListControl.SetItemSource();
        }
        #endregion

        #endregion
    }
}
