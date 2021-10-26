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
using BO;

namespace Gui.Controlers
{
    /// <summary>
    /// Notes view
    /// Show all the notes of student or pair
    /// Add or delete a note from a student or pair
    /// </summary>
    public partial class NoteControl : UserControl, INotifyPropertyChanged
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isStudent;
        /// <summary>
        /// Determines if the note is for a student or a pair
        /// </summary>
        public bool IsStudent
        {
            get { return _isStudent; }
            set 
            { 
                _isStudent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsStudent"));
            }
        }

        private Note _newNote = new Note();
        /// <summary>
        /// The new note to add 
        /// </summary>
        public Note NewNote 
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

        private void addNote_Click(object sender, RoutedEventArgs e)
        {
            NewNote.Date = DateTime.Now;
            if (IsStudent)
            {
                logicLayer.AddNoteToStudent(DataContext as Student, NewNote);
            }
            else
            {
                logicLayer.AddNoteToPair(DataContext as Pair, NewNote);
            }

            lvNotes.Items.Refresh();
            expder.IsExpanded = false;
            NewNote = new Note();
        }

        private void deleteNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Messages.MessageBoxConfirmation("האם אתה בטוח שברצונך למחוק הערה זו?"))
            {
                var note = (sender as Button).DataContext as Note;
                if (IsStudent)
                {
                    var student = DataContext as Student;
                    logicLayer.RemoveNoteFromStudent(student, note);
                }
                else
                {
                    var pair = DataContext as Pair;
                    logicLayer.RemoveNoteFromPair(pair, note);
                }

                lvNotes.Items.Refresh();
            }
        }
    }
}
