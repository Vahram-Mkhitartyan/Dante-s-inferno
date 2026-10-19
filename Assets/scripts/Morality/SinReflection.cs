using UnityEngine;
using DantesInferno.SinSystem;

[RequireComponent(typeof(GearEquipper))]
public class SinReflection : MonoBehaviour
{
    private GearEquipper gear;
    private SinManager boundManager;
    private int baseFace;
    private int baseArmor;
    private int baseFeet;
    private int baseArm;
    private int baseShoulder;

    void Awake()
    {
        gear = GetComponent<GearEquipper>();
        baseFace = gear.Face;
        baseArmor = gear.Armor;
        baseFeet = gear.Feet;
        baseArm = gear.Arm;
        baseShoulder = gear.Shoulder;
    }

    void OnEnable()
    {
        BindManager();
    }

    void OnDisable()
    {
        UnbindManager();
    }

    void Update()
    {
        if (boundManager != SinManager.Instance)
            BindManager();
    }

    private void BindManager()
    {
        UnbindManager();
        boundManager = SinManager.Instance;
        if (boundManager == null)
        {
            ApplyStage(0);
            return;
        }

        boundManager.OnSnapshotChanged += ApplySnapshot;
        ApplySnapshot(boundManager.CurrentSnapshot);
    }

    private void UnbindManager()
    {
        if (boundManager != null)
            boundManager.OnSnapshotChanged -= ApplySnapshot;

        boundManager = null;
    }

    private void ApplySnapshot(SinStateSnapshot snapshot)
    {
        if (snapshot == null)
        {
            ApplyStage(0);
            return;
        }

        SinAxisSnapshot dominantAxis = snapshot.GetAxis(snapshot.CurrentDominantSin);
        float severity = Mathf.Max(snapshot.OverallPressure, dominantAxis.Pressure);

        int stage = severity >= 0.85f ? 4
            : severity >= 0.6f ? 3
            : severity >= 0.35f ? 2
            : severity >= 0.1f ? 1
            : 0;
        ApplyStage(stage);
    }

    private void ApplyStage(int stage)
    {
        gear.Face = baseFace;
        gear.Armor = baseArmor;
        gear.Feet = baseFeet;
        gear.Arm = baseArm;
        gear.Shoulder = baseShoulder;

        if (stage >= 1)
            gear.Face = 16;

        if (stage >= 2)
            gear.Armor = 10;

        if (stage >= 3)
            gear.Feet = 10;

        if (stage >= 4)
        {
            gear.Face = 10;
            gear.Armor = 10;
            gear.Feet = 10;
            gear.Arm = 10;
            gear.Shoulder = 10;
        }

        gear.ApplySkinChanges();
    }
}
