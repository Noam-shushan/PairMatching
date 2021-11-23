using Gui.ViewModels;
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

namespace Gui.Views
{
    /// <summary>
    /// Interaction logic for CompareTwoStudentView.xaml
    /// </summary>
    public partial class CompareTwoStudentsView : Window
    {
        public CompareTwoStudentsViewModel CurrentTwoStudentsVM { get; set; }

        public CompareTwoStudentsView(CompareTwoStudentsViewModel twoStudentsViewModel)
        {
            InitializeComponent();
            CurrentTwoStudentsVM = twoStudentsViewModel;
            CurrentTwoStudentsVM.StudentFromIsrael.IsCompereWin = true;
            CurrentTwoStudentsVM.StudentFromWorld.IsCompereWin = true;
        }
    }
}
