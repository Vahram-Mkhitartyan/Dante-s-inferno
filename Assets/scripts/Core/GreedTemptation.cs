using UnityEngine;

public class GreedTemptation : MonoBehaviour
{
    [Min(0f)] public float rewardValue = 1f;
    [Min(0f)] public float availableAlternatives = 1f;
    [Range(0f, 1f)] public float danger;
    public string encounterId;

    public bool IsResolved { get; private set; }

    public void Collect()
    {
        if (!TryResolve())
            return;

        PlayerActionEvents.RaiseRewardCollected(
            rewardValue,
            GetEncounterId(),
            availableAlternatives,
            danger);
    }

    public void Reject()
    {
        if (!TryResolve())
            return;

        PlayerActionEvents.RaiseRewardRejected(rewardValue, GetEncounterId(), danger);
    }

    public void Invest()
    {
        if (!TryResolve())
            return;

        PlayerActionEvents.RaiseRewardInvested(rewardValue, GetEncounterId(), danger);
    }

    private bool TryResolve()
    {
        if (IsResolved)
            return false;

        IsResolved = true;
        return true;
    }

    private string GetEncounterId()
    {
        return string.IsNullOrWhiteSpace(encounterId) ? gameObject.name : encounterId;
    }
}
