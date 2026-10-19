using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameRuntimeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetRuntimeState()
    {
        PlayerActionEvents.ResetRuntimeState();
        SinManager.ResetRuntimeState();
        SeedManager.ResetRuntimeState();
        SceneManager.sceneLoaded -= EnsureRequiredServices;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneBootstrap()
    {
        SceneManager.sceneLoaded -= EnsureRequiredServices;
        SceneManager.sceneLoaded += EnsureRequiredServices;
    }

    private static void EnsureRequiredServices(Scene scene, LoadSceneMode mode)
    {
        if (SeedManager.Instance == null)
            new GameObject(nameof(SeedManager)).AddComponent<SeedManager>();

        if (SinManager.Instance == null)
            new GameObject(nameof(SinManager)).AddComponent<SinManager>();
    }
}
