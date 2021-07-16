using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LogicLayer
{
    public class EnglishDiscriptor : IStudentDescriptor
    {
        public string SpreadsheetId => "1s8JObwXhv9kCdAEX6e_m4SV6N0_RRRgmoeyoG1oR82c";

        public string Range => "A2:W";

        public string SheetName => "Shalhevet Regestration form תשפ\"א(תגובות)";

        public EnglishLevels GetEnglishLevel(object row)
        {
            switch (row.ToString())
            {
                case "Excellent (I don't know any hebrew whatsoever)":
                    return EnglishLevels.GOOD;
                case "Doesn't have to be perfect. I know some Hebrew":
                    return EnglishLevels.NOT_GOOD;
                case "Conversational level":
                    return EnglishLevels.TALK_LEVEL;
            }
            return EnglishLevels.DONT_MATTER;
        }

        public Genders GetGender(object row)
        {
            switch (row.ToString())
            {
                case "Male":
                    return Genders.MALE;
                case "Female":
                    return Genders.FMALE;
                case "Prefer not to say":
                    return Genders.DONT_MATTER;
            }
            return default;
        }

        public LearningStyles GetLearningStyle(object row)
        {
            switch (row.ToString())
            {
                case "Deep and slow":
                    return LearningStyles.DEEP_AND_SLOW;
                case "progressed, flowing":
                    return LearningStyles.PROGRESSED_FLOWING;
                case "Text centered":
                    return LearningStyles.TEXTUALL_CENTERED;
                case "Philosofical, free talking, deriving from text into thought":
                    return LearningStyles.FREE;
            }
            return LearningStyles.DONT_MATTER;
        }

        public Genders GetPrefferdGender(object row)
        {
            switch (row.ToString())
            {
                case "Only with men":
                    return Genders.MALE;
                case "Only with women":
                    return Genders.FMALE;
                case "No prefrence":
                    return Genders.DONT_MATTER;
            }
            return Genders.DONT_MATTER;
        }

        private PrefferdTracks SwitchPrefferdTracks(string row)
        {
            switch (row.ToString())
            {
                case "Tanya":
                    return PrefferdTracks.TANYA;
                case "Talmud":
                    return PrefferdTracks.TALMUD;
                case "Parsha":
                    return PrefferdTracks.PARASHA;
                case "Prayer":
                    return PrefferdTracks.PRAYER;
                case "Pirkey Avot (Ethics of the fathers)":
                    return PrefferdTracks.PIRKEY_AVOT;
                case "No preference":
                    return PrefferdTracks.DONT_MATTER;
            }
            return PrefferdTracks.DONT_MATTER;
        }

        public List<PrefferdTracks> GetPrefferdTracks(object row)
        {
            var tracksInString = row.ToString()
                .Replace(",", "")
                .Split(' ');
            var result = new List<PrefferdTracks>();
            foreach (var s in tracksInString)
            {
                result.Add(SwitchPrefferdTracks(s));
            }
            return result;
        }

        public SkillLevels GetSkillLevel(object row)
        {
            switch (row.ToString())
            {
                case "Advanced":
                    return SkillLevels.ADVANCED;
                case "Moderate":
                    return SkillLevels.MODERATE;
                case "Begginer":
                    return SkillLevels.BEGGINER;
            }
            return SkillLevels.DONT_MATTER;
        }

        public TimeSpan GetStudentOffset(object row)
        {
            string timeFormat = Regex.Replace(row.ToString(), "[^0-9.:-]", "");
            return TimeSpan.Parse(timeFormat);
        }

        public IEnumerable<TimesInDay> GetTimesInDey(object row)
        {
            var timesInString = row.ToString()
                .Replace(",", "")
                .Replace("Late", "")
                .Split(' ');
            var result = new List<TimesInDay>();

            foreach(var s in timesInString)
            {
                switch (s)
                {
                    case "morning":
                        result.Add(TimesInDay.MORNING);
                        break;
                    case "Noon":
                        result.Add(TimesInDay.NOON);
                        break;
                    case "Evening":
                        result.Add(TimesInDay.EVENING);
                        break;
                    case "night":
                        result.Add(TimesInDay.NIGHT);
                        break;
                }
            }
            return result;
        }

        public Days GetDay(int i)
        {
            switch (i)
            {
                case 2:
                    return Days.SUNDAY;
                case 3:
                    return Days.MONDAY;
                case 4:
                    return Days.TUESDAY;
                case 5:
                    return Days.WEDNESDAY;
                case 6:
                    return Days.THURSDAY;

            }
            return Days.DONT_MATTER;
        }

        public string GetCountryName(object row)
        {
            var rgx = new  Regex("[^a-zA-Z ]");
            return rgx.Replace(row.ToString(), "").Trim();
        }
    }
}

