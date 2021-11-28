using LogicLayer;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using UtilEntities;

namespace Gui.Controlers
{
    /// <summary>
    /// Interaction logic for PairsListControl.xaml
    /// </summary>
    public partial class PairsListControl : UserControl, INotifyPropertyChanged
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        public List<string> TracksNames { get; set; }

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
            TracksNames = new List<string>(Dictionaries.PrefferdTracksDict.Values);
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
                            await RemovePairAsync(pair);
                        }
                        await logicLayer.UpdateAsync();
                    }
                }
                else
                {
                    if (Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את החברותא {selectedPairs.First()} ?"))
                    {
                        var pair = selectedPairs.First();
                        await RemovePairAsync(pair);
                        await logicLayer.UpdateAsync();
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
                Email = string.Join(",", from p in selectedPair
                                          select p.StudentFromIsrael.Email)
            }.Show();
            new SendOpenEmail()
            {
                StudentName = string.Join(", ", from p in selectedPair
                                                select p.StudentFromWorld.Name),
                Email = string.Join(",", from p in selectedPair
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
            var track = (sender as ComboBox).SelectedItem.ToString();
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

        private async Task RemovePairAsync(Pair pairToRem)
        {
            IsLoadingData = true;
            try
            {
                bool notifyByEmail = false;
                if (pairToRem.IsActive && Messages.MessageBoxConfirmation("האם לשלוח מייל לחברותא על ביטול חברותא?"))
                {
                    notifyByEmail = true;
                }
                await logicLayer.RemovePairAsync(pairToRem.Id, notifyByEmail);
                var mainWin = Application.Current.MainWindow as MainWindow;
                mainWin.RefreshMyStudentsView();
                SetItemSource();
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
            IsLoadingData = false;
        }

        private async void deletePairBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPair = (sender as Button).DataContext as Pair;
            if(selectedPair != null)
            {
                if(Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך למחוק את החברותא {selectedPair}?"))
                {
                    await RemovePairAsync(selectedPair);
                    await logicLayer.UpdateAsync();
                }
            }
        }

        private async void activePairBtn_Click(object sender, RoutedEventArgs e)
        {
            IsLoadingData = true;
            var selectedPair = (sender as Button).DataContext as Pair;
            try
            {
                if (selectedPair != null)
                {
                    bool sendEmail = false;
                    if(Messages.MessageBoxConfirmation("האם ברצונך לשלוח מייל אוטומטי למשתתפים בחברותא?"))
                    {
                        sendEmail = true;
                    }
                    await logicLayer.ActivatePairAsync(selectedPair, sendEmail);
                    
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
            var track = (sender as ComboBox).SelectedItem.ToString();
            logicLayer.FilterPairsByTrack(track);
            lvPairs.ItemsSource = logicLayer.PairList;
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            if (tbSearch.Text != string.Empty)
            {
                logicLayer.SearchPairs(tbSearch.Text);
                lvPairs.ItemsSource = logicLayer.PairList;
                if (lvPairs.Items.IsEmpty)
                {
                    return;
                }
                lvPairs.Items.Refresh();
            }
            else
            {
                logicLayer.PairListFilter = p => !p.IsDeleted;
                lvPairs.ItemsSource = logicLayer.PairList;
            }
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Search();
            }
        }
    }
}
