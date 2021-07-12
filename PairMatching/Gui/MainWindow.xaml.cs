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
        private static IBL bl; 
        ObservableCollection<BO.Student> StudentsList;
        ObservableCollection<Tuple<BO.Student, BO.Student>> PairList;


        public MainWindow()
        {
            InitializeComponent();
            try
            {
                bl = BlFactory.GetBL();
                StudentsList = new ObservableCollection<BO.Student>(bl.StudentList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void allStudentBtn_Click(object sender, RoutedEventArgs e)
        {
            lvStudents.ItemsSource = StudentsList;
            lvStudents.Items.Refresh();
            allPairsGrig.Visibility = Visibility.Collapsed;
            allStudentGrig.Visibility = Visibility.Visible;
        }

        private void allPairsBtn_Click(object sender, RoutedEventArgs e)
        {
            PairList = new ObservableCollection<Tuple<BO.Student, BO.Student>>(bl.GetAllPairs());
            lvPairs.ItemsSource = PairList;
            allStudentGrig.Visibility = Visibility.Collapsed;
            spStudent.Visibility = Visibility.Collapsed;
            lbOpenQuestions.Visibility = Visibility.Collapsed;
            allPairsGrig.Visibility = Visibility.Visible;
        }

        private void allStudentWithoutPairBtn_Click(object sender, RoutedEventArgs e)
        {
            lvStudents.ItemsSource = new ObservableCollection<BO.Student>(from s in bl.StudentList where s.MatchTo == 0 select s); ;
            lvStudents.Items.Refresh();
            allPairsGrig.Visibility = Visibility.Collapsed;
            allStudentGrig.Visibility = Visibility.Visible;
        }

        private async void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            allStudentGrig.Visibility = Visibility.Collapsed;
            spStudent.Visibility = Visibility.Collapsed;
            lbOpenQuestions.Visibility = Visibility.Collapsed;
            allPairsGrig.Visibility = Visibility.Collapsed;
            try
            {
                pbUpdate.Visibility = Visibility.Visible;
                await Task.Run(() => bl.UpdateData());
                StudentsList = new ObservableCollection<BO.Student>(bl.StudentList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                pbUpdate.Visibility = Visibility.Collapsed;
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

        private void matchBtn_Click(object sender, RoutedEventArgs e)
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
                    bl.Match(selectedStudent, firstSelectedMatch);
                }
                return;
            }
            if (secondSelectedMatch != null)
            {
                if (MessageBox.Show($"בטוח שברצונך להתאים את {selectedStudent.Name} ל- {secondSelectedMatch.Name}?",
                       "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    bl.Match(selectedStudent, secondSelectedMatch);
                }
                return;
            }
        }

        private void manualMatchBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedList = StudentsList.Where(s => s.IsSelected);
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
                bl.Match(first, second);
            }
        }
    }
}
