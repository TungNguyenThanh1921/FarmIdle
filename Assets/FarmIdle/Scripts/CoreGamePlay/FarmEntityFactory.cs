using System;
using CoreGamePlay;
using Data;

public static class FarmEntityFactory
{
    public static FarmEntity Create(string entityId, DateTime now)
    {
        if (!FarmEntityConfigLoader.All.TryGetValue(entityId, out var data))
        {
            UnityEngine.Debug.LogError($"❌ Không tìm thấy cấu hình cho: {entityId}");
            return null;
        }

        return data.Type switch
        {
            "Crop" => new Crop(data.Id, data.YieldInterval, data.MaxYield, now),
            "Animal" => new Animal(data.Id, data.YieldInterval, data.MaxYield, now),
            _ => throw new NotImplementedException($"⚠️ Chưa hỗ trợ type: {data.Type}")
        };
    }
}