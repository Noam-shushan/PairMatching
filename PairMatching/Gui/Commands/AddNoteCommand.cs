using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UtilEntities;

namespace Gui.Commands
{
    public class AddNoteCommand : ICommand
    {
        public event Action<Note> AddNote;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var note = parameter as Note;
            if (AddNote != null && note != null)
            {
                AddNote(note);
            }
        }
    }
}
