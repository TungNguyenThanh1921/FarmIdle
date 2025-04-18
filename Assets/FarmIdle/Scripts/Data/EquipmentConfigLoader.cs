using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EquipmentConfigLoader
{
    public static Dictionary<string, EquipmentConfigData> All = new();

    public static void Load()
    {
        All = new Dictionary<string, EquipmentConfigData>();
        LoadFromResources();
    }
    public static EquipmentConfigData GetDefault()
    {
        return All.Values.FirstOrDefault();
    }
    public static void LoadFromResources()
    {
        TextAsset csv = Resources.Load<TextAsset>("Configs/EquipmentConfig");
        if (csv == null)
        {
            Debug.LogError("❌ Không tìm thấy EquipmentConfig.csv trong Resources/Configs/");
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

            var config = new EquipmentConfigData
            {
                Id = cells[0].Trim(),
                DisplayName = cells[1].Trim(),
                Price = int.Parse(cells[2]),
                BoostPercent = int.Parse(cells[3]),
                Level = int.Parse(cells[4])
            };

            All[config.Id] = config;
        }

        Debug.Log($"✅ Loaded EquipmentConfig: {All.Count} loại thiết bị.");
    }
}