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
    /// Interaction logic for MatchingHistoriesView.xaml
    /// </summary>
    public partial class MatchingHistoriesView : UserControl
    {
        public MatchingHistoriesViewModel CurrentMatchingHistoriesVM
        {
            get { return (MatchingHistoriesViewModel)GetValue(CurrentMatchingHistoriesVMProperty); }
            set { SetValue(CurrentMatchingHistoriesVMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentMatchingHistoriesVM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentMatchingHistoriesVMProperty =
            DependencyProperty.Register("CurrentMatchingHistoriesVM", typeof(MatchingHistoriesViewModel), 
                typeof(MatchingHistoriesView));

        public MatchingHistoriesView()
        {
            InitializeComponent();
        }
    }
}
