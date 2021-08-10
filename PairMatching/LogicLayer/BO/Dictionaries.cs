using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public static class Dictionaries
    {
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
        public static Dictionary<string, StringBuilder> TemplateEmails =
            new Dictionary<string, StringBuilder>
            {
                {
                    "match for student from word",
                    new StringBuilder()
                    .AppendLine("<h2>Hi <b>@Model.FirstStudent.Name</b></h2>")
                    .AppendLine("<h3>We found Chevruta for you!</h3>")
                    .AppendLine("<h4>Details of the Chevruta:</h4>")
                    .AppendLine("<h4>Yours study track is <i>@Model.PrefferdTracksShow</i></h4>")
                    .AppendLine("<h4>Name: <i>@Model.SecondStudent.Name</i><br>" +
                        "Email: <i>@Model.SecondStudent.Email</i>.<br>" +
                        "Phone number: <i>@Model.SecondStudent.PhoneNumber.</h4>")
                
                },
                {
                    "match for student from israel",
                    new StringBuilder()
                    .AppendLine("<h2><b>@Model.FirstStudent.Name</b>שלום!</h2>")
                    .AppendLine("<h3>We found Chevruta for you!</h3>")
                    .AppendLine("<h4>Details of the Chevruta:</h4>")
                    .AppendLine("<h4>Yours study track is <i>@Model.PrefferdTracksShow</i></h4>")
                    .AppendLine("<h4>Name: <i>@Model.SecondStudent.Name</i><br>" +
                        "Email: <i>@Model.SecondStudent.Email</i>.<br>" +
                        "Phone number: <i>@Model.SecondStudent.PhoneNumber.</h4>")

                },
                {
                    "pair to the secretary",
                    new StringBuilder()
                    .AppendLine("<h2>@Model</h2>")
                    .AppendLine("<p>שעות </p>")
                }
            };
    }
}
