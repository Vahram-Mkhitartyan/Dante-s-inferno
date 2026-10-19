using System;
using UnityEngine;

namespace DantesInferno.SinSystem
{
    public static class SinSaveJson
    {
        public static string Serialize(SinRunSaveData data, bool prettyPrint = false)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            data.Validate();
            return JsonUtility.ToJson(data, prettyPrint);
        }

        public static SinRunSaveData Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("Save JSON is required.", nameof(json));

            SinRunSaveData data = JsonUtility.FromJson<SinRunSaveData>(json);
            if (data == null)
                throw new InvalidOperationException("Save JSON could not be read.");

            data.Validate();
            return data;
        }
    }
}
