using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LogicLayer;
using BO;
using System.Globalization;

namespace Gui
{
    public class OrdinalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            ListViewItem lvi = value as ListViewItem;
            int ordinal = 0;

            if (lvi != null)
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly IBL bl = BlFactory.GetBL();

        private ObservableCollection<Pair> pairsList;

        bool isStudentWitoutPairUi;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void allStudentBtn_Click(object sender, RoutedEventArgs e)
        {
            isStudentWitoutPairUi = false;
            if (bl.StudentList.Count() == 0)
            {
                await bl.UpdateAsync();
            }
            lvStudents.ItemsSource = bl.StudentList;
            allPairsGrig.Visibility = Visibility.Collapsed;
            allStudentGrig.Visibility = Visibility.Visible;
        }

        private void allPairsBtn_Click(object sender, RoutedEventArgs e)
        {
            pairsList = new ObservableCollection<Pair>(bl.GetAllPair());
            lvPairs.ItemsSource = pairsList;
            allStudentGrig.Visibility = Visibility.Collapsed;
            spStudent.Visibility = Visibility.Collapsed;
            spBtnForMatch.Visibility = Visibility.Collapsed;
            lbOpenQuestions.Visibility = Visibility.Collapsed;
            allPairsGrig.Visibility = Visibility.Visible;
        }

        private async void removePairBtn_Click(object sender, RoutedEventArgs e)
        {
            if(pairsList == null)
            {
                return;
            }
            var selectedPairs = pairsList.Where(p => p.IsSelected);
            int numOfPairsToRem = selectedPairs.Count();
            if (numOfPairsToRem == 0)
            {
                MessageBoxWarning("בחר אחת או יותר חברותות");
                return;
            }
            try
            {
                if (numOfPairsToRem > 1)
                {
                    if (MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את {numOfPairsToRem} החברותות שבחרת?"))
                    {
                        foreach (var p in selectedPairs)
                        {
                            await bl.RemovePairAsync(p);
                        }
                        RefreshMyPairView();
                        RefreshMyView();
                    }
                }
                else
                {
                    if (MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את החברותא {selectedPairs.First()} ?"))
                    {
                        await bl.RemovePairAsync(selectedPairs.First());
                        RefreshMyPairView();
                        RefreshMyView();
                    }
                }
            }
            catch (BadPairException ex)
            {
                MessageBoxError(ex.Message);
            }
        }

        private async void allStudentWithoutPairBtn_Click(object sender, RoutedEventArgs e)
        {
            isStudentWitoutPairUi = true;
            if (bl.StudentList.Count() == 0)
            {
                await bl.UpdateAsync();
            }
            lvStudents.ItemsSource = new ObservableCollection<Student>(from s in bl.StudentList
                                                                        where s.MatchTo == 0
                                                                        select s);
            allPairsGrig.Visibility = Visibility.Collapsed;
            allStudentGrig.Visibility = Visibility.Visible;
        }

        private async void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            allStudentGrig.Visibility = Visibility.Collapsed;
            spStudent.Visibility = Visibility.Collapsed;
            lbOpenQuestions.Visibility = Visibility.Collapsed;
            allPairsGrig.Visibility = Visibility.Collapsed;
            spBtnForMatch.Visibility = Visibility.Collapsed;
            cbUpdateForEnable.IsChecked = false;
            try
            {
                pbUpdate.Visibility = Visibility.Visible;
                await Task.Run(() => bl.UpdateDataAsync());
            }
            catch (Exception ex)
            {
                MessageBoxError(ex.Message);
            }
            finally
            {
                pbUpdate.Visibility = Visibility.Collapsed;
                cbUpdateForEnable.IsChecked = true;
            }
        }

        private void lvStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedStudent = lvStudents.SelectedItem as Student;
            if(selectedStudent == null)
            {
                return;
            }
            spStudent.DataContext = selectedStudent;
            lbOpenQuestions.DataContext = selectedStudent;
            spStudent.Visibility = Visibility.Visible;
            lbOpenQuestions.Visibility = Visibility.Visible;
            spBtnForMatch.Visibility = Visibility.Visible;
        }


        private async void matchBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = lvStudents.SelectedItem as Student;
            var firstSelectedMatch = cbFirstMatching.SelectedItem as SuggestStudent;
            var secondSelectedMatch = cbSecondeMatching.SelectedItem as SuggestStudent;
            if (firstSelectedMatch == null && secondSelectedMatch == null)
            {
                MessageBoxWarning("בחר תלמיד ממהצעות על מנת להתאים");
                return;
            }
            try
            {
                if (firstSelectedMatch != null)
                {

                    if (MessageBoxConfirmation($"בטוח שברצונך להתאים את {selectedStudent.Name} ל- {firstSelectedMatch.SuggestStudentName}?"))
                    {
                        var first = bl.StudentList.FirstOrDefault(s => s.Id == firstSelectedMatch.SuggestStudentId);
                        await bl.MatchAsync(selectedStudent, first);
                        RefreshMyView();
                    }
                    return;
                }
                if (secondSelectedMatch != null)
                {
                    if (MessageBoxConfirmation($"בטוח שברצונך להתאים את {selectedStudent.Name} ל- {secondSelectedMatch.SuggestStudentName}?"))
                    {
                        var seconde = bl.StudentList.FirstOrDefault(s => s.Id == secondSelectedMatch.SuggestStudentId);
                        await bl.MatchAsync(selectedStudent, seconde);
                        RefreshMyView();
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBoxError(ex.Message);
            }
        }

        private async void manualMatchBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedList = bl.StudentList.Where(s => s.IsSelected);
            if(selectedList.Count() != 2)
            {
                MessageBoxWarning("בחר 2 תלמידים מהרשימה על מנת לבצע התאמה");
                return;
            }

            try
            {
                var first = selectedList.First();
                var second = selectedList.Last();
                if (MessageBoxConfirmation($"בטוח שברצונך להתאים את {first.Name} ל- {second.Name}?"))
                {
                    await bl.MatchAsync(first, second);
                    RefreshMyView();
                }
            }
            catch (Exception ex)
            {
                MessageBoxError(ex.Message);
            }
        }

