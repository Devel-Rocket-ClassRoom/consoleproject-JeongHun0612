using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal static class DataManager
    {
        public static string FolderPath = "./GameData";

        public static void SaveData<T>(T data, string fileName)
        {
            string filePath = GetFilePath(fileName);

            try
            {
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                string json = JsonSerializer.Serialize<T>(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static T LoadData<T>(string fileName)
        {
            string filePath = GetFilePath(fileName);

            T? data = default;

            try
            {
                string json = File.ReadAllText(filePath);
                data = JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return data;
        }

        private static string GetFilePath(string fileName)
        {
            return Path.Combine(FolderPath, $"{fileName}.json");
        }
    }
}
