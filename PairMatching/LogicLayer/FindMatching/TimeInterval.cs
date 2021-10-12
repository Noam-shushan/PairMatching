using System;

namespace LogicLayer.FindMatching
{
    internal class TimeInterval
    {
        private static TimeSpan DELTA = TimeSpan.Parse("01:00");
        private static TimeSpan MIN_TIME_TO_LEARN = TimeSpan.Parse("01:30");

        internal TimeInterval(TimeSpan start, TimeSpan end)
        {
            Start = Raound(start);
            End = end < start ? Raound(end + TimeSpan.FromHours(24)) : Raound(end);
        }

        internal TimeSpan Start { get; set; }
        internal TimeSpan End { get; set; }

        private TimeSpan Raound(TimeSpan t)
        {
            if (t < TimeSpan.Zero)
                return TimeSpan.FromHours(24 + t.Hours);
            return t;
        }

        internal bool IsIn(TimeInterval other)
        {
            var tempStart = other.Start.Days >= 1 ? other.Start - TimeSpan.FromDays(1) : other.Start;
            var tempEnd = other.End.Days >= 1 ? other.End - TimeSpan.FromDays(1) : other.End;
            return (tempStart + MIN_TIME_TO_LEARN <= End && tempEnd >= Start) 
                || (tempEnd - MIN_TIME_TO_LEARN >= Start && tempStart <= End);
        }

        public static bool operator ==(TimeInterval left, TimeInterval right)
        {
            return (left.Start == right.Start && left.End == right.End) ||
                (left.Start + DELTA == right.Start && left.End + DELTA == right.End) ||
                (left.Start - DELTA == right.Start && left.End - DELTA == right.End) ||
                (left.Start - DELTA == right.Start && left.End + DELTA == right.End) ||
                (left.Start + DELTA == right.Start && left.End - DELTA == right.End) ||

                (left.Start == right.Start + DELTA && left.End == right.End + DELTA) ||
                (left.Start == right.Start - DELTA && left.End == right.End - DELTA) ||
                (left.Start == right.Start + DELTA && left.End == right.End - DELTA) ||
                (left.Start == right.Start - DELTA && left.End == right.End + DELTA) ||

                (left.Start + DELTA == right.Start && left.End == right.End + DELTA) ||
                (left.Start + DELTA == right.Start && left.End == right.End - DELTA) ||
                (left.Start - DELTA == right.Start && left.End == right.End + DELTA) ||
                (left.Start - DELTA == right.Start && left.End == right.End - DELTA) ||

                (left.Start == right.Start + DELTA && left.End + DELTA == right.End) ||
                (left.Start == right.Start + DELTA && left.End - DELTA == right.End) ||
                (left.Start == right.Start - DELTA && left.End - DELTA == right.End) ||
                (left.Start == right.Start - DELTA && left.End + DELTA == right.End);
        }

        public static bool operator !=(TimeInterval left, TimeInterval right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return this == obj as TimeInterval;
        }

        public override int GetHashCode()
        {
            return (Start, End).GetHashCode();
        }
    }
}
