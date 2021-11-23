using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilEntities;

namespace Gui.ViewModels
{
    public class MatchingHistoriesViewModel
    {
        public ObservableCollection<MatchingHistoryViewModel> MatchingHistories { get; set; }

        public DateTime DateOfRegistered { get; set; }

        public MatchingHistoriesViewModel(List<StudentMatchingHistory> studentMatchingHistories)
        {
            var temp = new List<MatchingHistoryViewModel>();
            foreach(var mh in studentMatchingHistories)
            {
                temp.Add(new MatchingHistoryViewModel(mh));
            }
            MatchingHistories = new ObservableCollection<MatchingHistoryViewModel>(temp);
        }
    }
}
