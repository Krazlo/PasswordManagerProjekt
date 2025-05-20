using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PwM_Library
{
    public static class JsonHelper
    {
        public static T ReadJsonFromFile<T>(string filePath)
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(filePath));
        }

        public static void CreateJsonFile(string filePath, object ob)
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(ob));
        }

        public static void UpdateJsonFile<T>(string filePath, T ToAdd)
        {
            if (!File.Exists(filePath))
            {
                CreateJsonFile(filePath, new[] { ToAdd });
                return;
            }
            var json = ReadJsonFromFile<T[]>(filePath).ToList();
            json.Add(ToAdd);
            CreateJsonFile(filePath, json.ToArray());
        }

        public static void DeleteJsonData<T>(string filePath, Func<IEnumerable<T>, IEnumerable<T>> toRemove)
        {
            if (!File.Exists(filePath)) { return; }
            var json = ReadJsonFromFile<T[]>(filePath).ToList();
            json = json.Except(toRemove(json)).ToList();
            CreateJsonFile(filePath, json.ToArray());
        }
    }
}
