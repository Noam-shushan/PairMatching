using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui.ViewModels
{
    public class StudnetsListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<StudentViewModel> Students { get; set; }

        private StudentViewModel _selectedStudent;

        public StudentViewModel SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedStudent"));
            }
        }

        public StudnetsListViewModel(List<BO.Student> students)
        {
            var temp = new List<StudentViewModel>();
            foreach(var s in students)
            {
                temp.Add(new StudentViewModel(s));
            }
            Students = new ObservableCollection<StudentViewModel>(temp);
        }
    }
}
