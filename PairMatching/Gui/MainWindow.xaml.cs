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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBL bl = BlFactory.GetBL();

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
            try
            {
                IsLoadedData = true;
                await bl.ReadDataFromSpredsheetAsync();
                await bl.UpdateAsync();

                if (bl.StudentWithUnvalidEmail.Count > 0)
                {
                    var result = string.Join(",\n", from s in bl.StudentWithUnvalidEmail select s.Name);
                    Messages.MessageBoxWarning($"הכתובות מייל של התלמידים הבאים:\n {result} אינן חוקיות");
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
            finally
            {
                IsLoadedData = false;
            }
        }

        private async void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            IsLoadedData = true;
            try
            {
                await bl.ReadDataFromSpredsheetAsync();
                await bl.UpdateAsync();
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
            finally
            {
                IsLoadedData = false;
            }
        }
        #endregion

        #region UI Visibility
        private void statisticsBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStatisticsUi = true;
            spStatistics.DataContext = bl.Statistics;
        }
        #region Students UI
        private void allStudentBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStudentWitoutPairUi = false;
            IsStudentsUi = true;
            bl.StudentListFilter = s => !s.IsDeleted;
            lvStudents.ItemsSource = bl.StudentList;
            tbIsThereResultOfSearcing.Text = string.Empty;
        }

        private void allStudentWithoutPairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStudentWitoutPairUi = true;
            bl.StudentListFilter = s => s.IsOpenToMatch;
            lvStudents.ItemsSource = bl.StudentList;

            tbIsThereResultOfSearcing.Text = string.Empty;
        }

        private void lvStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsStudentsUi = true;
            var selectedStudent = lvStudents.SelectedItem as Student;
            if (selectedStudent == null)
            {
                return;
            }
            studentControl.DataContext = selectedStudent;
            studentControl.Visibility = Visibility.Visible;
        }

        private void allStudentFromWorldBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStudentsUi = true;
            bl.StudentListFilter = s => !s.IsFromIsrael;
            lvStudents.ItemsSource = bl.StudentList;
            tbIsThereResultOfSearcing.Text = string.Empty;
        }

        private void allStudentFromIsraelBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStudentsUi = true;
            bl.StudentListFilter = s => s.IsFromIsrael;
            lvStudents.ItemsSource = bl.StudentList;
            tbIsThereResultOfSearcing.Text = string.Empty;
        }

        public void RefreshMyStudentsView()
        {
            if (IsStudentWitoutPairUi)
            {
                bl.StudentListFilter = s => s.IsOpenToMatch;
            }
            else 
            {
                bl.StudentListFilter = s => !s.IsDeleted; 
            }
            lvStudents.ItemsSource = bl.StudentList;
            studentControl.Visibility = Visibility.Collapsed;
            tbIsThereResultOfSearcing.Text = string.Empty;
        }


        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Search();
            }
        }

        private void Search()
        {
            tbIsThereResultOfSearcing.Text = string.Empty;
            if (tbSearch.Text != string.Empty)
            {
                bl.SearchStudents(tbSearch.Text);
                lvStudents.ItemsSource = bl.StudentList;
                if (lvStudents.Items.IsEmpty)
                {
                    tbIsThereResultOfSearcing.Text = "אין תוצאות";
                    return;
                }
                lvStudents.Items.Refresh();
            }
            else
            {
                bl.StudentListFilter = s => !s.IsDeleted;
                lvStudents.ItemsSource = bl.StudentList;
            }
        } 
        #endregion

        #region Pair UI
        private void allPairsBtn_Click(object sender, RoutedEventArgs e)
        {
            IsPairsUi = true;
            bl.PairListFilter = p => !p.IsDeleted;
            lvPairs.ItemsSource = bl.PairList;
        }

        private void allActivePairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsPairsUi = true;
            bl.PairListFilter = p => p.IsActive;
            lvPairs.ItemsSource = bl.PairList;

        }

        private void allStandbyPairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsPairsUi = true;
            bl.PairListFilter = p => !p.IsActive;
            lvPairs.ItemsSource = bl.PairList;
        }

        private void cbTracksFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var track = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            bl.FilterPairsByTrack(track);
            lvPairs.ItemsSource = bl.PairList;
        }

        public void RefreshMyPairView()
        {
            bl.PairListFilter = p => !p.IsDeleted;
            lvPairs.ItemsSource = bl.PairList;
            tbIsThereResultOfSearcing.Text = string.Empty;
        }

        private void lvPairs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedPair = lvPairs.SelectedItem as Pair;
            if (selectedPair == null)
            {
                return;
            }
            var first = bl.GetStudent(s => s.Id == selectedPair.StudentFromIsrael.Id);
            var seconde = bl.GetStudent(s => s.Id == selectedPair.StudentFromWorld.Id);
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
            var selectedPairs = bl.PairList.Where(p => p.IsSelected);
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
                            await bl.SendEmailToPairAsync(pair, EmailTypes.ToSecretaryPairBroke);
                            await bl.RemovePairAsync(pair);
                            await bl.SendEmailToPairAsync(pair, EmailTypes.PairBroke);

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
                        await bl.RemovePairAsync(pair);
                        await bl.SendEmailToPairAsync(pair, EmailTypes.PairBroke);
                        await bl.SendEmailToPairAsync(pair, EmailTypes.ToSecretaryPairBroke);
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
            var selectedList = bl.GetAllStudentsBy(s => s.IsSelected);
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
                    int id = await bl.MatchAsync(first, second);
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

            foreach (var p in bl.PairList)
            {
                p.IsSelected = true;
            }
            lvPairs.Items.Refresh();
        }

        private void selectAllPairCB_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var p in bl.PairList)
            {
                p.IsSelected = false;
            }
            lvPairs.Items.Refresh();
        }

        private void sendEmailToManyPairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPair = bl.PairList.Where(p => p.IsSelected);
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
                    var pairToUpdate = bl.GetPair(selectedPair.Id);
                    if (_trackForEditPair != "")
                    {
                        pairToUpdate.EditPrefferdTracks(_trackForEditPair);
                        _trackForEditPair = "";
                    }
                    lvPairs.Items.Refresh();
                    
                    bl.UpdatePair(pairToUpdate);
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
                    if(Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את החברותא {selectedPair}?"))
                    {
                        await bl.RemovePairAsync(selectedPair);
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
                if(selectedPair != null)
                {
                    await bl.ActivatePairAsync(selectedPair);
                    RefreshMyPairView();
                    RefreshMyStudentsView();
                }
            }
            catch(Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }
        #endregion

        #region Student operetion
        private void addStudent_Click(object sender, RoutedEventArgs e)
        {
            new AddStudentWin().Show();
        }

        private void selectAllStudentsCB_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var s in bl.StudentList)
            {
                s.IsSelected = true;
            }
            lvStudents.Items.Refresh();
        }

        private void selectAllStudentsCB_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var s in bl.StudentList)
            {
                s.IsSelected = false;
            }
            lvStudents.Items.Refresh();
        }

        private async void deleteStudent_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = (sender as Button).DataContext as Student;
            try
            {
                if (Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את {selectedStudent.Name}?"))
                {
                    await bl.RemoveStudentAsync(selectedStudent.Id);
                    RefreshMyStudentsView();
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }

        }

        private void sendEmaileToStudentsBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = (sender as Button).DataContext as Student;
            new SendOpenEmail()
            {
                StudentName = selectedStudent.Name,
                Email = selectedStudent.Email
            }.Show();
        }

        private void sendEmaileForAllStudentsBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudents = bl.GetAllStudentsBy(s => s.IsSelected);
            if (selectedStudents.Count() == 0)
            {
                Messages.MessageBoxWarning("בחר תלמיד אחד או יותר");
                return;
            }
            new SendOpenEmail()
            {
                StudentName = selectedStudents.Count() > 5 ? "כל המסומנים" :
                                string.Join(", ", from s in selectedStudents
                                                  select s.Name),
                Email = string.Join(", ", from s in selectedStudents
                                          select s.Email)
            }.Show();
        }

        private async void sendStatusEmailForAll_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudents = bl.GetAllStudentsBy(s => s.IsSelected);
            if (selectedStudents.Count() == 0)
            {
                Messages.MessageBoxWarning("בחר תלמיד אחד או יותר");
                return;
            }
            List<Task> tasks = new List<Task>();
            try
            {
                foreach (var s in selectedStudents)
                {
                    tasks.Add(bl.SendEmailToStudentAsync(s, EmailTypes.StatusQuiz));
                }
                await Task.WhenAll(tasks);
                Messages.MessageBoxSimple("המיילים נשלחו בהצלחה!");
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }

        }

        private void editStudentBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = (sender as Button).DataContext as Student;
            try
            {
                if(selectedStudent != null)
                {
                    bl.UpdateStudent(selectedStudent);
                    Messages.MessageBoxSimple($"המשתתף {selectedStudent.Name} עודכן בהצלחה");
                }
            }
            catch(Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }
    }
    #endregion

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
