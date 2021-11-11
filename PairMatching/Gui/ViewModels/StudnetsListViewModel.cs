using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui.ViewModels
{
    public class StudnetsListViewModel
    {
        public ObservableCollection<StudentViewModel> Students { get; set; }

        public StudnetsListViewModel(List<StudentViewModel> students)
        {
            Students = new ObservableCollection<StudentViewModel>(students);
        }
    }
}
