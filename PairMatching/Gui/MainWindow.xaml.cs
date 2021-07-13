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

namespace Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly IBL bl = BlFactory.GetBL();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void allStudentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (bl.StudentList == null || bl.StudentList.Count() == 0)
            {
                await bl.Update();
            }
            lvStudents.ItemsSource = bl.StudentList;
            allPairsGrig.Visibility = Visibility.Collapsed;
            allStudentGrig.Visibility = Visibility.Visible;
        }

        private void allPairsBtn_Click(object sender, RoutedEventArgs e)
        {
            lvPairs.ItemsSource = new ObservableCollection<Tuple<BO.Student, BO.Student>>(bl.GetAllPairs()); ;
            allStudentGrig.Visibility = Visibility.Collapsed;
            spStudent.Visibility = Visibility.Collapsed;
            lbOpenQuestions.Visibility = Visibility.Collapsed;
            allPairsGrig.Visibility = Visibility.Visible;
        }

        private async void allStudentWithoutPairBtn_Click(object sender, RoutedEventArgs e)
        {
            if (bl.StudentList == null || bl.StudentList.Count() == 0)
            {
                await bl.Update();             
            }
            lvStudents.ItemsSource = new ObservableCollection <BO.Student>(from s in bl.StudentList
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
            cbUpdateForEnable.IsChecked = false;
            try
            {
                pbUpdate.Visibility = Visibility.Visible;
                await Task.Run(() => bl.UpdateData());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                pbUpdate.Visibility = Visibility.Collapsed;
                cbUpdateForEnable.IsChecked = true;
            }
        }

        private void lvStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedStudent = lvStudents.SelectedItem as BO.Student;
            spStudent.DataContext = selectedStudent;
            lbOpenQuestions.DataContext = selectedStudent;
            spStudent.Visibility = Visibility.Visible;
            lbOpenQuestions.Visibility = Visibility.Visible;
        }

        private async void matchBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = lvStudents.SelectedItem as BO.Student;
            var firstSelectedMatch = cbFirstMatching.SelectedItem as BO.Student;
            var secondSelectedMatch = cbSecondeMatching.SelectedItem as BO.Student;
            if (firstSelectedMatch == null && secondSelectedMatch == null)
            {
                MessageBox.Show("בחר תלמיד ממהצעות על מנת להתאים", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(firstSelectedMatch != null)
            {

                if (MessageBox.Show($"בטוח שברצונך להתאים את {selectedStudent.Name} ל- {firstSelectedMatch.Name}?",
                       "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    await bl.Match(selectedStudent, firstSelectedMatch);
                }
                return;
            }
            if (secondSelectedMatch != null)
            {
                if (MessageBox.Show($"בטוח שברצונך להתאים את {selectedStudent.Name} ל- {secondSelectedMatch.Name}?",
                       "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    await bl.Match(selectedStudent, secondSelectedMatch);
                }
                return;
            }
        }

        private async void manualMatchBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedList = bl.StudentList.Where(s => s.IsSelected);
            if(selectedList.Count() != 2)
            {
                MessageBox.Show("בחר 2 תלמידים מהרשימה על מנת לבצע התאמה", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var first = selectedList.First();
            var second = selectedList.Last();
            if (MessageBox.Show($"בטוח שברצונך להתאים את {first.Name} ל- {second.Name}?",
                        "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await bl.Match(first, second);
            }
        }

        private async void allStudentFromWorldBtn_Click(object sender, RoutedEventArgs e)
        {
            if (bl.StudentList == null || bl.StudentList.Count() == 0)
            {
                await bl.Update();
            }
            lvStudents.ItemsSource = new ObservableCollection<BO.Student>(from s in bl.StudentList
                                                                          where s.Country != "Israel"
                                                                          select s);
            allPairsGrig.Visibility = Visibility.Collapsed;
            allStudentGrig.Visibility = Visibility.Visible;
        }

        private async void allStudentFromIsraelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (bl.StudentList == null || bl.StudentList.Count() == 0)
            {
                await bl.Update();
            }
            lvStudents.ItemsSource = new ObservableCollection<BO.Student>(from s in bl.StudentList
                                                                          where s.Country == "Israel"
                                                                          select s);
            allPairsGrig.Visibility = Visibility.Collapsed;
            allStudentGrig.Visibility = Visibility.Visible;
        }
    }
}
