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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gui.Views
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        public SearchViewModel CurrntVM
        {
            get { return (SearchViewModel)GetValue(CurrntVMProperty); }
            set { SetValue(CurrntVMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrntVM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrntVMProperty =
            DependencyProperty.Register("CurrntVM", typeof(SearchViewModel), typeof(SearchView));

        public SearchView()
        {
            InitializeComponent();
        }
    }
}
