using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace DS
{
    interface IStudentDescriptor
    {
        Genders GetGender(object row);
        LearningStyles GetLearningStyle(object row);
        SkillLevels GetSkillLevel(object row);
        EnglishLevels GetEnglishLevel(object row);
        Genders GetPrefferdGender(object row);
        PrefferdTracks GetPrefferdTracks(object row);
    }
}
