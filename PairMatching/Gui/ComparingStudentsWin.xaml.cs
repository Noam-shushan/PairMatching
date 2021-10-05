using System;
using System.ComponentModel;
using System.Windows;
using BO;
using LogicLayer;

namespace Gui
{
    /// <summary>
    /// Compare between tow student
    /// Make a match is optional
    /// </summary>
    public partial class ComparingStudentsWin : Window, INotifyPropertyChanged
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        Student _fromIsrael;
        Student _fromWolrd;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isPair;
        /// <summary>
        /// Determines if it is a view for existing pair just to show their properties
        /// </summary>
        public bool IsPair
        {
            get => _isPair;
            set
            {
                _isPair = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPair"));
            }
        }

        public ComparingStudentsWin(Student fromIsrael, Student fromWolrd)
        {
            InitializeComponent();
            _fromIsrael = fromIsrael.Clone();
            _fromWolrd = fromWolrd.Clone();
            
            _fromIsrael.IsCompereWin = true;
            _fromWolrd.IsCompereWin = true;

            studentFromIsrael.DataContext = _fromIsrael;
            studentFromWorld.DataContext = _fromWolrd;

            studentFromIsrael.SetHeightOfOpenQA();
            studentFromWorld.SetHeightOfOpenQA();
        }

        private async void matchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(Messages.MessageBoxConfirmation("האם להתאים ולחזור לעמוד הראשי?"))
                {
                    int id = await logicLayer.MatchAsync(_fromIsrael, _fromWolrd);

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
