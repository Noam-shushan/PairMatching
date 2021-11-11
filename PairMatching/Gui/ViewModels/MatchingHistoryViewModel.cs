using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilEntities;

namespace Gui.ViewModels
{
    public class MatchingHistoryViewModel
    {
        private StudentMatchingHistory _matchingHistory;

        public MatchingHistoryViewModel(StudentMatchingHistory matchingHistory)
        {
            _matchingHistory = matchingHistory;
        }

        public DateTime DateOfMatch { get => _matchingHistory.DateOfMatch; }

        public string DateOfUnMatch
        {
            get => _matchingHistory.DateOfUnMatch == new DateTime() ? "" : 
                _matchingHistory.DateOfUnMatch.ToString("d");
        }

        public string TracksHistory
        {
            get => string.Join("\n", from t in _matchingHistory.TracksHistory
                                     let date = t.Item1.ToString("d")
                                     let track = Dictionaries.PrefferdTracksDict[t.Item2]
                                     select date + ": " + track);

        }

        public string MatchStudentName { get => _matchingHistory.MatchStudentName; }

        public int MatchStudentId { get => _matchingHistory.MatchStudentId; }

        public bool IsUnMatch { get => _matchingHistory.IsUnMatch; }

        public bool IsActive { get => _matchingHistory.IsActive; }
    }
}
