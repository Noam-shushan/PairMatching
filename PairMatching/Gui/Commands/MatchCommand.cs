using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BO;
using Gui.Converters;

namespace Gui.Commands
{
    public class MatchCommand : ICommand
    {
        public event Func<Student, Student, Task<bool>> MathcAsync;

        public bool IsMatch { get; set; } = false;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            // Add logic here
            return !IsMatch;
        }

        public async void Execute(object parameter)
        {
            var tempPair = parameter as TempPair;

            if (MathcAsync != null && tempPair != null)
            {
                IsMatch = await MathcAsync(tempPair.First, tempPair.Second);
            }
        }
    }
}
