using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DalJson
{
    public class JsonTools
    {
        static string dir = @"json\";
        static JsonTools()
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
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
                throw new Exception("the file" + filePath + "\nis not exist");
            }

            var jsonString = File.ReadAllText(dir + filePath);
            return JsonConvert.DeserializeObject<List<T>>(jsonString);
        }

        public static T LoadObjFromJsonFile<T>(string filePath)
        {
            if (!File.Exists(dir + filePath))
            {
                throw new Exception("the file" + filePath + "\nis not exist");
            }

            var jsonString = File.ReadAllText(dir + filePath);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
