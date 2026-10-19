using System;
using System.Collections.Generic;
using DantesInferno.SinSystem;
using UnityEngine;

public enum DominantSin
{
    Pride,
    Greed,
    Lust,
    Envy,
    Gluttony,
    Wrath,
    Sloth
}

public enum SinState
{
    Controlled,
    Rising,
    Unstable,
    Collapse
}

[DefaultExecutionOrder(-1000)]
public class SinManager : MonoBehaviour
{
    public static SinManager Instance { get; private set; }

    public static void ResetRuntimeState()
    {
        Instance = null;
    }

    [Header("Run Identity")]
    public DominantSin dominantSin = DominantSin.Wrath;
    public SinDefinition definition;

    [Header("Momentum")]
    [SerializeField] private float momentum;
    public float maxMomentum = 100f;
    public float passiveMomentumDecay = 0.75f;

    [Header("Stability")]
    [SerializeField] private float stability = 75f;
    public float maxStability = 100f;
    public float deliberatePauseDelay = 1.1f;
    public float deliberatePauseRecovery = 6f;

    [Header("Power")]
    public float maxPowerBonus = 0.75f;

    [Header("Collapse")]
    public float collapseMomentumLoss = 25f;
    public float collapseStabilityRecovery = 35f;
    public float collapseCooldown = 2f;

    [Header("State Thresholds")]
    [Range(0f, 1f)] public float risingThreshold = 0.35f;
    [Range(0f, 1f)] public float unstableThreshold = 0.70f;
    [Range(0f, 1f)] public float pureFormMomentumThreshold = 0.75f;
    [Range(0f, 1f)] public float pureFormStabilityThreshold = 0.70f;

    [Header("Long-Term Run State")]
    [Range(0f, 1f)] [SerializeField] private float deathCorruption;
    [SerializeField] private DeathCompanionChoice companionChoice;

    public int Sin { get; private set; } = 1;
    public float Momentum => momentum;
    public float Stability => stability;
    public SinState State { get; private set; } = SinState.Controlled;
    public int CollapseCount { get; private set; }
    public bool IsPureForm { get; private set; }
    public float PowerMultiplier => 1f + GetMomentumRatio() * GetMaxPowerBonus();
    public float DeathCorruption => deathCorruption;
    public DeathCompanionChoice CompanionChoice => companionChoice;
    public SinStateSnapshot CurrentSnapshot { get; private set; }

    public event Action<int> OnSinChanged; // passes new Sin value
    public event Action<SinManager> OnSinProfileChanged;
    public event Action<SinStateSnapshot> OnSnapshotChanged;

