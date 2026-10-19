namespace DantesInferno.SinSystem
{
    public sealed class GreedEvaluationRules
    {
        public float CollectionInfluenceScale { get; }
        public float CollectionPressureScale { get; }
        public float AlternativePressureScale { get; }
        public float RejectionPressureRecovery { get; }
        public float RejectionMasteryEvidence { get; }
        public float InvestmentPressureRecovery { get; }
        public float InvestmentMasteryEvidence { get; }

        public static GreedEvaluationRules Default { get; } = new GreedEvaluationRules();

        public GreedEvaluationRules(
            float collectionInfluenceScale = 0.25f,
            float collectionPressureScale = 0.015f,
            float alternativePressureScale = 0.02f,
            float rejectionPressureRecovery = 0.05f,
            float rejectionMasteryEvidence = 0.015f,
            float investmentPressureRecovery = 0.035f,
            float investmentMasteryEvidence = 0.01f)
        {
            CollectionInfluenceScale = collectionInfluenceScale;
            CollectionPressureScale = collectionPressureScale;
            AlternativePressureScale = alternativePressureScale;
            RejectionPressureRecovery = rejectionPressureRecovery;
            RejectionMasteryEvidence = rejectionMasteryEvidence;
            InvestmentPressureRecovery = investmentPressureRecovery;
            InvestmentMasteryEvidence = investmentMasteryEvidence;
        }
    }
}
