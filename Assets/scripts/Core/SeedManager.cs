using System;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance { get; private set; }

    [Header("Seed")]
    public int seed = 12345;
    public bool randomizeOnPlay = false;

    public System.Random Rng { get; private set; }
    public long DrawCount { get; private set; }

    public static void ResetRuntimeState()
    {
        Instance = null;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        PersistentServiceLifecycle.PersistIndependently(this);

        if (randomizeOnPlay)
            seed = Environment.TickCount;

        Rng = new System.Random(seed);
        DrawCount = 0;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // Deterministic helpers
    public int RangeInt(int minInclusive, int maxExclusive)
    {
        DrawCount++;
        return Rng.Next(minInclusive, maxExclusive);
    }

    public float RangeFloat(float minInclusive, float maxInclusive)
    {
        DrawCount++;
        return (float)(minInclusive + (maxInclusive - minInclusive) * Rng.NextDouble());
    }

    public void Restore(int restoredSeed, long restoredDrawCount)
    {
        seed = restoredSeed;
        Rng = new System.Random(seed);
        DrawCount = 0;

        long targetDrawCount = Math.Max(0, restoredDrawCount);
        while (DrawCount < targetDrawCount)
        {
            Rng.Next();
            DrawCount++;
        }
    }

    public void Shuffle<T>(System.Collections.Generic.IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = RangeInt(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
