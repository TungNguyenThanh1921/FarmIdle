using System;
using System.IO;

public class ConsolePathProvider : IPathProvider
{
    public string GetSavePath(string filename)
    {
        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FarmIdleData");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, filename + ".fi");
    }
}