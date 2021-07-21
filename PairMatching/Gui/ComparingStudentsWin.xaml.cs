using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BO;
using LogicLayer;

namespace Gui
{
    /// <summary>
    /// Interaction logic for ComparingStudentsWin.xaml
    /// </summary>
    public partial class ComparingStudentsWin : Window
    {
        public ComparingStudentsWin(Student fromIsrael, Student fromWolrd)
        {
            InitializeComponent();
            SetLayoutOfOpenQ(fromIsrael);
            SetLayoutOfOpenQ(fromWolrd);
            spStudentFromIsreal.DataContext = fromIsrael;
            spStudentFromWorld.DataContext = fromWolrd;
        }

        private void SetLayoutOfOpenQ(Student student)
        {
            Dictionary<string, string> openQA = new Dictionary<string, string>();
            foreach(var o in student.OpenQuestions)
            {
                openQA.Add(o.Key, SpliceText(o.Value));
            }
            student.OpenQuestions = openQA;
        }

        private static string SpliceText(string text)
        {
            return string.Join(Environment.NewLine, text.Split()
                .Select((word, index) => new { word, index })
                .GroupBy(x => x.index / 6)
                .Select(grp => string.Join(" ", grp.Select(x => x.word))));
        }
    }
}
