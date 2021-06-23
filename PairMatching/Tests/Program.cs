using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using System.Text.RegularExpressions;

namespace Tests
{
    class Counters
    {
        static int _studentCounter = 0;

        public static Counters Instance { get; } = new Counters();

        Counters() { }

        public int StudentCounter
        {
            get
            {
                return _studentCounter;
            }
            set => _studentCounter = value;
        }

        public void IncStudentCounter()
        {
            ++_studentCounter;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            /*            if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        string countersPath = @"counters.josn";
                        var count = LoadObjFromJsonFile<Counters>(countersPath);
                        //Counters.Instance.IncStudentCounter();
                        count.IncStudentCounter();
                        var c = count.StudentCounter; 
                        Console.WriteLine(c);
                        SaveObjToJsonFile(count, countersPath);
                        Console.ReadKey();*/
            
            
            var diff = TimeZoneInfo.Local.BaseUtcOffset - TimeSpan.Parse("-05:00");
            Console.WriteLine(diff);
            var list = new List<TimeSpan>
            {
                TimeSpan.Parse("12:30"),
                TimeSpan.Parse("18:30"),
                TimeSpan.Parse("22:00"),
                TimeSpan.Parse("1:00")
            };
            var lll = GetTimesInDey("morning, Noon, Evening, Late night");
            Console.WriteLine(GetStudentOffset("Hongary -01:00"));
            //Console.WriteLine(TimeSpan.Parse("+01:00"));
            //(var t in list)
            // {
            //Console.WriteLine(getTimesInDay(TimeSpan.Parse("18:30") - diff));
            //}
            Console.ReadKey();
        }

        public static IEnumerable<string> GetTimesInDey(string row)
        {
            var timesInString = row
                .Replace(",", "")
                .Replace("Late", "")
                .Split(' ');
            var result = new List<string>();

            foreach (var s in timesInString)
            {
                switch (s)
                {
                    case "morning":
                        result.Add("morning");
                        break;
                    case "Noon":
                        result.Add("Noon");
                        break;
                    case "Evening":
                        result.Add("Evening");
                        break;
                    case "night":
                        result.Add("night");
                        break;
                }
            }
            return result;
        }

        public static TimeSpan GetStudentOffset(string row)
        {
            string timeFormat = Regex.Replace(row, "[^0-9.:-]", "");
            return TimeSpan.Parse(timeFormat);
        }

        private static string getTimesInDay(TimeSpan t)
        {
            if (t >= TimeSpan.Parse("7:00") && t <= TimeSpan.Parse("12:30"))
                return "MORNING";
            
            if (t >= TimeSpan.Parse("12:30") && t <= TimeSpan.Parse("18:30"))
                return "NOON";

            if (t >= TimeSpan.Parse("18:30") && t <= TimeSpan.Parse("22:00"))
                return "EVENING";

            if (t >= TimeSpan.Parse("22:00") && t <= TimeSpan.Parse("1:00"))
                return "NIGHT";


            return "bad";
        }









        static string dir = @"json\";

        public static T LoadObjFromJsonFile<T>(string filePath)
        {
            if (!File.Exists(dir + filePath))
            {
                throw new Exception("the file" + filePath + "\nis not exist");
            }

            var jsonString = File.ReadAllText(dir + filePath);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static void SaveListToJsonFile<T>(List<T> list, string filePath)
        {
            using (StreamWriter file = File.CreateText(dir + filePath))
            {
                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                serializer.Serialize(file, list);
            }
        }

        public static void SaveObjToJsonFile<T>(T obj, string filePath)
        {
            using (StreamWriter file = File.CreateText(dir + filePath))
            {
                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                serializer.Serialize(file, obj);
            }
        }
    }
}
