﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace GoogleSheetDataSource
{
    public interface IStudentDescriptor
    {
        string SpreadsheetId { get; }
        string Range { get; }
        string SheetName { get; }

        Genders GetGender(object row);
        LearningStyles GetLearningStyle(object row);
        SkillLevels GetSkillLevel(object row);
        EnglishLevels GetEnglishLevel(object row);
        Genders GetPrefferdGender(object row);
        List<PrefferdTracks> GetPrefferdTracks(object row);
        TimeSpan GetStudentOffset(object v);
        Days GetDay(int i);
        IEnumerable<TimesInDay> GetTimesInDey(object row);
        string GetCountryName(object row);
    }
}
