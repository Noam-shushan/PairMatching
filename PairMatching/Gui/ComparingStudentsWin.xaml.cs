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
        private readonly IBL bl = BlFactory.GetBL();

        Student _fromIsrael;
        Student _fromWolrd;

        public ComparingStudentsWin(Student fromIsrael, Student fromWolrd)
        {
            InitializeComponent();
            _fromIsrael = fromIsrael.Clone();
            _fromWolrd = fromWolrd.Clone();
            
            _fromIsrael.IsCompereWin = true;
            _fromWolrd.IsCompereWin = true;
            SetLayoutOfOpenQ(_fromIsrael);
            SetLayoutOfOpenQ(_fromWolrd);
            
            studentFromIsreal.DataContext = _fromIsrael;
            studentFromWorld.DataContext = _fromWolrd;
        }

        private void SetLayoutOfOpenQ(Student student)
        {
            Dictionary<string, string> openQA = new Dictionary<string, string>();
            foreach(var o in student.OpenQuestionsDict)
            {
                openQA.Add(o.Key, o.Value.SpliceText(6));
            }
            student.OpenQuestionsDict = openQA;
        }

        private async void matchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(Messages.MessageBoxConfirmation("האם להתאים ולחזור לעמוד הראשי?"))
                {
                    await bl.MatchAsync(_fromIsrael, _fromWolrd);

                    var mainWin = Application.Current.MainWindow as MainWindow;
                    mainWin.RefreshMyStudentsView();
                    mainWin.RefreshMyPairView();
                    Close();
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }
    }
}
