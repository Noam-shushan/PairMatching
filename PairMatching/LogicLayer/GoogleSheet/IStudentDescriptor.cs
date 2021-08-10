using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace LogicLayer
{
    public interface IStudentDescriptor
    {
        string SpreadsheetId { get; }
        string Range { get; }
        string SheetName { get; }
        DateTime LastUpdate { get; set; }

        Genders GetGender(string row);
        LearningStyles GetLearningStyle(string row);
        SkillLevels GetSkillLevel(string row);
        EnglishLevels GetEnglishLevel(string row);
        Genders GetPrefferdGender(string row);
        List<PrefferdTracks> GetPrefferdTracks(string row);
        TimeSpan GetStudentOffset(string v);
        Days GetDay(int i);
        IEnumerable<TimesInDay> GetTimesInDey(string row);
        string GetCountryName(string row);
        IEnumerable<string> GetLanguages(string row);
        MoreLanguages GetMoreLanguages(string row);
        int GetPrefferdNumberOfMatchs(string row);
    }
}
