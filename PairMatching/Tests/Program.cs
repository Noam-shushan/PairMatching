using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DS;

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
            
            
            var time = (from t in TimeZoneInfo.GetSystemTimeZones()
                where t.Id == "Germany Standard Time"
                        select t).FirstOrDefault();
            //Console.WriteLine(TimeZoneInfo.FindSystemTimeZoneById("Germany Standard Time"));
            
            Console.WriteLine(TimeSpan.Parse("-05:00") + DateTime.UtcNow.TimeOfDay);
            Console.WriteLine(getTimesInDay(DateTime.UtcNow.TimeOfDay + TimeSpan.Parse("-05:00")));

            var l = DS.DataSource.StudentsList;
            foreach(var s in l)
            {
                Console.WriteLine(s.Name);
            }

            Console.ReadKey();
        }

        private static string getTimesInDay(TimeSpan studentOffset)
        {
            if (studentOffset >= TimeSpan.Parse("7:00") && studentOffset <= TimeSpan.Parse("12:30"))
                return "MORNING";

            if (studentOffset >= TimeSpan.Parse("12:30") && studentOffset <= TimeSpan.Parse("18:30"))
                return "NOON";

            if (studentOffset >= TimeSpan.Parse("18:30") && studentOffset <= TimeSpan.Parse("22:00"))
                return "EVENING";

            if (studentOffset >= TimeSpan.Parse("22:00") && studentOffset <= TimeSpan.Parse("1:00"))
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
