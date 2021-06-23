using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalJson
{
    public class JsonTools 
    {
        static string dir = @"json\";
        static string studentsPath = @"studentListJson.json";
        static string pairsPath = @"pairListJson.json";
        static string countersPath = @"counters.json";
        static string learningTimePath = @"learningTime.json";
        static string lastDateOfSheetsPath = @"lastDateOfSheets.json";

        static JsonTools()
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(dir + studentsPath))
            {
                File.Create(dir + studentsPath);
            }

            if (!File.Exists(dir + pairsPath))
            {
                File.Create(dir + pairsPath);
            }

            if (!File.Exists(dir + countersPath))
            {
                File.Create(dir + countersPath);
            }

            if (!File.Exists(dir + learningTimePath))
            {
                File.Create(dir + learningTimePath);
            }

            if (!File.Exists(dir + lastDateOfSheetsPath))
            {
                File.Create(dir + lastDateOfSheetsPath);
            }
        }

        public static void SaveListToJsonFile<T>(List<T> list, string filePath)
        {
            using (StreamWriter file = File.CreateText(dir + filePath))
            {
                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                serializer.Serialize(file, list);
            }
        }

        public static List<T> LoadListFromJsonFile<T>(string filePath)
        {
            if (!File.Exists(dir + filePath))
            {
                File.Create(dir + filePath);
            }

            var jsonString = File.ReadAllText(dir + filePath);
            if (jsonString == string.Empty)
            {
                return new List<T>();
            }
            return JsonConvert.DeserializeObject<List<T>>(jsonString);
        }

        public static T LoadObjFromJsonFile<T>(string filePath)
        {
            if (!File.Exists(dir + filePath))
            {
                File.Create(dir + filePath);
            }

            var jsonString = File.ReadAllText(dir + filePath);
            return JsonConvert.DeserializeObject<T>(jsonString);
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

