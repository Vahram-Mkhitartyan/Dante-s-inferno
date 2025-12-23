using System;
using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance { get; private set; }

    [Header("Seed")]
    public int seed = 12345;
    public bool randomizeOnPlay = false;

    public System.Random Rng { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        if (randomizeOnPlay)
            seed = Environment.TickCount;

        Rng = new System.Random(seed);
    }

    // Deterministic helpers
    public int RangeInt(int minInclusive, int maxExclusive)
        => Rng.Next(minInclusive, maxExclusive);

    public float RangeFloat(float minInclusive, float maxInclusive)
        => (float)(minInclusive + (maxInclusive - minInclusive) * Rng.NextDouble());

    public void Shuffle<T>(System.Collections.Generic.IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = RangeInt(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
