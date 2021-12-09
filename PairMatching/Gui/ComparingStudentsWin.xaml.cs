using System;
using System.ComponentModel;
using System.Windows;
using BO;
using LogicLayer;
using Gui.Controlers;
using UtilEntities;
using Gui.ViewModels;

namespace Gui
{
    /// <summary>
    /// Compare between tow student
    /// Make a match is optional
    /// </summary>
    public partial class ComparingStudentsWin : Window, INotifyPropertyChanged
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        private readonly Student _fromIsrael;
        private readonly Student _fromWolrd;

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

            foreach(var s in _fromIsrael.MatchToShow)
            {
                s.IsCompereWin = true;
            }

            foreach (var s in _fromWolrd.MatchToShow)
            {
                s.IsCompereWin = true;
            }

            studentFromIsrael.DataContext = _fromIsrael;
            studentFromWorld.DataContext = _fromWolrd;
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
