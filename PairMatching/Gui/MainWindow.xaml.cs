using System;
using System.Collections.Generic;
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

namespace Gui
{
    /// <summary>
    /// Main window of the app
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

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

        #region Update data
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoadedData = true;
            try
            {
                await logicLayer.ReadDataFromSpredsheetAsync();
            }
            catch (Exception ex2)
            {
                Messages.MessageBoxError(ex2.Message);
            }
            try
            {

                await logicLayer.UpdateAsync();

                if (logicLayer.StudentWithUnvalidEmail.Count > 0)
                {
                    var result = string.Join(",\n", from s in logicLayer.StudentWithUnvalidEmail select s.Name);
                    Messages.MessageBoxWarning($"הכתובות מייל של התלמידים הבאים:\n {result} אינן חוקיות");
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
            IsLoadedData = false;
        }

        private async void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            IsLoadedData = true;
            try
            {
                await logicLayer.ReadDataFromSpredsheetAsync();
                await logicLayer.UpdateAsync();
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
            IsLoadedData = false;
        }
        #endregion

        #region UI Visibility
        private void statisticsBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStatisticsUi = true;
            spStatistics.DataContext = logicLayer.Statistics;
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
            studentControl.DataContext = student;
            studentControl.SetHeightOfOpenQA(height:270, width: 700);
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

        #region Pair UI
        private void allPairsBtn_Click(object sender, RoutedEventArgs e)
        {
            IsPairsUi = true;
            logicLayer.PairListFilter = p => !p.IsDeleted;
            lvPairs.ItemsSource = logicLayer.PairList;
        }

        private void allActivePairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsPairsUi = true;
            logicLayer.PairListFilter = p => p.IsActive;
            lvPairs.ItemsSource = logicLayer.PairList;

        }

