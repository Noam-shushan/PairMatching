using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Gui.ViewModels;

namespace Gui.Commands
{
    public class UpdateStudentCommand : ICommand
    {
        public event Action<StudentViewModel> Update;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            //TODO add logic whene yuo can update
            return true;
        }

        public void Execute(object parameter)
        {
            var student = parameter as StudentViewModel;
            if(Update != null && student != null)
            {
                Update(student);
            }

        }
    }
}
