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

        private ObservableCollection<PairViewModel> _pairs;
        public ObservableCollection<PairViewModel> Pairs
        {
            get
            {
                if (ListFilter != BaseFilter)
                {
                    return new ObservableCollection<PairViewModel>
                        (_pairs.Where(p => ListFilter(p)));
                }
                return _pairs;
            }
            set
            {
                _pairs = value;
                OnPropertyChanged(nameof(Pairs));
            }
        }

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

            SearchVM = new SearchViewModel(this);
        }

        public override void Search(string subtext)
        {
            if (string.IsNullOrEmpty(subtext))
            {
                ListFilter = BaseFilter;
            }
            else
            {
                ListFilter = p => (p as PairViewModel).StudentFromIsrael.Name.SearchText(subtext) ||
                                (p as PairViewModel).StudentFromWorld.Name.SearchText(subtext);
            }
        }
    }
}
