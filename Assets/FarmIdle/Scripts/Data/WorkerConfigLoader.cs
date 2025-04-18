using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WorkerConfigLoader
{
    public static Dictionary<string, WorkerConfigData> All = new();

    public static void Load()
    {
        All = new Dictionary<string, WorkerConfigData>();
        LoadFromResources();
    }
    public static WorkerConfigData GetDefault()
    {
        return All.Values.FirstOrDefault();
    }
    public static void LoadFromResources()
    {
        TextAsset csv = Resources.Load<TextAsset>("Configs/WorkerConfig");
        if (csv == null)
        {
            Debug.LogError("❌ Không tìm thấy WorkerConfig.csv trong Resources/Configs/");
            return;
        }

        All.Clear();
        var lines = csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            var row = lines[i].Trim();
            if (string.IsNullOrEmpty(row)) continue;

            var cells = row.Split(',');
            if (cells.Length < 5) continue;

            var config = new WorkerConfigData
            {
                Id = cells[0].Trim(),
                DisplayName = cells[1].Trim(),
                Price = int.Parse(cells[2].Trim()),
                ActionTimeSeconds = int.Parse(cells[3].Trim()),
                AutoHarvest = bool.Parse(cells[4].Trim()),
                Role = cells.Length > 5 ? cells[5].Trim() : "General"
            };

            All[config.Id] = config;
        }

        Debug.Log($"✅ Loaded WorkerConfig: {All.Count} loại công nhân.");
    }
}