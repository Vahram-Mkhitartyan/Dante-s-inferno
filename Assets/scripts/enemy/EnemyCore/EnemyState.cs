using UnityEngine;

public class EnemyState : MonoBehaviour
{
    public bool IsHostile { get; private set; } = true;

    public void SetHostile(bool value)
    {
        IsHostile = value;
    }
}
