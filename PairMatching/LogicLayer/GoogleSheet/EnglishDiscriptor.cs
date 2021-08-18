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

        public string Range => "A2:Z";

        public string SheetName => "Shalhevet Regestration form תשפ\"א(תגובות)";

        public DateTime LastUpdate { get; set; }

        public EnglishDiscriptor(LastDataOfSpredsheet lastDateOfUpdate)
        {
            LastUpdate = lastDateOfUpdate.EnglishSheets;
        }

        public EnglishLevels GetEnglishLevel(string value)
        {
            switch (value)
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

        public Genders GetGender(string value)
        {
            switch (value)
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

        public LearningStyles GetLearningStyle(string value)
        {
            switch (value)
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

        public Genders GetPrefferdGender(string value)
        {
            switch (value)
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

        private PrefferdTracks SwitchPrefferdTracks(string value)
        {
            switch (value.Replace(",", "").Trim())
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

        public List<PrefferdTracks> GetPrefferdTracks(string value)
        {
            var tracksInString = value.Split(',');
            var result = new List<PrefferdTracks>();
            foreach (var s in tracksInString)
            {
                result.Add(SwitchPrefferdTracks(s));
            }
            return result;
        }

        public SkillLevels GetSkillLevel(string value)
        {
            switch (value)
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

        public TimeSpan GetStudentOffset(string value)
        {
            string timeFormat = Regex.Replace(value, "[^0-9.:-]", "");
            return TimeSpan.Parse(timeFormat);
        }

        public IEnumerable<TimesInDay> GetTimesInDey(string value)
        {
            var timesInString = value
                .Replace("Late", "")
                .Split(',');
            var result = new List<TimesInDay>();

            foreach(var s in timesInString)
            {
                switch (s.Replace(",", "").Trim())
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
                    case "This day is not available for me":
                        result.Add(TimesInDay.INCAPABLE);
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

        public string GetCountryName(string value)
        {
            var rgx = new  Regex("[^a-zA-Z ]");
            return rgx.Replace(value, "").Trim();
        }

        public IEnumerable<string> GetLanguages(string value)
        {
            var result = value
                .Split(',')
                .ToList();
            result.ForEach(l => l.Replace(",", "").Trim());
            return result;
        }

        public MoreLanguages GetMoreLanguages(string value)
        {
            switch (value)
            {
                case "Yes":
                    return MoreLanguages.YES;
                case "No":
                    return MoreLanguages.NO;
                default:
                    return MoreLanguages.NOT_ENGLISH;
            }
        }

        public int GetPrefferdNumberOfMatchs(string value)
        {
            switch (value)
            {
                case "One is enough":
                    return 1;
                case "I would like to have 2":
                    return 2;
                case "I would like to have 3":
                    return 3;
                default:
                    break;
            }
            return 1;
        }
    }
}

