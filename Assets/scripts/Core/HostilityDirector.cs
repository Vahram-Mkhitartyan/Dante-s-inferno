using System.Collections.Generic;
using UnityEngine;

public class HostilityDirector : MonoBehaviour
{
    [Header("Base Hostility")]
    [Range(0f, 1f)]
    public float baseHostileRatio = 0.30f;

    [Header("Sin Effect")]
    [Tooltip("Extra hostile ratio per 1 sin (0.03 = +3% per sin)")]
    public float ratioPerSin = 0.03f;

    public int minHostile = 0;
    public int maxHostile = 999;

    [Header("Backstabbers")]
    [Range(0f, 1f)]
    public float baseBackstabRatio = 0.10f;

    [Tooltip("Extra backstab chance per sin (0.01 = +1% per sin)")]
    public float backstabPerSin = 0.01f;

    private readonly List<EnemyState> cached = new List<EnemyState>();


    void Start()
    {
        if (!SeedManager.Instance)
        {
            Debug.LogError("No SeedManager in scene.");
            return;
        }

        CacheEnemies();

        AssignPotentialBackstabbers();        // 🔒 identity FIRST
        ApplyHostilityFromCurrentSin(true);   // then hostility
        AssignBackstabbers();                 // then activation

        if (SinManager.Instance != null)
            SinManager.Instance.OnSinChanged += OnSinChanged;
    }


    void OnDestroy()
    {
        if (SinManager.Instance != null)
            SinManager.Instance.OnSinChanged -= OnSinChanged;
    }

    void OnSinChanged(int newSin)
    {
        ApplyHostilityFromCurrentSin(allowDehostile: false);
        AssignBackstabbers();
    }

    // -----------------------------
    // CACHE MANAGEMENT
    // -----------------------------
    void CacheEnemies()
    {
        cached.Clear();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies)
        {
            var s = e.GetComponent<EnemyState>();
            if (s) cached.Add(s);
        }

        // Stable deterministic ordering
        cached.Sort((a, b) =>
        {
            if (a == null || b == null) return 0;

            int ax = Mathf.RoundToInt(a.transform.position.x * 1000f);
            int bx = Mathf.RoundToInt(b.transform.position.x * 1000f);
            if (ax != bx) return ax.CompareTo(bx);

            int ay = Mathf.RoundToInt(a.transform.position.y * 1000f);
            int by = Mathf.RoundToInt(b.transform.position.y * 1000f);
            if (ay != by) return ay.CompareTo(by);

            return a.name.CompareTo(b.name);
        });
    }

    void AssignPotentialBackstabbers()
    {
        RemoveDeadEnemies();
        if (cached.Count == 0) return;

        List<EnemyState> all = new List<EnemyState>(cached);

        SeedManager.Instance.Shuffle(all);

        int count = Mathf.RoundToInt(all.Count * baseBackstabRatio);

        for (int i = 0; i < all.Count; i++)
        {
            var b = all[i].GetComponent<Backstabber>();
            if (!b) continue;

            b.SetPotentialTraitor(i < count);
            b.Arm(false);
        }

        Debug.Log($"[Backstab] Potential traitors locked = {count}/{all.Count}");
    }
   



    void RemoveDeadEnemies()
    {
        cached.RemoveAll(e => e == null);
    }

    // -----------------------------
    // HOSTILITY ASSIGNMENT
    // -----------------------------
    void ApplyHostilityFromCurrentSin(bool allowDehostile)
    {
        RemoveDeadEnemies();
        if (cached.Count == 0) return;

        int sin = SinManager.Instance ? SinManager.Instance.Sin : 0;

        float finalRatio = Mathf.Clamp01(baseHostileRatio + sin * ratioPerSin);
        int targetHostile = Mathf.RoundToInt(cached.Count * finalRatio);
        targetHostile = Mathf.Clamp(
            targetHostile,
            minHostile,
            Mathf.Min(maxHostile, cached.Count)
        );

        int currentHostile = 0;
        foreach (var e in cached)
            if (e.IsHostile) currentHostile++;

        if (allowDehostile)
        {
            foreach (var e in cached)
                e.SetHostile(false);

            currentHostile = 0;
        }

        int need = targetHostile - currentHostile;
        if (need <= 0) return;

        List<EnemyState> neutrals = new List<EnemyState>();
        foreach (var e in cached)
            if (!e.IsHostile)
                neutrals.Add(e);

        SeedManager.Instance.Shuffle(neutrals);

        for (int i = 0; i < need && i < neutrals.Count; i++)
            neutrals[i].SetHostile(true);

        Debug.Log(
            $"[Hostility] Sin={sin} Ratio={finalRatio:0.00} " +
            $"Hostile={currentHostile + Mathf.Min(need, neutrals.Count)}/{cached.Count}"
        );
    }

    // -----------------------------
    // BACKSTABBER ASSIGNMENT
    // -----------------------------
    void AssignBackstabbers()
    {
        RemoveDeadEnemies();
        if (cached.Count == 0) return;

        int sin = SinManager.Instance ? SinManager.Instance.Sin : 0;
        float finalRatio = Mathf.Clamp01(baseBackstabRatio + sin * backstabPerSin);

        List<Backstabber> potentials = new List<Backstabber>();
        foreach (var e in cached)
        {
            var b = e.GetComponent<Backstabber>();
            if (b && b.IsPotentialTraitor && !e.IsHostile)
                potentials.Add(b);
        }

        int target = Mathf.RoundToInt(potentials.Count * finalRatio);

        // deactivate all first
        foreach (var b in potentials)
            b.Arm(false);

        for (int i = 0; i < target && i < potentials.Count; i++)
            potentials[i].Arm(true);

        Debug.Log($"[Backstab] Active {target}/{potentials.Count} (Sin={sin})");
    }

}