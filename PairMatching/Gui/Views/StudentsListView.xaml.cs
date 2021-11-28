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
using Gui.ViewModels;
using LogicLayer;

namespace Gui.Views
{
    /// <summary>
    /// Interaction logic for StudentsListView.xaml
    /// </summary>
    public partial class StudentsListView : UserControl
    {


        public StudentsListViewModel CurrentStudentsListVM
        {
            get { return (StudentsListViewModel)GetValue(CurrentStudentsListVMProperty); }
            set { SetValue(CurrentStudentsListVMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentStudentsListVM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentStudentsListVMProperty =
            DependencyProperty.Register("CurrentStudentsListVM", typeof(StudentsListViewModel), typeof(StudentsListView));

        public StudentsListView()
        {
            InitializeComponent();
        }
    }
}
