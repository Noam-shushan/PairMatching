using LogicLayer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilEntities;
using BO;
using System;

namespace Gui.ViewModels
{
    public class PairsListViewModel : MainViewModelBase
    {
        public List<string> TracksNames { get; set; }

        public ObservableCollection<PairViewModel> Pairs { get; set; }

        private ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        public PairsListViewModel()
        {
            TracksNames = new List<string>(Dictionaries.PrefferdTracksDict.Values);

            var pairs = new List<Pair>(logicLayer.PairList.ToList());
            var temp = new List<PairViewModel>();
            foreach(var p in pairs)
            {
                temp.Add(new PairViewModel(p));
            }
            Pairs = new ObservableCollection<PairViewModel>(temp);
        }

        public PairsListViewModel(Predicate<Pair> pairsFilter)
        {
            TracksNames = new List<string>(Dictionaries.PrefferdTracksDict.Values);

            var pairs = logicLayer.PairList.ToList().Where(p => pairsFilter(p));
            var temp = new List<PairViewModel>();
            foreach (var p in pairs)
            {
                temp.Add(new PairViewModel(p));
            }
            Pairs = new ObservableCollection<PairViewModel>(temp);
        }
    }
}
