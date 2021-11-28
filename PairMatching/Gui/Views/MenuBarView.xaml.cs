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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gui.ViewModels;

namespace Gui.Views
{
    /// <summary>
    /// Interaction logic for MenuBarView.xaml
    /// </summary>
    public partial class MenuBarView : UserControl
    {

        public NavigationBarViewModel CurrentVM
        {
            get { return (NavigationBarViewModel)GetValue(CurrentVMProperty); }
            set { SetValue(CurrentVMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentVM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentVMProperty =
            DependencyProperty.Register("CurrentVM", typeof(NavigationBarViewModel), typeof(MenuBarView));

        public MenuBarView()
        {
            InitializeComponent();
        }
    }
}
