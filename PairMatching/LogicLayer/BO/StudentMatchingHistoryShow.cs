using System;
using System.Collections.Generic;
using System.Linq;
using DO;

namespace BO
{
    public class StudentMatchingHistoryShow
    {
        public DateTime DateOfMatch { get; set; }
        public string DateOfMatchShow { get => DateOfMatch != new DateTime() ?
                DateOfMatch.ToString("d") : ""; }

        public DateTime DateOfUnMatch { get; set; }
        public string DateOfUnMatchShow { get => DateOfUnMatch != new DateTime() ?
                DateOfUnMatch.ToString("d") : ""; }

        public List<Tuple<DateTime, PrefferdTracks>> TracksHistory { get; set; } =
            new List<Tuple<DateTime, PrefferdTracks>>();

        public string TracksHistoryShow
        {
            get => string.Join("\n", from t in TracksHistory
                                         let date = t.Item1.ToString("d")
                                         let track = Dictionaries.PrefferdTracksDict[t.Item2]
                                         select date + ": " + track);

        }

        public string MatchStudentName { get; set; }
        public int MatchStudentId { get; set; }
        public bool IsUnMatch { get; set; }
        public bool IsActive { get; set; }
    }
}

