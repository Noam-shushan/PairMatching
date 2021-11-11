using System;
using System.Collections.Generic;

namespace UtilEntities
{
    public class StudentMatchingHistory
    {
        public DateTime DateOfMatch { get; set; }
        public DateTime DateOfUnMatch { get; set; }
        public List<Tuple<DateTime, PrefferdTracks>> TracksHistory { get; set; } =
            new List<Tuple<DateTime, PrefferdTracks>>();
        public string MatchStudentName { get; set; }
        public int MatchStudentId { get; set; }
        public bool IsUnMatch { get; set; }
        public bool IsActive { get; set; }
    }
}