    private float lastSinActionTime;
    private float lastCollapseTime = -999f;
    private SinStateModel stateModel;
    private readonly List<ISinActionEvaluator> evaluators = new List<ISinActionEvaluator>();
    private readonly HashSet<SinEventId> processedEventIds = new HashSet<SinEventId>();
    private readonly HashSet<string> resolvedTemptationIds = new HashSet<string>(StringComparer.Ordinal);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        PersistentServiceLifecycle.PersistIndependently(this);
        InitializeStateModel(LegacySinCompatibilityAdapter.ToSinType(dominantSin));
        RecalculateState();
    }

    void OnEnable()
    {
        PlayerActionEvents.OnAttackStarted += HandleAttackStarted;
        PlayerActionEvents.OnAttackHit += HandleAttackHit;
        PlayerActionEvents.OnDamageBlocked += HandleDamageBlocked;
        PlayerActionEvents.OnDamageTaken += HandleDamageTaken;
        PlayerActionEvents.OnPlayerDied += HandlePlayerDied;
        PlayerActionEvents.OnNonHostileEnemyKilled += HandleNonHostileEnemyKilled;
        PlayerActionEvents.OnActionRecorded += HandleActionRecorded;
        PlayerActionEvents.OnModifyOutgoingDamage += ScaleDamage;
    }

    void OnDisable()
    {
        PlayerActionEvents.OnAttackStarted -= HandleAttackStarted;
        PlayerActionEvents.OnAttackHit -= HandleAttackHit;
        PlayerActionEvents.OnDamageBlocked -= HandleDamageBlocked;
        PlayerActionEvents.OnDamageTaken -= HandleDamageTaken;
        PlayerActionEvents.OnPlayerDied -= HandlePlayerDied;
        PlayerActionEvents.OnNonHostileEnemyKilled -= HandleNonHostileEnemyKilled;
        PlayerActionEvents.OnActionRecorded -= HandleActionRecorded;
        PlayerActionEvents.OnModifyOutgoingDamage -= ScaleDamage;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        if (momentum > 0f)
            SetMomentum(momentum - passiveMomentumDecay * Time.deltaTime);

        if (Time.time - lastSinActionTime >= deliberatePauseDelay)
            SetStability(stability + deliberatePauseRecovery * Time.deltaTime);

        RecalculateState();

        if (stateModel != null)
        {
            SinType previousDominant = stateModel.CurrentDominantSin;
            stateModel.AdvanceDominance(Time.deltaTime);
            if (stateModel.CurrentDominantSin != previousDominant)
            {
                dominantSin = LegacySinCompatibilityAdapter.ToDominantSin(stateModel.CurrentDominantSin);
                PublishSnapshot();
            }
        }
    }

    void HandleAttackStarted(AttackType attack)
    {
        PlayerBehaviorPattern pattern = PlayerBehaviorTracker.Instance != null
            ? PlayerBehaviorTracker.Instance.CurrentPattern
            : PlayerBehaviorPattern.Balanced;

        HandleAttackStarted(attack, pattern);
    }

    void HandleAttackStarted(AttackType attack, PlayerBehaviorPattern pattern)
    {
        float gain = GetAttackMomentum(attack);
        float stabilityCost = GetPatternStabilityCost(pattern);

        AddMomentum(gain);
        SetStability(stability - stabilityCost);
        lastSinActionTime = Time.time;
    }

    void HandleAttackHit(AttackType attack)
    {
        AddMomentum(GetAttackMomentum(attack) * GetHitMomentumMultiplier());
        lastSinActionTime = Time.time;
    }

    void HandleDamageBlocked()
    {
        SetStability(stability + GetBlockedDamageRecovery());
    }

    void HandleDamageTaken()
    {
        SetStability(stability - GetDamageTakenCost());
        lastSinActionTime = Time.time;
    }

    void HandlePlayerDied()
    {
        Collapse();
    }

    void HandleNonHostileEnemyKilled()
    {
        AddMomentum(GetNonHostileKillMomentum());
    }

    void HandleActionRecorded(SinActionRecord action)
    {
        if (action == null || !processedEventIds.Add(action.EventId))
            return;

        if (IsGreedChoice(action.Kind)
            && !string.IsNullOrWhiteSpace(action.EncounterId)
            && !resolvedTemptationIds.Add(action.EncounterId))
            return;

        SinStateSnapshot snapshot = stateModel.CreateSnapshot();
        for (int index = 0; index < evaluators.Count; index++)
        {
            if (evaluators[index].TryEvaluate(action, snapshot, out SinEvaluationDelta delta))
                stateModel.Apply(delta);
        }

        PublishSnapshot();
    }

    public int ScaleDamage(int baseDamage)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseDamage * PowerMultiplier));
    }

    void AddMomentum(float amount)
    {
        if (amount <= 0f) return;
        SetMomentum(momentum + amount);
        RecalculateState();
    }

    void SetMomentum(float value)
    {
        float before = momentum;
        momentum = Mathf.Clamp(value, 0f, maxMomentum);

        int previousSin = Sin;
        Sin = Mathf.Max(1, Mathf.RoundToInt(momentum));

        if (Sin != previousSin)
            OnSinChanged?.Invoke(Sin);

        if (!Mathf.Approximately(before, momentum))
            OnSinProfileChanged?.Invoke(this);
    }

    void SetStability(float value)
    {
        float before = stability;
        stability = Mathf.Clamp(value, 0f, maxStability);

        if (!Mathf.Approximately(before, stability))
            OnSinProfileChanged?.Invoke(this);
    }

    void RecalculateState()
    {
        float momentumRatio = GetMomentumRatio();
        float stabilityRatio = maxStability <= 0f ? 0f : stability / maxStability;

        SinState nextState;
        if (momentum > stability)
            nextState = SinState.Collapse;
        else if (momentumRatio >= unstableThreshold || stabilityRatio <= 0.3f)
            nextState = SinState.Unstable;
        else if (momentumRatio >= risingThreshold)
            nextState = SinState.Rising;
        else
            nextState = SinState.Controlled;

        IsPureForm = momentumRatio >= pureFormMomentumThreshold && stabilityRatio >= pureFormStabilityThreshold;

        if (nextState == SinState.Collapse)
            Collapse();
        else if (State != nextState)
        {
            State = nextState;
            OnSinProfileChanged?.Invoke(this);
        }

        SynchronizeCompatibilityPressure();
    }

    void Collapse()
    {
        if (Time.time < lastCollapseTime + collapseCooldown)
            return;

        lastCollapseTime = Time.time;
        CollapseCount++;
        State = SinState.Collapse;
        SetMomentum(momentum - GetCollapseMomentumLoss());
        SetStability(GetCollapseStabilityRecovery());
        OnSinProfileChanged?.Invoke(this);
    }

    float GetAttackMomentum(AttackType attack)
    {
        if (definition != null)
            return definition.GetAttackMomentum(attack);

        switch (attack)
        {
            case AttackType.TopDownSwing:
                return dominantSin == DominantSin.Wrath ? 4f : 2.5f;
            case AttackType.UpwardLauncher:
                return dominantSin == DominantSin.Pride ? 4f : 3f;
            case AttackType.ForwardPierce:
                return dominantSin == DominantSin.Greed ? 5f : 4f;
            default:
                return 2f;
        }
    }

    float GetPatternStabilityCost(PlayerBehaviorPattern pattern)
    {
        if (definition != null)
            return definition.GetPatternStabilityCost(pattern);

        switch (pattern)
        {
            case PlayerBehaviorPattern.RepeatedAttack:
            case PlayerBehaviorPattern.AlternatingAttack:
                return 8f;
            case PlayerBehaviorPattern.Overcommitted:
                return 6f;
            default:
                return 2f;
        }
    }

    float GetHitMomentumMultiplier()
    {
        return definition != null ? definition.hitMomentumMultiplier : 0.5f;
    }

    float GetBlockedDamageRecovery()
    {
        return definition != null ? definition.blockedDamageRecovery : 4f;
    }

    float GetDamageTakenCost()
    {
        return definition != null ? definition.damageTakenCost : 12f;
    }

    float GetNonHostileKillMomentum()
    {
        return definition != null ? definition.nonHostileKillMomentum : 1f;
    }

    float GetCollapseMomentumLoss()
    {
        return definition != null ? definition.collapseMomentumLoss : collapseMomentumLoss;
    }

    float GetCollapseStabilityRecovery()
    {
        return definition != null ? definition.collapseStabilityRecovery : collapseStabilityRecovery;
    }

    float GetMaxPowerBonus()
    {
        return definition != null ? definition.maxPowerBonus : maxPowerBonus;
    }

    float GetMomentumRatio()
    {
        return maxMomentum <= 0f ? 0f : Mathf.Clamp01(momentum / maxMomentum);
    }

    public SinRunSaveData CaptureSaveData()
    {
        EnsureStateModel();

        int currentSeed = SeedManager.Instance != null ? SeedManager.Instance.seed : 0;
        long drawCount = SeedManager.Instance != null ? SeedManager.Instance.DrawCount : 0;
        var processed = new SinEventId[processedEventIds.Count];
        processedEventIds.CopyTo(processed);
        Array.Sort(processed, CompareEventIds);
        var resolvedTemptations = new string[resolvedTemptationIds.Count];
        resolvedTemptationIds.CopyTo(resolvedTemptations);
        Array.Sort(resolvedTemptations, StringComparer.Ordinal);

        return new SinRunSaveData(
            PlayerActionEvents.RunId,
            PlayerActionEvents.NextSequence,
            stateModel.OriginSin,
            stateModel.CurrentDominantSin,
            stateModel.CreateSaveAxes(),
            CollapseCount,
            deathCorruption,
            companionChoice,
            currentSeed,
            drawCount,
            processed,
            resolvedTemptations);
    }

    public void RestoreSaveData(SinRunSaveData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        data.Validate();
        InitializeStateModel(data.OriginSin);
        stateModel.Restore(data.CurrentDominantSin, data.Axes);

        processedEventIds.Clear();
        for (int index = 0; index < data.ProcessedEventIds.Length; index++)
        {
            SinEventId eventId = data.ProcessedEventIds[index];
            if (eventId.IsValid)
                processedEventIds.Add(eventId);
        }

        resolvedTemptationIds.Clear();
        for (int index = 0; index < data.ResolvedTemptationIds.Length; index++)
        {
            string encounterId = data.ResolvedTemptationIds[index];
            if (!string.IsNullOrWhiteSpace(encounterId))
                resolvedTemptationIds.Add(encounterId);
        }

        CollapseCount = data.CollapseCount;
        deathCorruption = data.DeathCorruption;
        companionChoice = data.CompanionChoice;
        dominantSin = LegacySinCompatibilityAdapter.ToDominantSin(data.CurrentDominantSin);
        PlayerActionEvents.ConfigureRun(data.RunId, data.NextEventSequence);

        if (SeedManager.Instance != null)
            SeedManager.Instance.Restore(data.Seed, data.RandomDrawCount);

        float originPressure = stateModel.GetPressure(stateModel.OriginSin);
        momentum = originPressure * Mathf.Max(0f, maxMomentum);
        stability = (1f - originPressure) * Mathf.Max(0f, maxStability);
        Sin = Mathf.Max(1, Mathf.RoundToInt(momentum));
        RecalculateState();
        PublishSnapshot();
    }

    public void SetDeathCorruption(float value)
    {
        deathCorruption = Mathf.Clamp01(value);
    }

    public void SetCompanionChoice(DeathCompanionChoice choice)
    {
        companionChoice = choice;
    }

    private void InitializeStateModel(SinType originSin)
    {
        stateModel = new SinStateModel(originSin);
        evaluators.Clear();
        evaluators.Add(new WrathActionEvaluator());
        evaluators.Add(new GreedActionEvaluator());
        SynchronizeCompatibilityPressure();
        PublishSnapshot();
    }

    private void EnsureStateModel()
    {
        if (stateModel == null)
            InitializeStateModel(LegacySinCompatibilityAdapter.ToSinType(dominantSin));
    }

    private void SynchronizeCompatibilityPressure()
    {
        if (stateModel == null)
            return;

        SinType legacySin = LegacySinCompatibilityAdapter.ToSinType(dominantSin);
        float compatibilityPressure = LegacySinCompatibilityAdapter.CalculatePressure(
            momentum,
            maxMomentum,
            stability,
            maxStability);

        if (stateModel.GetPressure(legacySin) + 0.0001f < compatibilityPressure)
        {
            stateModel.SetPressure(legacySin, compatibilityPressure);
            PublishSnapshot();
        }
    }

    private void PublishSnapshot()
    {
        if (stateModel == null)
            return;

        CurrentSnapshot = stateModel.CreateSnapshot();
        OnSnapshotChanged?.Invoke(CurrentSnapshot);
    }

    private static bool IsGreedChoice(SinActionKind kind)
    {
        return kind == SinActionKind.RewardCollected
            || kind == SinActionKind.RewardRejected
            || kind == SinActionKind.RewardInvested;
    }

    private static int CompareEventIds(SinEventId left, SinEventId right)
    {
        int runComparison = string.Compare(left.RunId, right.RunId, StringComparison.Ordinal);
        return runComparison != 0 ? runComparison : left.Sequence.CompareTo(right.Sequence);
    }
}
