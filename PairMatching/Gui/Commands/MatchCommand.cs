using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BO;

namespace Gui.Commands
{
    public class MatchCommand : ICommand
    {
        public event Func<Student, Student, Task> MathcAsync;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            // Add logic here
            return true;
        }

        public async void Execute(object parameter)
        {
            if(MathcAsync != null)
            {
                await MathcAsync(null, null);
            }
        }
    }
}
