using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class FileManager
{
    private static string GetPath(string filename) => Path.Combine(Application.persistentDataPath, filename);

    // Lưu dữ liệu xuống file JSON
    public static void Save<T>(T data, string filename)
    {
        string path = GetPath(filename + ".cttt");
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(path, json);
        Debug.Log($"Saved data to: {path}");
    }

    // Đọc dữ liệu từ file JSON
    public static T Load<T>(string filename)
    {
        string path = GetPath(filename + ".cttt");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }
        Debug.LogWarning("File not found: " + path);
        return default;
    }

    // Kiểm tra file có tồn tại không
    public static bool Exists(string filename)
    {
        return File.Exists(GetPath(filename+".cttt"));
    }

    // Xóa file dữ liệu
    public static void Delete(string filename)
    {
        string path = GetPath(filename+".cttt");
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Deleted file: {path}");
        }
    }
}
