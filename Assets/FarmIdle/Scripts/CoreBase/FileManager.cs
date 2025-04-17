using System.IO;
using Newtonsoft.Json;

public static class FileManager
{
    public static IPathProvider PathProvider;

    public static void Init(IPathProvider provider)
    {
        PathProvider = provider;
    }

    public static void Save<T>(T data, string filename)
    {
        string path = PathProvider.GetSavePath(filename);

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };

        string json = JsonConvert.SerializeObject(data, settings);
        File.WriteAllText(path, json);
    }

    public static T Load<T>(string filename)
    {
        string path = PathProvider.GetSavePath(filename);
        if (File.Exists(path))
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        return default;
    }

    public static bool Exists(string filename)
    {
        return File.Exists(PathProvider.GetSavePath(filename));
    }

    public static void Delete(string filename)
    {
        string path = PathProvider.GetSavePath(filename);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}