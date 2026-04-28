using System;
using UnityEngine;

public enum PlayerBehaviorPattern
{
    Balanced,
    RepeatedAttack,
    AlternatingAttack,
    Overcommitted
}

[DefaultExecutionOrder(-50)]
public class PlayerBehaviorTracker : MonoBehaviour
{
    public static PlayerBehaviorTracker Instance { get; private set; }

    [Header("Pattern Detection")]
    public int repeatedAttackThreshold = 3;
    public int alternatingAttackPairs = 3;
    public int overcommitAttackCount = 5;
    public float overcommitWindow = 1.5f;

    public PlayerBehaviorPattern CurrentPattern { get; private set; } = PlayerBehaviorPattern.Balanced;
    public bool IsUnstablePattern => CurrentPattern != PlayerBehaviorPattern.Balanced;

    public event Action<PlayerBehaviorPattern> OnPatternChanged;

    private readonly QueueBuffer<AttackType> attacks = new QueueBuffer<AttackType>(12);
    private float firstAttackInWindowTime;
    private int attacksInWindow;

    void Awake()
    {
        Instance = this;
    }

    void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        if (CurrentPattern != PlayerBehaviorPattern.Balanced && Time.time - firstAttackInWindowTime > overcommitWindow)
            SetPattern(PlayerBehaviorPattern.Balanced);
    }

    public void RecordAttack(AttackType attack)
    {
        attacks.Add(attack);

        if (Time.time - firstAttackInWindowTime > overcommitWindow)
        {
            firstAttackInWindowTime = Time.time;
            attacksInWindow = 0;
        }

        attacksInWindow++;

        if (IsSameAttackSpam())
            SetPattern(PlayerBehaviorPattern.RepeatedAttack);
        else if (IsAlternatingAttackSpam())
            SetPattern(PlayerBehaviorPattern.AlternatingAttack);
        else if (attacksInWindow >= overcommitAttackCount)
            SetPattern(PlayerBehaviorPattern.Overcommitted);
        else
            SetPattern(PlayerBehaviorPattern.Balanced);
    }

    bool IsSameAttackSpam()
    {
        if (attacks.Count < repeatedAttackThreshold)
            return false;

        AttackType last = attacks[attacks.Count - 1];
        for (int i = 2; i <= repeatedAttackThreshold; i++)
        {
            if (attacks[attacks.Count - i] != last)
                return false;
        }

        return true;
    }

    bool IsAlternatingAttackSpam()
    {
        int needed = alternatingAttackPairs * 2;
        if (attacks.Count < needed)
            return false;

        AttackType first = attacks[attacks.Count - needed];
        AttackType second = attacks[attacks.Count - needed + 1];
        if (first == second)
            return false;

        for (int i = 0; i < needed; i++)
        {
            AttackType expected = i % 2 == 0 ? first : second;
            if (attacks[attacks.Count - needed + i] != expected)
                return false;
        }

        return true;
    }

    void SetPattern(PlayerBehaviorPattern pattern)
    {
        if (CurrentPattern == pattern)
            return;

        CurrentPattern = pattern;
        OnPatternChanged?.Invoke(pattern);
    }

    private class QueueBuffer<T>
    {
        private readonly T[] values;
        private int start;

        public int Count { get; private set; }

        public QueueBuffer(int capacity)
        {
            values = new T[Mathf.Max(1, capacity)];
        }

        public T this[int index]
        {
            get
            {
                int wrapped = (start + index) % values.Length;
                return values[wrapped];
            }
        }

        public void Add(T value)
        {
            if (Count < values.Length)
            {
                values[(start + Count) % values.Length] = value;
                Count++;
                return;
            }

            values[start] = value;
            start = (start + 1) % values.Length;
        }
    }
}
