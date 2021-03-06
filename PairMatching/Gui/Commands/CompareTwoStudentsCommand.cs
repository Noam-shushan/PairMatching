using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Gui.Converters;
using Gui.ViewModels;

namespace Gui.Commands
{
    public class CompareTwoStudentsCommand : ICommand
    {
        public event Action<TempPair> Compare;

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
            var studentsToCompare = parameter as TempPair;
            if(Compare != null && studentsToCompare != null)
            {
                Compare(studentsToCompare);
            }
        }
    }
}
