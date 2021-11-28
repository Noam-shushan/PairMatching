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
    /// Interaction logic for PairsView.xaml
    /// </summary>
    public partial class PairsView : UserControl
    {
        public PairsListViewModel CurrentPairsVM
        {
            get { return (PairsListViewModel)GetValue(CurrentPairsVMProperty); }
            set { SetValue(CurrentPairsVMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPairsVM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPairsVMProperty =
            DependencyProperty.Register("CurrentPairsVM", typeof(PairsListViewModel), typeof(PairsView));

        public PairsView()
        {
            InitializeComponent();
        }
    }
}
