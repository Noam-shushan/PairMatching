using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilEntities;
using Gui.Commands;

namespace Gui.ViewModels
{
    public class NotesViewModel : ViewModelBase
    {
        public ObservableCollection<Note> Notes { get; set; }

        private bool _isStudent;
        /// <summary>
        /// Determines if the note is for a student or a pair
        /// </summary>
        public bool IsStudent
        {
            get => _isStudent; 
            set
            {
                _isStudent = value;
                OnPropertyChanged(nameof(IsStudent));
            }
        }

        public AddNoteCommand AddNote;

        public NotesViewModel(List<Note> notes)
        {
            Notes = new ObservableCollection<Note>(notes);
            AddNote = new AddNoteCommand();
            AddNote.AddNote += AddNote_AddNote;
        }

        private void AddNote_AddNote(Note obj)
        {
            if (IsStudent)
            {
                // add to studnet
            }
        }
    }
}
