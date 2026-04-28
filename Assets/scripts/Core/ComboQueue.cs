using System.Collections.Generic;
using UnityEngine;

public class ComboQueue : MonoBehaviour
{
    public int maxSize = 8;
    public float resetTime = 0.8f;

    private readonly Queue<char> inputs = new Queue<char>();
    private float lastInputTime;

    void OnEnable()
    {
        inputs.Clear();
        lastInputTime = 0f;
    }

    public void Register(char input)
    {
        // time gap -> reset history
        if (Time.time - lastInputTime > resetTime)
        {
            inputs.Clear();
        }

        lastInputTime = Time.time;

        inputs.Enqueue(input);
        while (inputs.Count > maxSize)
            inputs.Dequeue();
    }

    public void Clear()
    {
        inputs.Clear();
    }

    public string DebugString() => new string(inputs.ToArray());

    public string DebugStringLast(int count)
    {
        if (count <= 0 || inputs.Count == 0) return string.Empty;
        char[] arr = inputs.ToArray();
        int take = Mathf.Min(count, arr.Length);
        return new string(arr, arr.Length - take, take);
    }

    // AAA / BBB / CCC
    public bool IsSameSpam(int count = 3)
    {
        if (inputs.Count < count) return false;

        char[] arr = inputs.ToArray();
        int lastIndex = arr.Length - 1;
        char last = arr[lastIndex];

        for (int i = 1; i < count; i++)
        {
            if (arr[lastIndex - i] != last)
                return false;
        }

        return true;
    }


    // ABABAB (or BCB CBC, etc.)
    public bool IsAlternatingSpam(int pairs = 3)
    {
        int needed = pairs * 2;
        if (inputs.Count < needed) return false;

        char[] arr = inputs.ToArray();
        int startIndex = arr.Length - needed;

        char first = arr[startIndex];
        char second = arr[startIndex + 1];

        if (first == second)
            return false;

        for (int i = 0; i < needed; i++)
        {
            char expected = (i % 2 == 0) ? first : second;
            if (arr[startIndex + i] != expected)
                return false;
        }

        return true;
    }


}
