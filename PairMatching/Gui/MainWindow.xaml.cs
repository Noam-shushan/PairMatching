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
using System.ComponentModel;

namespace Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBL bl = BlFactory.GetBL();

        public ObservableCollection<Pair> PairsList { get; set; } = new ObservableCollection<Pair>();

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isStudentWitoutPairUi;
        public bool IsStudentWitoutPairUi
        {
            get => _isStudentWitoutPairUi;
            set
            {
                _isStudentWitoutPairUi = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsStudentWitoutPairUi"));
            }
        }

        bool _isLoaded;
        public bool IsLoaded
        {
            get => _isLoaded;
            set
            {
                _isLoaded = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLoaded"));
            }
        }

        public MainWindow()
        {
            // TODO: create data template for the info of student and change that by the country
            // TODO: make a better solution for the Visablity of the panels, maybe using trigers 
            // and data tamplates
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                IsLoaded = true;
                await bl.ReadDataFromSpredsheetAsync();
                await bl.UpdateAsync();
                await Task.Run(() =>
                {
                    var pairsTemp = bl.GetAllPairs();
                    if (pairsTemp != null)
                    {
                        PairsList = new ObservableCollection<Pair>(pairsTemp);
                    }
                });
                IsLoaded = false;
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }

        private void allStudentBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStudentWitoutPairUi = false;
            lvStudents.ItemsSource = bl.StudentList;
            spAllPairs.Visibility = Visibility.Collapsed;
            tbIsThereResultOfSearcing.Text = string.Empty;
            spAllStudents.Visibility = Visibility.Visible;
        }

        private void allPairsBtn_Click(object sender, RoutedEventArgs e)
        {
            lvPairs.ItemsSource = PairsList;
            
            spAllStudents.Visibility = Visibility.Collapsed;
            tbIsThereResultOfSearcing.Text = string.Empty;
            studentControl.Visibility = Visibility.Collapsed;
            spAllPairs.Visibility = Visibility.Visible;
        }

        private async void removePairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPairs = PairsList.Where(p => p.IsSelected);
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
                        foreach (var p in selectedPairs)
                        {
                            await bl.RemovePairAsync(p);
                        }
                        RefreshMyPairView();
                        RefreshMyStudentsView();
                    }
                }
                else
                {
                    if (Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את החברותא {selectedPairs.First()} ?"))
                    {
                        await bl.RemovePairAsync(selectedPairs.First());
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

        private void allStudentWithoutPairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsStudentWitoutPairUi = true;
            lvStudents.ItemsSource = new ObservableCollection<Student>(bl.GetAllStudentsBy(s => s.MatchTo == 0));
            
            spAllPairs.Visibility = Visibility.Collapsed;
            tbIsThereResultOfSearcing.Text = string.Empty;
            spAllStudents.Visibility = Visibility.Visible;
        }

        private async void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            IsLoaded = true;
            tbIsThereResultOfSearcing.Text = string.Empty;
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
                //pbUpdate.Visibility = Visibility.Collapsed;
                IsLoaded = false;
            }
        }

        private void lvStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedStudent = lvStudents.SelectedItem as Student;
            if(selectedStudent == null)
            {
                return;
            }
            studentControl.DataContext = selectedStudent;
            studentControl.Visibility = Visibility.Visible;
        }

        private async void manualMatchBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedList = bl.GetAllStudentsBy(s => s.IsSelected);
            if(selectedList.Count() != 2)
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
                    var newPair = PairsList.FirstOrDefault(p => p.Id == id);
                    if(newPair != null)
                    {
                        await bl.SendEmailToPairAsync(newPair, EmailTypes.YouGotPair);
                        await bl.SendEmailToPairAsync(newPair, EmailTypes.ToSecretaryNewPair);
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }

        private void allStudentFromWorldBtn_Click(object sender, RoutedEventArgs e)
        {
            lvStudents.ItemsSource = new ObservableCollection<Student>(bl.GetAllStudentsBy(s => !s.IsFromIsrael));
            spAllPairs.Visibility = Visibility.Collapsed;
            tbIsThereResultOfSearcing.Text = string.Empty;
            spAllStudents.Visibility = Visibility.Visible;
        }

        private void allStudentFromIsraelBtn_Click(object sender, RoutedEventArgs e)
        {
            lvStudents.ItemsSource = new ObservableCollection<Student>(bl.GetAllStudentsBy(s => s.IsFromIsrael));
            spAllPairs.Visibility = Visibility.Collapsed;
            tbIsThereResultOfSearcing.Text = string.Empty;
            spAllStudents.Visibility = Visibility.Visible;
        }

        public void RefreshMyPairView()
        {
            PairsList = new ObservableCollection<Pair>(bl.GetAllPairs());
            lvPairs.ItemsSource = PairsList;
            tbIsThereResultOfSearcing.Text = string.Empty;
        }

        public void RefreshMyStudentsView()
        {
            lvStudents.ItemsSource = IsStudentWitoutPairUi ?
                new ObservableCollection<Student>(bl.GetAllStudentsBy(s => s.MatchTo == 0)) 
                : bl.StudentList;

            studentControl.Visibility = Visibility.Collapsed;
            tbIsThereResultOfSearcing.Text = string.Empty;
        }


        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void lvPairs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedPair = lvPairs.SelectedItem as Pair; 
            if(selectedPair == null)
            {
                return;
            }
            var first = bl.StudentList.FirstOrDefault(s => s.Id == selectedPair.StudentFromIsrael.Id);
            var seconde = bl.StudentList.FirstOrDefault(s => s.Id == selectedPair.StudentFromWorld.Id);
            if (first.IsFromIsrael)
            {
                new ComparingStudentsWin(first, seconde).Show();
            }
            else
            {
                new ComparingStudentsWin(seconde, first).Show();
            }
        }

        private  void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                Search();
            }
        }

        private void Search()
        {
            tbIsThereResultOfSearcing.Text = string.Empty;
            if (tbSearch.Text != string.Empty)
            {
                lvStudents.ItemsSource = new ObservableCollection<Student>(bl.SearchStudents(tbSearch.Text));
                if (lvStudents.Items.IsEmpty)
                {
                    tbIsThereResultOfSearcing.Text = "אין תוצאות";
                    return;
                }
                lvStudents.Items.Refresh();
                spAllPairs.Visibility = Visibility.Collapsed;
                spAllStudents.Visibility = Visibility.Visible;
            }
        }

        private async void sendEmailToThePairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPair = PairsList.Where(p => p.IsSelected);
            int numOfPairs = selectedPair.Count();
            if (numOfPairs == 0 || numOfPairs > 1)
            {
                Messages.MessageBoxWarning("בחר חברותא אחת");
                return;
            }

            try
            {
                await bl.SendEmailToPairAsync(selectedPair.First(), EmailTypes.YouGotPair);
                MessageBox.Show("המייל נשלח בהצלחה!");
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }

        private void selectAllPairCB_Checked(object sender, RoutedEventArgs e)
        {

            foreach(var p in PairsList)
            {
                p.IsSelected = true;
            }
            lvPairs.Items.Refresh();
        }

        private void selectAllPairCB_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var p in PairsList)
            {
                p.IsSelected = false;
            }
            lvPairs.Items.Refresh();
        }

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
                if(Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את {selectedStudent.Name}?"))
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
            if(selectedStudents.Count() == 0)
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
            foreach(var s in selectedStudents)
            {
                tasks.Add(bl.SendEmailToStudentAsync(s, EmailTypes.StatusQuiz));
            }
            await Task.WhenAll(tasks);
        }
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

    public class BooleanAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Any(v => (v is bool b) && b);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
