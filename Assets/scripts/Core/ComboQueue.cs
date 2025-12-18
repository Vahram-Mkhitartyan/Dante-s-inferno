using System.Collections.Generic;
using UnityEngine;

public class ComboQueue : MonoBehaviour
{
    public int maxSize = 6;
    public float resetTime = 0.6f;

    private Queue<char> queue = new Queue<char>();
    private float lastInputTime;

    public void Register(char input)
    {
        if (Time.time - lastInputTime > resetTime)
            queue.Clear();

        if (queue.Count >= maxSize)
            queue.Dequeue();

        queue.Enqueue(input);
        lastInputTime = Time.time;
    }

    public bool Matches(string pattern)
    {
        if (pattern.Length > queue.Count) return false;

        char[] arr = queue.ToArray();
        for (int i = 0; i < pattern.Length; i++)
        {
            if (arr[arr.Length - pattern.Length + i] != pattern[i])
                return false;
        }
        return true;
    }

    public string DebugString()
    {
        return new string(queue.ToArray());
    }

    public void Clear()
    {
        queue.Clear();
    }
}
