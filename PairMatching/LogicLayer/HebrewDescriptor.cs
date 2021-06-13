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
        public string Range { get => "A2:P"; }

        public string SpreadsheetId { get => "1iNKE8QeDxPqCkOvnmi4Qa7tiDCDjOQ6uDZ6Z_eL4b8Q"; }

        public string SheetName { get => "shalhevet in hebrew"; }

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
            return default;
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
            return default;
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
            return default;
        }

        public Genders GetPrefferdGender(object row)
        {
            switch (row.ToString())
            {
                case "רק עם גבר":
                    return Genders.MALE;
                case "רק עם אישה":
                    return Genders.FMALE;
                case "אין לי העדפה":
                    return Genders.DONT_MATTER;
            }
            return default;
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
            return default;
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
            return default;
        }
    }
}
