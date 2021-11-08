using Gui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilEntities;

namespace Gui.ViewModels
{
    public class NotesViewModel : INotifyPropertyChanged
    {
        private NotesModel notesModel;

        public ObservableCollection<Note> Notes { get; set; }

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

        public NotesViewModel()
        {
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
