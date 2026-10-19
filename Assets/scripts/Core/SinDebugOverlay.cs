using DantesInferno.SinSystem;
using UnityEngine;

[DefaultExecutionOrder(-900)]
public sealed class SinDebugOverlay : MonoBehaviour
{
    private const KeyCode ToggleKey = KeyCode.F8;

    private SinManager boundManager;
    private SinStateSnapshot snapshot;
    private SinActionRecord lastAction;
    private GUIStyle panelStyle;
    private GUIStyle headingStyle;
    private GUIStyle labelStyle;
    private bool visible;

    void Awake()
    {
        PersistentServiceLifecycle.PersistIndependently(this);
    }

    void OnEnable()
    {
        PlayerActionEvents.OnActionRecorded += HandleActionRecorded;
        BindManager();
    }

    void OnDisable()
    {
        PlayerActionEvents.OnActionRecorded -= HandleActionRecorded;
        UnbindManager();
    }

    void Update()
    {
        if (!Application.isEditor && !Debug.isDebugBuild)
            return;

        if (Input.GetKeyDown(ToggleKey))
            visible = !visible;

        if (boundManager != SinManager.Instance)
            BindManager();
    }

    void OnGUI()
    {
        if (!visible || snapshot == null || (!Application.isEditor && !Debug.isDebugBuild))
            return;

        EnsureStyles();

        const float width = 310f;
        float height = 116f + snapshot.AxisCount * 22f;
        GUILayout.BeginArea(new Rect(16f, 16f, width, height), panelStyle);
        GUILayout.Label("SIN DIAGNOSTICS  [F8]", headingStyle);
        GUILayout.Label($"Dominant: {snapshot.CurrentDominantSin}   Overall: {snapshot.OverallState} {snapshot.OverallPressure:P0}", labelStyle);

        SinAxisSnapshot[] axes = snapshot.CopyAxes();
        for (int index = 0; index < axes.Length; index++)
        {
            SinAxisSnapshot axis = axes[index];
            GUILayout.Label($"{axis.Sin,-9} pressure {axis.Pressure,5:P0}   mastery {axis.Mastery,5:P0}", labelStyle);
        }

        string actionText = lastAction == null
            ? "Last action: none"
            : $"Last action: {lastAction.Kind} ({lastAction.BehaviorPattern})";
        GUILayout.Label(actionText, labelStyle);
        GUILayout.EndArea();
    }

    private void BindManager()
    {
        UnbindManager();
        boundManager = SinManager.Instance;
        if (boundManager == null)
            return;

        boundManager.OnSnapshotChanged += HandleSnapshotChanged;
        snapshot = boundManager.CurrentSnapshot;
    }

    private void UnbindManager()
    {
        if (boundManager != null)
            boundManager.OnSnapshotChanged -= HandleSnapshotChanged;

        boundManager = null;
    }

    private void HandleSnapshotChanged(SinStateSnapshot nextSnapshot)
    {
        snapshot = nextSnapshot;
    }

    private void HandleActionRecorded(SinActionRecord action)
    {
        lastAction = action;
    }

    private void EnsureStyles()
    {
        if (panelStyle != null)
            return;

        panelStyle = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(12, 12, 10, 10),
            alignment = TextAnchor.UpperLeft
        };
        headingStyle = new GUIStyle(GUI.skin.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            normal = { textColor = new Color(1f, 0.55f, 0.45f) }
        };
        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            normal = { textColor = Color.white }
        };
    }
}
