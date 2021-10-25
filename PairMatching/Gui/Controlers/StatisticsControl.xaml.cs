using LogicLayer;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gui.Controlers
{
    /// <summary>
    /// Interaction logic for StatisticsControl.xaml
    /// </summary>
    public partial class StatisticsControl : UserControl
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        public StatisticsControl()
        {
            InitializeComponent();
        }
        
        public void SetDataContext()
        {
            DataContext = new RecordCollection(new List<Bar>
            {
                new Bar
                {
                    BarName = "מספר המשתתפים הכולל",
                    Value = logicLayer.Statistics.NumberOfStudents
                },
                new Bar
                {
                    BarName = "מספר המשתתפים מישראל",
                    Value = logicLayer.Statistics.NumberOfStudentFromIsrael
                },
                new Bar
                {
                    BarName = "מספר המשתתפים מהתפוצות",
                    Value = logicLayer.Statistics.NumberOfStudentFromWorld
                },
                new Bar
                {
                    BarName = "מספר המשתתפים ללא חברותא",
                    Value = logicLayer.Statistics.NumberOfStudentsWithoutPair
                },
                new Bar
                {
                    BarName = "מספר המשתתפים מישראל ללא חברותא",
                    Value = logicLayer.Statistics.NumberOfStudentFromIsraelWithoutPair
                },
                new Bar
                {
                    BarName = "מספר המשתתפים מהתפוצות ללא חברותא",
                    Value = logicLayer.Statistics.NumberOfStudentFromWorldWithoutPair
                },
                new Bar
                {
                    BarName = "מספר החברותות",
                    Value = logicLayer.Statistics.NumberOfPair
                }
            });
        }
    }
}
