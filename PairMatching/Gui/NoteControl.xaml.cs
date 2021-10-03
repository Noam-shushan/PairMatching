using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using LogicLayer;

namespace Gui
{
    /// <summary>
    /// Interaction logic for NoteControl.xaml
    /// </summary>
    public partial class NoteControl : UserControl, INotifyPropertyChanged
    {
        static readonly IBL bl = BlFactory.GetBL();

        public BO.Student Student { get; set; }

        private bool _isStudent;
        public bool IsStudent
        {
            get { return _isStudent; }
            set 
            { 
                _isStudent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsStudent"));
            }
        }

        private BO.Note _newNote = new BO.Note();
        public BO.Note NewNote 
        {
            get => _newNote;
            set
            {
                _newNote = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewNote"));
            } 
        }

        public NoteControl()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void addNote_Click(object sender, RoutedEventArgs e)
        {
            NewNote.Date = DateTime.Now;
            if (IsStudent)
            {
                bl.AddNoteToStudent(DataContext as BO.Student, NewNote);
            }
            else
            {
                bl.AddNoteToPair(DataContext as BO.Pair, NewNote);
            }

            lvNotes.Items.Refresh();
            expder.IsExpanded = false;
            NewNote = new BO.Note() { Author = "", Text = "", Date = new DateTime() };
        }
    }
}
