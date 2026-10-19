using System;
using System.IO;
using DantesInferno.SinSystem;
using UnityEngine;

public static class RunSaveService
{
    private const string SaveFileName = "dantes-inferno-run.json";

    public static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static void SaveCurrentRun()
    {
        if (SinManager.Instance == null)
            throw new InvalidOperationException("A SinManager is required to save the current run.");

        string json = SinSaveJson.Serialize(SinManager.Instance.CaptureSaveData(), true);
        string temporaryPath = SavePath + ".tmp";
        File.WriteAllText(temporaryPath, json);

        if (File.Exists(SavePath))
            File.Delete(SavePath);

        File.Move(temporaryPath, SavePath);
    }

    public static bool TryLoadCurrentRun(out string error)
    {
        error = string.Empty;
        if (!File.Exists(SavePath))
            return false;
        if (SinManager.Instance == null)
        {
            error = "A SinManager is required to load a run.";
            return false;
        }

        try
        {
            SinRunSaveData data = SinSaveJson.Deserialize(File.ReadAllText(SavePath));
            SinManager.Instance.RestoreSaveData(data);
            return true;
        }
        catch (Exception exception)
        {
            error = exception.Message;
            return false;
        }
    }
}
