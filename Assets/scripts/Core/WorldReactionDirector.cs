using UnityEngine;

public class WorldReactionDirector : MonoBehaviour
{
    [Header("References")]
    public HostilityDirector hostility;
    public PlayerBehaviorTracker behavior;

    [Header("Behavior Pressure")]
    public float repeatedPatternPressure = 4f;
    public float overcommitPressure = 3f;
    public float collapsePressure = 8f;

    void Awake()
    {
        if (hostility == null)
            hostility = FindAnyObjectByType<HostilityDirector>();

        if (behavior == null)
            behavior = PlayerBehaviorTracker.Instance;
    }

    void OnEnable()
    {
        if (SinManager.Instance != null)
            SinManager.Instance.OnSinProfileChanged += OnSinProfileChanged;

        if (behavior != null)
            behavior.OnPatternChanged += OnPatternChanged;
    }

    void OnDisable()
    {
        if (SinManager.Instance != null)
            SinManager.Instance.OnSinProfileChanged -= OnSinProfileChanged;

        if (behavior != null)
            behavior.OnPatternChanged -= OnPatternChanged;
    }

    void Start()
    {
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

    void ApplyCurrentReaction(bool allowDehostile)
    {
        if (hostility == null || SinManager.Instance == null)
            return;

        float pressure = SinManager.Instance.Sin;
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
}
