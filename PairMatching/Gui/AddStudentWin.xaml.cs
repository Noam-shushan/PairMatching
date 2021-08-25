using LogicLayer;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Gui
{
    /// <summary>
    /// Interaction logic for AddStudentWin.xaml
    /// </summary>
    public partial class AddStudentWin : Window
    {
        private static readonly IBL bl = BlFactory.GetBL();

        public AddStudentWin()
        {
            InitializeComponent();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(tbCountry.Text) 
                || string.IsNullOrEmpty(tbEmail.Text)
                || string.IsNullOrEmpty(tbName.Text)
                || string.IsNullOrEmpty(tbPhone.Text))
            {
                Messages.MessageBoxWarning("אנא מלא את כל הפרטים");
                return;
            }
            try
            {
                bl.AddStudent(new BO.Student
                {
                    Name = tbName.Text,
                    Country = tbCountry.Text,
                    Email = tbEmail.Text,
                    PhoneNumber = tbPhone.Text,
                    IsSimpleStudent = true
                });
                var mainWin = Application.Current.MainWindow as MainWindow;
                mainWin.RefreshMyStudentsView();
                Close();
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }
    }
}
