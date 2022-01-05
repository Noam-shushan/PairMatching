using LogicLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using Gui.Commands;
using UtilEntities;
using System.Collections.Specialized;

namespace Gui.ViewModels
{
    public class StudentsListViewModel : MainViewModelBase
    {
        private ObservableCollection<StudentViewModel> _students;
        public ObservableCollection<StudentViewModel> Students 
        {
            get
            {
                if(ListFilter != BaseFilter)
                {
                    return new ObservableCollection<StudentViewModel>
                        (_students.Where(s => ListFilter(s)));
                }
                return _students;
            }
            set
            {
                _students = value;
                OnPropertyChanged(nameof(Students));
            } 
        }

        private bool _isSelectAny = false;
        public bool IsSelectAny
        {
            get => _isSelectAny;
            set
            {
                _isSelectAny = value;
                OnPropertyChanged(nameof(IsSelectAny));
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
                OnPropertyChanged(nameof(SelectedStudent));
            }
        }

        public RelayCommand AddStudent { get; set; }

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
            Students.CollectionChanged += Students_CollectionChanged;

            AddStudent = new RelayCommand(
                student => Students.Add(student as StudentViewModel),
                student => (student as StudentViewModel).IsChanged 
            );

            SearchVM = new SearchViewModel(this);
        }

        private void Students_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var studentVM = e.NewItems[0] as StudentViewModel;
    
            if(studentVM == null)
            {
                return;
            }
            var student = studentVM.GetModel();
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                logicLayer.AddStudent(student);
            }
        }

        public override void Search(string subtext)
        {
            if (string.IsNullOrEmpty(subtext))
            {
                ListFilter = BaseFilter;
            }
            else
            {
                ListFilter = s => (s as StudentViewModel).Name.SearchText(subtext);
            }
        }
    }
}
