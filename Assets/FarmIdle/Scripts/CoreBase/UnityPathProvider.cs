using UnityEngine;
using System.IO;

public class UnityPathProvider : IPathProvider
{
    public string GetSavePath(string filename)
    {
        return Path.Combine(Application.persistentDataPath, filename + ".fi");
    }
}