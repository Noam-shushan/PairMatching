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
    /// Interaction logic for NoteView.xaml
    /// </summary>
    public partial class NotesView : UserControl
    {
        public NotesViewModel CurrentNotesVM
        {
            get { return (NotesViewModel)GetValue(CurrentNotesVMProperty); }
            set { SetValue(CurrentNotesVMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentNotesVM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentNotesVMProperty =
            DependencyProperty.Register("CurrentNotesVM", typeof(NotesViewModel), typeof(NotesView));

        public NotesView()
        {
            InitializeComponent();
        }
    }
}
