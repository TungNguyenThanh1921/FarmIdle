using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LandConfigLoader
{
    public static Dictionary<string, LandConfigData> All = new();

    public static void Load()
    {
        All = new Dictionary<string, LandConfigData>();
        LoadFromResources();
    }
    public static LandConfigData GetDefault()
    {
        return All.Values.FirstOrDefault();
    }
    public static void LoadFromResources()
    {
        TextAsset csv = Resources.Load<TextAsset>("Configs/LandConfig");
        if (csv == null)
        {
            Debug.LogError("Không tìm thấy LandConfig.csv trong Resources/Configs/");
            return;
        }

        All.Clear();
        var lines = csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            var row = lines[i].Trim();
            if (string.IsNullOrEmpty(row)) continue;

            var cells = row.Split(',');
            if (cells.Length < 4) continue;

            var config = new LandConfigData
            {
                Id = cells[0].Trim(),
                DisplayName = cells[1].Trim(),
                Price = int.Parse(cells[2]),
                SlotCount = int.Parse(cells[3]),
                YieldBuffPercent = int.Parse(cells[4])
            };

            All[config.Id] = config;
        }

        Debug.Log($"Loaded LandConfig: {All.Count} loại đất.");
    }
}