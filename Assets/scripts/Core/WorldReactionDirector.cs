using UnityEngine;
using DantesInferno.SinSystem;

public class WorldReactionDirector : MonoBehaviour
{
    [Header("References")]
    public HostilityDirector hostility;
    public PlayerBehaviorTracker behavior;

    [Header("Behavior Pressure")]
    public float repeatedPatternPressure = 4f;
    public float overcommitPressure = 3f;
    public float collapsePressure = 8f;

    private SinManager boundSinManager;
    private PlayerBehaviorTracker boundBehavior;

    void Awake()
    {
        if (hostility == null)
            hostility = FindAnyObjectByType<HostilityDirector>();

        if (behavior == null)
            behavior = PlayerBehaviorTracker.Instance;
    }

    void OnEnable()
    {
        BindDependencies();
    }

    void OnDisable()
    {
        UnbindDependencies();
    }

    void Start()
    {
        BindDependencies();
        ApplyCurrentReaction(allowDehostile: true);
    }

    void OnSinProfileChanged(SinManager sin)
    {
        ApplyCurrentReaction(allowDehostile: false);
    }

    void OnPatternChanged(PlayerBehaviorPattern pattern)
    {
        ApplyCurrentReaction(allowDehostile: false);
    }

    void OnSnapshotChanged(SinStateSnapshot snapshot)
    {
        ApplyCurrentReaction(allowDehostile: false);
    }

    void ApplyCurrentReaction(bool allowDehostile)
    {
        if (hostility == null || SinManager.Instance == null)
            return;

        float pressure = SinManager.Instance.CurrentSnapshot != null
            ? SinManager.Instance.CurrentSnapshot.OverallPressure * 100f
            : SinManager.Instance.Sin;
        pressure += GetBehaviorPressure();

        if (SinManager.Instance.State == SinState.Collapse)
            pressure += collapsePressure;

        hostility.ApplyReaction(pressure, allowDehostile);
    }

    float GetBehaviorPressure()
    {
        PlayerBehaviorTracker tracker = behavior != null ? behavior : PlayerBehaviorTracker.Instance;
        if (tracker == null)
            return 0f;

        switch (tracker.CurrentPattern)
        {
            case PlayerBehaviorPattern.RepeatedAttack:
            case PlayerBehaviorPattern.AlternatingAttack:
                return repeatedPatternPressure;
            case PlayerBehaviorPattern.Overcommitted:
                return overcommitPressure;
            default:
                return 0f;
        }
    }

    void BindDependencies()
    {
        SinManager currentSinManager = SinManager.Instance;
        if (boundSinManager != currentSinManager)
        {
            if (boundSinManager != null)
            {
                boundSinManager.OnSinProfileChanged -= OnSinProfileChanged;
                boundSinManager.OnSnapshotChanged -= OnSnapshotChanged;
            }

            boundSinManager = currentSinManager;
            if (boundSinManager != null)
            {
                boundSinManager.OnSinProfileChanged += OnSinProfileChanged;
                boundSinManager.OnSnapshotChanged += OnSnapshotChanged;
            }
        }

        PlayerBehaviorTracker currentBehavior = behavior != null
            ? behavior
            : PlayerBehaviorTracker.Instance;
        if (boundBehavior == currentBehavior)
            return;

        if (boundBehavior != null)
            boundBehavior.OnPatternChanged -= OnPatternChanged;

        boundBehavior = currentBehavior;
        if (boundBehavior != null)
            boundBehavior.OnPatternChanged += OnPatternChanged;
    }

    void UnbindDependencies()
    {
        if (boundSinManager != null)
        {
            boundSinManager.OnSinProfileChanged -= OnSinProfileChanged;
            boundSinManager.OnSnapshotChanged -= OnSnapshotChanged;
            boundSinManager = null;
        }

        if (boundBehavior != null)
        {
            boundBehavior.OnPatternChanged -= OnPatternChanged;
            boundBehavior = null;
        }
    }
}
