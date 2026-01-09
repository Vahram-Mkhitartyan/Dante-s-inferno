using UnityEngine;

[RequireComponent(typeof(GearEquipper))]
public class SinReflection : MonoBehaviour
{
    private GearEquipper gear;
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
        if (SinManager.Instance != null)
            SinManager.Instance.OnSinChanged += ApplySin;

        ApplySin(SinManager.Instance != null ? SinManager.Instance.Sin : 0);
    }

    void OnDisable()
    {
        if (SinManager.Instance != null)
            SinManager.Instance.OnSinChanged -= ApplySin;
    }

    void ApplySin(int sin)
    {
        // Reset to base so lower sin can revert gear.
        gear.Face = baseFace;
        gear.Armor = baseArmor;
        gear.Feet = baseFeet;
        gear.Arm = baseArm;
        gear.Shoulder = baseShoulder;

        if (sin >= 1)
            gear.Face = 16;     // red eyes

        if (sin >= 11)
            gear.Armor = 10;   // red chest

        if (sin >= 16)
            gear.Feet = 10;    // red legs

        if (sin >= 25)
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
