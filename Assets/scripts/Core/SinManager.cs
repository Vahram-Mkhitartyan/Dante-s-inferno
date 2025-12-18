using System;
using UnityEngine;

public class SinManager : MonoBehaviour
{
    public static SinManager Instance { get; private set; }

    public int Sin { get; private set; } = 0;

    public event Action<int> OnSinChanged; // passes new Sin value

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DontDestroyOnLoad(transform.root.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);
    }

    public void AddSin(int amount)
    {
        if (amount <= 0) return;

        Sin += amount;
        Debug.Log($"[Sin] +{amount} -> Total Sin = {Sin}");

        OnSinChanged?.Invoke(Sin);
    }
}