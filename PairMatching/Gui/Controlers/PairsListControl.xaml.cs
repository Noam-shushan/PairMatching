using LogicLayer;
using BO;
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
using System.Globalization;
using System.ComponentModel;

namespace Gui.Controlers
{
    /// <summary>
    /// Interaction logic for PairsListControl.xaml
    /// </summary>
    public partial class PairsListControl : UserControl, INotifyPropertyChanged
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isLoadingData;

        public bool IsLoadingData
        {
            get { return _isLoadingData; }
            set 
            { 
                _isLoadingData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLoadingData"));
            }
        }


        public PairsListControl()
        {
            InitializeComponent();
        }

        public void SetItemSource()
        {
            lvPairs.ItemsSource = logicLayer.PairList;
            lvPairs.Items.Refresh();
        }

        private void lvPairs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedPair = lvPairs.SelectedItem as Pair;
            if (selectedPair == null)
            {
                return;
            }
            var first = logicLayer.GetStudent(s => s.Id == selectedPair.StudentFromIsrael.Id);
            var seconde = logicLayer.GetStudent(s => s.Id == selectedPair.StudentFromWorld.Id);
            if (first.IsFromIsrael)
            {
                new ComparingStudentsWin(first, seconde)
                {
                    IsPair = true
                }.Show();
            }
            else
            {
                new ComparingStudentsWin(seconde, first)
                {
                    IsPair = true
                }.Show();
            }
        }

        private async void removeMenyPairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsLoadingData = true;
            var selectedPairs = logicLayer.PairList.Where(p => p.IsSelected);
            int numOfPairsToRem = selectedPairs.Count();
            if (numOfPairsToRem == 0)
            {
                Messages.MessageBoxWarning("בחר אחת או יותר חברותות");
                return;
            }
            try
            {
                if (numOfPairsToRem > 1)
                {
                    if (Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את {numOfPairsToRem} החברותות שבחרת?"))
                    {
                        foreach (var pair in selectedPairs)
                        {
                            await logicLayer.SendEmailToPairAsync(pair, EmailTypes.ToSecretaryPairBroke);
                            await logicLayer.RemovePairAsync(pair);
                            await logicLayer.SendEmailToPairAsync(pair, EmailTypes.PairBroke);

                        }

                        var mainWin = Application.Current.MainWindow as MainWindow;
                        mainWin.RefreshMyStudentsView();
                        SetItemSource();
                    }
                }
                else
                {
                    if (Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את החברותא {selectedPairs.First()} ?"))
                    {
                        var pair = selectedPairs.First();
                        await logicLayer.RemovePairAsync(pair);
                        await logicLayer.SendEmailToPairAsync(pair, EmailTypes.PairBroke);
                        await logicLayer.SendEmailToPairAsync(pair, EmailTypes.ToSecretaryPairBroke);
                        
                        var mainWin = Application.Current.MainWindow as MainWindow;
                        mainWin.RefreshMyStudentsView();
                        SetItemSource();
                    }
                }
            }
            catch (BadPairException ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
            IsLoadingData = false;
        }

        private void selectAllPairCB_Checked(object sender, RoutedEventArgs e)
        {

            foreach (var p in logicLayer.PairList)
            {
                p.IsSelected = true;
            }
            lvPairs.Items.Refresh();
        }

        private void selectAllPairCB_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var p in logicLayer.PairList)
            {
                p.IsSelected = false;
            }
            lvPairs.Items.Refresh();
        }

        private void sendEmailToManyPairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPair = logicLayer.PairList.Where(p => p.IsSelected);
            int numOfPairs = selectedPair.Count();
            if (numOfPairs == 0)
            {
                Messages.MessageBoxWarning("בחר חברותא אחת או יותר");
                return;
            }
            new SendOpenEmail()
            {
                StudentName = string.Join(", ", from p in selectedPair
                                                select p.StudentFromIsrael.Name),
                Email = string.Join(", ", from p in selectedPair
                                          select p.StudentFromIsrael.Email)
            }.Show();
            new SendOpenEmail()
            {
                StudentName = string.Join(", ", from p in selectedPair
                                                select p.StudentFromWorld.Name),
                Email = string.Join(", ", from p in selectedPair
                                          select p.StudentFromWorld.Email)
            }.Show();
        }

        private void sendEmaileToPairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPair = (sender as Button).DataContext as Pair;
            if (selectedPair != null)
            {
                new SendOpenEmail()
                {
                    StudentName = selectedPair.StudentFromIsrael.Name,
                    Email = selectedPair.StudentFromIsrael.Email
                }.Show();
                new SendOpenEmail()
                {
                    StudentName = selectedPair.StudentFromWorld.Name,
                    Email = selectedPair.StudentFromWorld.Email
                }.Show();
            }
        }

        static string _trackForEditPair = "";

        private void cbTracksEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var track = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            _trackForEditPair = track;
        }

        private void editPairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPair = (sender as Button).DataContext as Pair;
            try
            {
                if (selectedPair != null)
                {
                    var pairToUpdate = logicLayer.GetPair(selectedPair.Id);
                    if (_trackForEditPair != "")
                    {
                        pairToUpdate.EditPrefferdTracks(_trackForEditPair);
                        _trackForEditPair = "";
                    }
                    lvPairs.Items.Refresh();
                    logicLayer.UpdatePair(pairToUpdate);
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }

        private async void deletePairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsLoadingData = true;
            var selectedPair = (sender as Button).DataContext as Pair;
            try
            {
                if (selectedPair != null)
                {
                    if (Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את החברותא {selectedPair}?"))
                    {
                        await logicLayer.RemovePairAsync(selectedPair);
                        
                        var mainWin = Application.Current.MainWindow as MainWindow;
                        mainWin.RefreshMyStudentsView();
                        SetItemSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
            IsLoadingData = false;
        }

        private async void activePairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsLoadingData = true;
            var selectedPair = (sender as Button).DataContext as Pair;
            try
            {
                if (selectedPair != null)
                {
                    await logicLayer.ActivatePairAsync(selectedPair);
                    
                    var mainWin = Application.Current.MainWindow as MainWindow;

                    SetItemSource();
                    mainWin.RefreshMyStudentsView();
                }
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
            IsLoadingData = false;
        }

        private void cbTracksFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var track = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            logicLayer.FilterPairsByTrack(track);
            lvPairs.ItemsSource = logicLayer.PairList;
        }
    }

    /// <summary>
    /// Converter class for convert from ListViewItem to number of row 
    /// atending to display row number in the list that displays
    /// </summary>
    public class OrdinalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int ordinal = 0;

            if (value is ListViewItem lvi)
            {
                ListView lv = ItemsControl.ItemsControlFromItemContainer(lvi) as ListView;
                ordinal = lv.ItemContainerGenerator.IndexFromContainer(lvi) + 1;
            }

            return ordinal;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter does not provide conversion back from ordinal position to list view item
            throw new InvalidOperationException();
        }
    }
}
