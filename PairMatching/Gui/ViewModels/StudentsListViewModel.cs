using LogicLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;

namespace Gui.ViewModels
{
    public class StudentsListViewModel : MainViewModelBase
    {

        public ObservableCollection<StudentViewModel> Students { get; set; }

        private bool _isSelectAny = false;
        public bool IsSelectAny
        {
            get => _isSelectAny;
            set
            {
                _isSelectAny = value;
                OnPropertyChanged("IsSelectAny");
            }
        }

        private StudentViewModel _selectedStudent;

        public StudentViewModel SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                if(value != null)
                {
                    IsSelectAny = true;
                }
                OnPropertyChanged("SelectedStudent");
            }
        }

        private ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        public StudentsListViewModel()
        {
            var students = logicLayer.StudentList.ToList();
            var temp = new List<StudentViewModel>();
            foreach(var s in students)
            {
                temp.Add(new StudentViewModel(s));
            }
            Students = new ObservableCollection<StudentViewModel>(temp);
        }

        public StudentsListViewModel(Predicate<Student> studentsFilter)
        {
            var students = logicLayer.StudentList.ToList().Where(s => studentsFilter(s));
            var temp = new List<StudentViewModel>();
            foreach (var s in students)
            {
                temp.Add(new StudentViewModel(s));
            }
            Students = new ObservableCollection<StudentViewModel>(temp);
        }
    }
}
