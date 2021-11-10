using System;
using System.Collections.Generic;
using DO;
using UtilEntities;

namespace LogicLayer
{
    /// <summary>
    /// Interface for deciphering the values ​​on the columns for a student's properties
    /// </summary>
    internal interface IStudentDescriptor
    {
        /// <summary>
        /// The spreadsheet id
        /// </summary>
        string SpreadsheetId { get; }

        /// <summary>
        /// The spreadsheet range of rows and colunms 
        /// </summary>
        string Range { get; }

        /// <summary>
        /// The spreadsheet name
        /// </summary>
        string SheetName { get; }

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
