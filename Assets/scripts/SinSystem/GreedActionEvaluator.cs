using System;

namespace DantesInferno.SinSystem
{
    public sealed class GreedActionEvaluator : ISinActionEvaluator
    {
        private readonly GreedEvaluationRules rules;

        public GreedActionEvaluator(GreedEvaluationRules rules = null)
        {
            this.rules = rules ?? GreedEvaluationRules.Default;
        }

        public bool TryEvaluate(
            SinActionRecord action,
            SinStateSnapshot currentState,
            out SinEvaluationDelta delta)
        {
            switch (action.Kind)
            {
                case SinActionKind.RewardCollected:
                    float value = Math.Max(1f, action.RewardValue);
                    delta = new SinEvaluationDelta(
                        SinType.Greed,
                        value * rules.CollectionInfluenceScale,
                        value * rules.CollectionPressureScale
                            + action.AvailableAlternatives * rules.AlternativePressureScale,
                        0f,
                        "greed.reward-collected");
                    return true;
                case SinActionKind.RewardRejected:
                    delta = new SinEvaluationDelta(
                        SinType.Greed,
                        0f,
                        -rules.RejectionPressureRecovery,
                        rules.RejectionMasteryEvidence,
                        "greed.reward-rejected");
                    return true;
                case SinActionKind.RewardInvested:
                    delta = new SinEvaluationDelta(
                        SinType.Greed,
                        action.RewardValue * rules.CollectionInfluenceScale * 0.25f,
                        -rules.InvestmentPressureRecovery,
                        rules.InvestmentMasteryEvidence,
                        "greed.reward-invested");
                    return true;
                default:
                    delta = default;
                    return false;
            }
        }
    }
}