        private async void allStudentFromWorldBtn_Click(object sender, RoutedEventArgs e)
        {
            if (bl.StudentList.Count() == 0)
            {
                await bl.UpdateAsync();
            }
            lvStudents.ItemsSource = new ObservableCollection<Student>(from s in bl.StudentList
                                                                        where s.Country != "Israel"
                                                                        select s);
            allPairsGrig.Visibility = Visibility.Collapsed;
            allStudentGrig.Visibility = Visibility.Visible;
        }

        private async void allStudentFromIsraelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (bl.StudentList.Count() == 0)
            {
                await bl.UpdateAsync();
            }
            lvStudents.ItemsSource = new ObservableCollection<Student>(from s in bl.StudentList
                                                                        where s.Country == "Israel"
                                                                        select s);
            allPairsGrig.Visibility = Visibility.Collapsed;
            allStudentGrig.Visibility = Visibility.Visible;
        }

        private void RefreshMyPairView()
        {
            pairsList = new ObservableCollection<Pair>(bl.GetAllPair());
            lvPairs.ItemsSource = pairsList;
        }

        private void RefreshMyView()
        {
            lvStudents.ItemsSource = isStudentWitoutPairUi ? 
                new ObservableCollection<Student>(from s in bl.StudentList
                                                where s.MatchTo == 0
                                                select s) : bl.StudentList;
            spStudent.Visibility = Visibility.Collapsed;
            lbOpenQuestions.Visibility = Visibility.Collapsed;
            spBtnForMatch.Visibility = Visibility.Collapsed;
        }

        private void MessageBoxError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK,
                MessageBoxImage.Error, MessageBoxResult.None,
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }

        private void MessageBoxWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK,
                MessageBoxImage.Warning, MessageBoxResult.None, 
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }

        private bool MessageBoxConfirmation(string message)
        {
            return MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo,
                        MessageBoxImage.Question, MessageBoxResult.OK,
                        MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading)
                            == MessageBoxResult.Yes;
        }

        private async void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            await Search();
        }

        private void clearCBFirstMatchBtn_Click(object sender, RoutedEventArgs e)
        {
            cbFirstMatching.SelectedItem = null;
        }

        private void clearCBSecondeMatchBtn_Click(object sender, RoutedEventArgs e)
        {
            cbSecondeMatching.SelectedItem = null;
        }

        private void compFirstMatchBtn_Click(object sender, RoutedEventArgs e)
        {
            if(cbFirstMatching.SelectedItem != null && lvStudents.SelectedItem != null)
            {
                var frist = bl.StudentList
                    .FirstOrDefault(s => s.Id == (cbFirstMatching.SelectedItem as SuggestStudent).SuggestStudentId);
                var seconde = lvStudents.SelectedItem as Student;
                if(frist.Country == "Israel")
                {
                    new ComparingStudentsWin(frist, seconde).Show();
                }
                else
                {
                    new ComparingStudentsWin(seconde, frist).Show();
                }
            }
        }

        private void compSecondeMatchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (cbSecondeMatching.SelectedItem != null && lvStudents.SelectedItem != null)
            {
                var first = bl.StudentList
                    .FirstOrDefault(s => s.Id == (cbSecondeMatching.SelectedItem as SuggestStudent).SuggestStudentId);
                var seconde = lvStudents.SelectedItem as Student;
                if (first.Country == "Israel")
                {
                    new ComparingStudentsWin(first, seconde).Show();
                }
                else
                {
                    new ComparingStudentsWin(seconde, first).Show();
                }
            }
        }

        private void lvPairs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedPair = lvPairs.SelectedItem as Pair;
            if(selectedPair == null)
            {
                return;
            }
            var first = bl.StudentList.FirstOrDefault(s => s.Id == selectedPair.FirstStudent.Id);
            var seconde = bl.StudentList.FirstOrDefault(s => s.Id == selectedPair.SecondStudent.Id);
            if (first.Country == "Israel")
            {
                new ComparingStudentsWin(first, seconde).Show();
            }
            else
            {
                new ComparingStudentsWin(seconde, first).Show();
            }
        }

        private void tbSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            searchBtn.Focus();
        }

        private async void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                await Search();
            }
        }

        private async Task Search()
        {
            if (tbSearch.Text != string.Empty)
            {
                if (bl.StudentList.Count() == 0)
                {
                    await bl.UpdateAsync();
                }
                lvStudents.ItemsSource = new ObservableCollection<Student>(from s in bl.StudentList
                                                                           where s.Name.StartsWith(tbSearch.Text, StringComparison.InvariantCultureIgnoreCase)
                                                                           select s);
                lvStudents.Items.Refresh();
                allPairsGrig.Visibility = Visibility.Collapsed;
                allStudentGrig.Visibility = Visibility.Visible;
            }
        }
    }
}