        private void allStandbyPairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsPairsUi = true;
            logicLayer.PairListFilter = p => !p.IsActive;
            lvPairs.ItemsSource = logicLayer.PairList;
        }

        private void cbTracksFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var track = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            logicLayer.FilterPairsByTrack(track);
            lvPairs.ItemsSource = logicLayer.PairList;
        }

        public void RefreshMyPairView()
        {
            logicLayer.PairListFilter = p => !p.IsDeleted;
            lvPairs.ItemsSource = logicLayer.PairList;
            //tbIsThereResultOfSearcing.Text = string.Empty;
        }

        private void lvPairs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedPair = lvPairs.SelectedItem as Pair;
            if (selectedPair == null)
            {
                return;
            }
            var first = logicLayer.GetStudent(s => s.Id == selectedPair.StudentFromIsrael.Id);
            var seconde = logicLayer.GetStudent(s => s.Id == selectedPair.StudentFromWorld.Id);
            if (first.IsFromIsrael)
            {
                new ComparingStudentsWin(first, seconde)
                {
                    IsPair = true
                }.Show();
            }
            else
            {
                new ComparingStudentsWin(seconde, first)
                {
                    IsPair = true
                }.Show();
            }
        }
        #endregion
        #endregion

        #region Pair operation
        private async void removeMenyPairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPairs = logicLayer.PairList.Where(p => p.IsSelected);
            int numOfPairsToRem = selectedPairs.Count();
            if (numOfPairsToRem == 0)
            {
                Messages.MessageBoxWarning("בחר אחת או יותר חברותות");
                return;
            }
            try
            {
                if (numOfPairsToRem > 1)
                {
                    if (Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את {numOfPairsToRem} החברותות שבחרת?"))
                    {
                        foreach (var pair in selectedPairs)
                        {
                            await logicLayer.SendEmailToPairAsync(pair, EmailTypes.ToSecretaryPairBroke);
                            await logicLayer.RemovePairAsync(pair);
                            await logicLayer.SendEmailToPairAsync(pair, EmailTypes.PairBroke);

                        }
                        RefreshMyPairView();
                        RefreshMyStudentsView();
                    }
                }
                else
                {
                    if (Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את החברותא {selectedPairs.First()} ?"))
                    {
                        var pair = selectedPairs.First();
                        await logicLayer.RemovePairAsync(pair);
                        await logicLayer.SendEmailToPairAsync(pair, EmailTypes.PairBroke);
                        await logicLayer.SendEmailToPairAsync(pair, EmailTypes.ToSecretaryPairBroke);
                        RefreshMyPairView();
                        RefreshMyStudentsView();
                    }
                }
            }
            catch (BadPairException ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }

        private async void manualMatchBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedList = logicLayer.GetAllStudentsBy(s => s.IsSelected);
            if (selectedList.Count() != 2)
            {
                Messages.MessageBoxWarning("בחר 2 תלמידים מהרשימה על מנת לבצע התאמה");
                return;
            }

            try
            {
                var first = selectedList.First();
                var second = selectedList.Last();
                if (Messages.MessageBoxConfirmation($"בטוח שברצונך להתאים את {first.Name} ל- {second.Name}?"))
                {
                    int id = await logicLayer.MatchAsync(first, second);
                    RefreshMyStudentsView();
                    RefreshMyPairView();
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }

        private void selectAllPairCB_Checked(object sender, RoutedEventArgs e)
        {

            foreach (var p in logicLayer.PairList)
            {
                p.IsSelected = true;
            }
            lvPairs.Items.Refresh();
        }

        private void selectAllPairCB_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var p in logicLayer.PairList)
            {
                p.IsSelected = false;
            }
            lvPairs.Items.Refresh();
        }

        private void sendEmailToManyPairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPair = logicLayer.PairList.Where(p => p.IsSelected);
            int numOfPairs = selectedPair.Count();
            if (numOfPairs == 0)
            {
                Messages.MessageBoxWarning("בחר חברותא אחת או יותר");
                return;
            }
            new SendOpenEmail()
            {
                StudentName = string.Join(", ", from p in selectedPair
                                                select p.StudentFromIsrael.Name),
                Email = string.Join(", ", from p in selectedPair
                                          select p.StudentFromIsrael.Email)
            }.Show();
            new SendOpenEmail()
            {
                StudentName = string.Join(", ", from p in selectedPair
                                                select p.StudentFromWorld.Name),
                Email = string.Join(", ", from p in selectedPair
                                          select p.StudentFromWorld.Email)
            }.Show();
        }

        private void sendEmaileToPairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPair = (sender as Button).DataContext as Pair;
            if (selectedPair != null)
            {
                new SendOpenEmail()
                {
                    StudentName = selectedPair.StudentFromIsrael.Name,
                    Email = selectedPair.StudentFromIsrael.Email
                }.Show();
                new SendOpenEmail()
                {
                    StudentName = selectedPair.StudentFromWorld.Name,
                    Email = selectedPair.StudentFromWorld.Email
                }.Show();
            }
        }

        static string _trackForEditPair = "";
        private void cbTracksEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var track = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            _trackForEditPair = track;
        }

        private void editPairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPair = (sender as Button).DataContext as Pair;
            try
            {
                if (selectedPair != null)
                {
                    var pairToUpdate = logicLayer.GetPair(selectedPair.Id);
                    if (_trackForEditPair != "")
                    {
                        pairToUpdate.EditPrefferdTracks(_trackForEditPair);
                        _trackForEditPair = "";
                    }
                    lvPairs.Items.Refresh();
                    logicLayer.UpdatePair(pairToUpdate);
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }

        private async void deletePairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPair = (sender as Button).DataContext as Pair;
            try
            {
                if (selectedPair != null)
                {
                    if (Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את החברותא {selectedPair}?"))
                    {
                        await logicLayer.RemovePairAsync(selectedPair);
                        RefreshMyPairView();
                        RefreshMyStudentsView();
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }

        private async void activePairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPair = (sender as Button).DataContext as Pair;
            try
            {
                if (selectedPair != null)
                {
                    await logicLayer.ActivatePairAsync(selectedPair);
                    RefreshMyPairView();
                    RefreshMyStudentsView();
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }
        #endregion
    }

    /// <summary>
    /// Converter class for convert from ListViewItem to number of row 
    /// atending to display row number in the list that displays
    /// </summary>
    public class OrdinalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int ordinal = 0;

            if (value is ListViewItem lvi)
            {
                ListView lv = ItemsControl.ItemsControlFromItemContainer(lvi) as ListView;
                ordinal = lv.ItemContainerGenerator.IndexFromContainer(lvi) + 1;
            }

            return ordinal;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter does not provide conversion back from ordinal position to list view item
            throw new InvalidOperationException();
        }
    }
}
