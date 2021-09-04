using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using BO;
using LogicLayer;

namespace Gui
{
    /// <summary>
    /// Interaction logic for StudentControl.xaml
    /// </summary>
    public partial class StudentControl : UserControl
    {
        private static readonly IBL bl = BlFactory.GetBL();

        public StudentControl()
        {
            InitializeComponent();
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
            ShowComparingStudentsWin(cbFirstMatching.SelectedItem);
        }

        private void compSecondeMatchBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowComparingStudentsWin(cbSecondeMatching.SelectedItem);
        }

        private void ShowComparingStudentsWin(object fristStudent)
        {
            if (fristStudent != null && DataContext != null)
            {
                var first = bl.GetStudent((fristStudent as SuggestStudent).SuggestStudentId);
                var seconde = DataContext as Student;
                if (first.IsFromIsrael)
                {
                    new ComparingStudentsWin(first, seconde).Show();
                }
                else
                {
                    new ComparingStudentsWin(seconde, first).Show();
                }
            }
        }

        private async void matchSecondeBtn_Click(object sender, RoutedEventArgs e)
        {
            var secondSelectedMatch = cbSecondeMatching.SelectedItem as SuggestStudent;
            await MatchAsync(secondSelectedMatch);
        }

        private async void matchFirstBtn_Click(object sender, RoutedEventArgs e)
        {
            var firstSelectedMatch = cbFirstMatching.SelectedItem as SuggestStudent;
            await MatchAsync(firstSelectedMatch);
        }

        private async Task MatchAsync(SuggestStudent suggestStudent)
        {
            var selectedStudent = DataContext as Student;
            if (suggestStudent == null)
            {
                Messages.MessageBoxWarning("בחר תלמיד ממהצעות על מנת להתאים");
                return;
            }
            try
            {
                if (Messages.MessageBoxConfirmation($"בטוח שברצונך להתאים את {selectedStudent.Name} ל- {suggestStudent.SuggestStudentName}?"))
                {
                    var first = bl.GetStudent(suggestStudent.SuggestStudentId);
                    int id = await bl.MatchAsync(selectedStudent, first);
                    
                    var mainWin = Application.Current.MainWindow as MainWindow;
                    mainWin.RefreshMyStudentsView();
                    mainWin.RefreshMyPairView();
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }

        private void Expander_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
