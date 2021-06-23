using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    class HebrewDescriptor : IStudentDescriptor
    {
        public string Range { get => "A2:Y"; }

        public string SpreadsheetId { get => "17gvL05Ar-0nCAKvLtAj_HUXDCmXs5L61qk_lkBSUIFo"; }

        public string SheetName { get => "טופס רישום שלהבת תשפ\"א (תגובות)"; }

        public EnglishLevels GetEnglishLevel(object row)
        {
            switch (row.ToString())
            {
                case "טובה":
                    return EnglishLevels.GOOD;
                case "לא כל כך טובה":
                    return EnglishLevels.NOT_GOOD;
                case "רמת שיחה":
                    return EnglishLevels.TALK_LEVEL;
            }
            return EnglishLevels.DONT_MATTER;
        }

        public Genders GetGender(object row)
        {
            switch (row.ToString())
            {
                case "גבר":
                    return Genders.MALE;
                case "אישה":
                    return Genders.FMALE;
                case "לא משנה":
                    return Genders.DONT_MATTER;
            }
            return Genders.DONT_MATTER;
        }

        public LearningStyles GetLearningStyle(object row)
        {
            switch (row.ToString())
            {
                case "לימוד איטי ומעמיק":
                    return LearningStyles.DEEP_AND_SLOW;
                case "לימוד מהיר, הספקי ומתקדם":
                    return LearningStyles.PROGRESSED_FLOWING;
                case "לימוד צמוד טקסט":
                    return LearningStyles.TEXTUALL_CENTERED;
                case "לימוד מעודד מחשבה מחוץ לטקסט, פילוסופי":
                    return LearningStyles.FREE;
            }
            return LearningStyles.DONT_MATTER;
        }

        public Genders GetPrefferdGender(object row)
        {
            switch (row.ToString())
            {
                case "אני מעוניין ללמוד רק עם גבר":
                    return Genders.MALE;
                case "אני מעוניינת ללמוד רק עם אישה":
                    return Genders.FMALE;
                case "אין לי העדפה":
                    return Genders.DONT_MATTER;
            }
            return Genders.DONT_MATTER;
        }

        public PrefferdTracks GetPrefferdTracks(object row)
        {
            switch (row.ToString())
            {
                case "תניא":
                    return PrefferdTracks.TANYA;
                case "גמרא":
                    return PrefferdTracks.TALMUD;
                case "פרשת שבוע":
                    return PrefferdTracks.PARASHA;
                case "תפילה":
                    return PrefferdTracks.PRAYER;
                case "פרקי אבות":
                    return PrefferdTracks.PIRKEY_AVOT;
                case "אין לי העדפה":
                    return PrefferdTracks.DONT_MATTER;
            }
            return PrefferdTracks.DONT_MATTER;
        }

        public SkillLevels GetSkillLevel(object row)
        {
            switch (row.ToString())
            {
                case "טובה":
                    return SkillLevels.ADVANCED;
                case "בינונית":
                    return SkillLevels.MODERATE;
                case "מתחיל":
                    return SkillLevels.BEGGINER;
                case "אין לי העדפה":
                    return SkillLevels.DONT_MATTER;
            }
            return SkillLevels.DONT_MATTER;
        }

        public TimeSpan GetStudentOffset(object v)
        {
            return TimeZoneInfo.Local.BaseUtcOffset;
        }

        public IEnumerable<TimesInDay> GetTimesInDey(object row)
        {
            var timesInString = row.ToString()
                .Replace(",", "")
                .Split(' ');
            var result = new List<TimesInDay>();

            foreach (var s in timesInString)
            {
                switch (s)
                {
                    case "בוקר":
                        result.Add(TimesInDay.MORNING);
                        break;
                    case "צהריים":
                        result.Add(TimesInDay.NOON);
                        break;
                    case "ערב":
                        result.Add(TimesInDay.EVENING);
                        break;
                    case "לילה":
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
    }
}
