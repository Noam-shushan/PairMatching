using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public static class TheOpenQuestions
    {
        private static readonly List<string> theOpenQuestions = new List<string>
        {
            "Personal information",
            "Personality trates",
            "Anything else you would like to tell us",
            "Who introduced you to this program",
            "What are your hopes and expectations from this program",
            "Additional information",
            "Country & City of residence"
        };

        public static string GetQuestion(string question)
        {
            return theOpenQuestions.Find(q => q == question);
        }
    }
}
