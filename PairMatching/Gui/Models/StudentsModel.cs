using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using LogicLayer;

namespace Gui.Models
{
    public class StudentsModel
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        public List<Student> Students { get; set; }

        public StudentsModel()
        {
            Students = new List<Student>(logicLayer.StudentList);
        }
    }
}
