using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui.ViewModels
{
    public class CompareTwoStudentsViewModel
    {
        public StudentViewModel StudentFromIsrael { get; set; }
        public StudentViewModel StudentFromWorld { get; set; }

        public CompareTwoStudentsViewModel(StudentViewModel first, StudentViewModel seconde)
        {
            if (first.IsFromIsrael)
            {
                StudentFromIsrael = first;
                StudentFromWorld = seconde;
            }
            else
            {
                StudentFromIsrael = seconde;
                StudentFromWorld = first;
            }
        }
    }
}
