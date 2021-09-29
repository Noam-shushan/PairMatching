using System.Collections.Generic;
using System.Linq;

namespace BO
{
    public static class Dictionaries
    {
        public static Dictionary<string, string> OpenQuestionsHeaderInHebrow = new Dictionary<string, string>
        {
            ["Personal information"] = "פרטים ביוגרפיים (גיל, מה עושה בחיים, רקע לימודי, השתייכות חברתית)",
            ["Personality trates"] = "תכונות אישיות, ערכים חשובים בשבילך, תחביבים ותחומי עניין",
            ["What are your hopes and expectations from this program"] = "מה מעניין אותך בהצטרפות לשלהבת?",
            ["Additional information"] = "דברים נוספים שהיית רוצה שנדע, או דברים שהיית רוצה לבקש מאיתנו?",
            ["Who introduced you to this program"] = "דרך מי (או דרך מה) הגעת לתכנית?"
        };

        public static Dictionary<string, string> OpenQuestionsHeaderInEnglish = new Dictionary<string, string>
        {
            ["Personal information"] = "Personal information (age, job, Jewish & community affiliation...)",
            ["Personality trates"] = "Personality traits, personal values, interests, hobbies",
            ["What are your hopes and expectations from this program"] = "What are your hopes and expectations from this program?",
            ["Additional information"] = "Additional information you would like us to know about you, or requests you have?",
            ["Who introduced you to this program"] = "Who introduced you to this program? Where did you hear about Shalhevet?",
            ["Country and City of residence"] = "Country & City of residence",
            ["Anything else you would like to tell us"] = "Anything else you would like to tell us?"
        };

        internal static Dictionary<DO.LearningStyles, string> LearningStylesDict = new
            Dictionary<DO.LearningStyles, string>()
        {
            {DO.LearningStyles.DEEP_AND_SLOW, "לימוד איטי ומעמיק" },
            {DO.LearningStyles.FREE, "לימוד מעודד מחשבה\n מחוץ לטקסט, פילוסופי" },
            {DO.LearningStyles.PROGRESSED_FLOWING, "לימוד מהיר, הספקי ומתקדם" },
            {DO.LearningStyles.TEXTUALL_CENTERED, "לימוד צמוד טקסט" },
            {DO.LearningStyles.DONT_MATTER, "לא משנה לי" }
        };

        internal static Dictionary<DO.SkillLevels, string> SkillLevelsDict = new
            Dictionary<DO.SkillLevels, string>()
        {
            {DO.SkillLevels.ADVANCED, "מתקדם" },
            {DO.SkillLevels.MODERATE, "בינוני" },
            {DO.SkillLevels.BEGGINER, "מתחיל" },
            {DO.SkillLevels.DONT_MATTER, "לא משנה" },
        };

        internal static Dictionary<DO.EnglishLevels, string> EnglishLevelsDict = new
            Dictionary<DO.EnglishLevels, string>()
        {
            {DO.EnglishLevels.GOOD, "טובה" },
            {DO.EnglishLevels.TALK_LEVEL, "רמת שיחה (בינונית)" },
            {DO.EnglishLevels.NOT_GOOD, "לא כל כך טובה" },
            {DO.EnglishLevels.DONT_MATTER, "לא משנה" }
        };

        internal static Dictionary<DO.PrefferdTracks, string> PrefferdTracksDict =
            new Dictionary<DO.PrefferdTracks, string>()
        {
            {DO.PrefferdTracks.TANYA, "תניא" },
            {DO.PrefferdTracks.TALMUD, "גמרא" },
            {DO.PrefferdTracks.PARASHA, "פרשה" },
            {DO.PrefferdTracks.PRAYER, "תפילה" },
            {DO.PrefferdTracks.PIRKEY_AVOT, "פרקי אבות" },
            {DO.PrefferdTracks.DONT_MATTER, "לא משנה לי" }
        };

        internal static Dictionary<string, DO.PrefferdTracks> PrefferdTracksDictInverse =
            PrefferdTracksDict.ToDictionary((i) => i.Value, (i) => i.Key);

        internal static Dictionary<DO.Days, string> DaysDict = new Dictionary<DO.Days, string>()
        {
            {DO.Days.SUNDAY, "ראשון" },
            {DO.Days.MONDAY, "שני" },
            {DO.Days.TUESDAY, "שלישי" },
            {DO.Days.WEDNESDAY, "רביעי" },
            {DO.Days.THURSDAY, "חמישי" }
        };

        internal static Dictionary<DO.TimesInDay, string> TimesInDayDict =
            new Dictionary<DO.TimesInDay, string>()
        {
            {DO.TimesInDay.MORNING, "בוקר" },
            {DO.TimesInDay.NOON, "צהריים" },
            {DO.TimesInDay.EVENING, "ערב" },
            {DO.TimesInDay.NIGHT, "לילה" },
            {DO.TimesInDay.INCAPABLE, "אין לי זמן ביום זה" }
        };

        internal static Dictionary<DO.Genders, string> GendersDict =
            new Dictionary<DO.Genders, string>()
            {
                {DO.Genders.MALE, "גבר"},
                {DO.Genders.FMALE, "אישה" },
                {DO.Genders.DONT_MATTER, "לא משנה לי" }
            };
    }
}
