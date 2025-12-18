using System.Collections.Generic;
using UnityEngine;

public class ComboQueue : MonoBehaviour
{
    public int maxSize = 8;
    public float resetTime = 0.8f;

    private readonly List<char> inputs = new List<char>();
    private float lastInputTime;

    public void Register(char input)
    {
        // time gap -> reset history
        if (Time.time - lastInputTime > resetTime)
            inputs.Clear();

        lastInputTime = Time.time;

        inputs.Add(input);
        if (inputs.Count > maxSize)
            inputs.RemoveAt(0);
    }

    public void Clear() => inputs.Clear();

    public string DebugString() => new string(inputs.ToArray());

    // AAA / BBB / CCC
    public bool IsSameSpam(int count = 3)
    {
        if (inputs.Count < count) return false;

        int lastIndex = inputs.Count - 1;
        char last = inputs[lastIndex];

        for (int i = 1; i < count; i++)
        {
            if (inputs[lastIndex - i] != last)
                return false;
        }

        return true;
    }


    // ABABAB (or BCB CBC, etc.)
    public bool IsAlternatingSpam(int pairs = 3)
    {
        int needed = pairs * 2;
        if (inputs.Count < needed) return false;

        // Take the last N inputs
        int startIndex = inputs.Count - needed;

        char first = inputs[startIndex];
        char second = inputs[startIndex + 1];

        if (first == second)
            return false;

        for (int i = 0; i < needed; i++)
        {
            char expected = (i % 2 == 0) ? first : second;
            if (inputs[startIndex + i] != expected)
                return false;
        }

        return true;
    }


}
