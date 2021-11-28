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
    /// Interaction logic for StatisticsView.xaml
    /// </summary>
    public partial class StatisticsView : UserControl
    {
        public StatisticsViewModel CurrentVM
        {
            get { return (StatisticsViewModel)GetValue(CurrentVMProperty); }
            set { SetValue(CurrentVMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentVM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentVMProperty =
            DependencyProperty.Register("CurrentVM", typeof(StatisticsViewModel), typeof(StatisticsView));

        public StatisticsView()
        {
            InitializeComponent();
            CurrentVM = new StatisticsViewModel();
        }
    }
}
