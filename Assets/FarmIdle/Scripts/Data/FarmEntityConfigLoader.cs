using System.Collections.Generic;
using UnityEngine;

public static class FarmEntityConfigLoader
{
    public static Dictionary<string, FarmEntityConfigData> All = new();
    public static void Load()
    {
        // Đảm bảo dòng này có
        All = new Dictionary<string, FarmEntityConfigData>();
        LoadFromResources();
    }

    public static void LoadFromResources()
    {
        TextAsset csv = Resources.Load<TextAsset>("Configs/FarmEntityConfig");
        if (csv == null)
        {
            Debug.LogError("❌ Không tìm thấy FarmEntityConfig.csv trong Resources/Configs/");
            return;
        }

        All.Clear();
        var lines = csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            var row = lines[i].Trim();
            if (string.IsNullOrEmpty(row)) continue;

            var cells = row.Split(',');
            if (cells.Length < 7) continue;

            var config = new FarmEntityConfigData
            {
                Id = cells[0].Trim(),
                Type = cells[1].Trim(),
                DisplayName = cells[2],
                YieldInterval = int.Parse(cells[3]),
                MaxYield = int.Parse(cells[4]),
                SeedRequired = int.Parse(cells[5]),
                SellPrice = int.Parse(cells[6]),
                ProductId = cells[7].Trim()
            };

            All[config.Id] = config;
        }

        Debug.Log($"✅ Loaded FarmEntityConfig: {All.Count} loại.");
    }

    public static List<FarmEntityConfigData> GetAllByType(string type)
    {
        List<FarmEntityConfigData> list = new();
        foreach (var config in All.Values)
        {
            if (config.Type.Equals(type, System.StringComparison.OrdinalIgnoreCase))
                list.Add(config);
        }
        return list;
    }
}