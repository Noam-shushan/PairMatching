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

        static JsonTools()
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public static void InsertRecords<T>(List<T> list, string filePath)
        {
            try
            {
                using (StreamWriter file = File.CreateText(dir + filePath))
                {
                    JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                    serializer.Serialize(file, list);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public static List<T> LoadRecords<T>(string filePath)
        {
            try
            {
                if (File.Exists(dir + filePath))
                {
                    var jsonString = File.ReadAllText(dir + filePath);
                    return JsonConvert.DeserializeObject<List<T>>(jsonString);
                }
                else
                {
                    return new List<T>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"can not load the file {dir + filePath}" + ex.Message);
            }
        }

        public static T LoadOne<T>(string filePath)
        {
            try
            {
                if (File.Exists(dir + filePath))
                {
                    var jsonString = File.ReadAllText(dir + filePath);
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
                else
                {
                    return default;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"can not load the file {dir + filePath}" + ex.Message);
            } 
        }

        public static void InsertOne<T>(T obj, string filePath)
        {
            try
            {
                using (StreamWriter file = File.CreateText(dir + filePath))
                {
                    JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                    serializer.Serialize(file, obj);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}

