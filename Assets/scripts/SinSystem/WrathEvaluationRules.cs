namespace DantesInferno.SinSystem
{
    public sealed class WrathEvaluationRules
    {
        public float BalancedAttackInfluence { get; }
        public float UncontrolledAttackInfluence { get; }
        public float BalancedAttackPressure { get; }
        public float RepeatedAttackPressure { get; }
        public float OvercommittedAttackPressure { get; }
        public float SuccessfulHitInfluence { get; }
        public float SuccessfulHitPressure { get; }
        public float BlockPressureRecovery { get; }
        public float BlockMasteryEvidence { get; }
        public float DamageTakenPressure { get; }
        public float NonHostileKillInfluence { get; }
        public float NonHostileKillPressure { get; }
        public float DeathPressure { get; }

        public static WrathEvaluationRules Default { get; } = new WrathEvaluationRules();

        public WrathEvaluationRules(
            float balancedAttackInfluence = 0.2f,
            float uncontrolledAttackInfluence = 0.8f,
            float balancedAttackPressure = 0.005f,
            float repeatedAttackPressure = 0.05f,
            float overcommittedAttackPressure = 0.035f,
            float successfulHitInfluence = 0.25f,
            float successfulHitPressure = 0.01f,
            float blockPressureRecovery = 0.035f,
            float blockMasteryEvidence = 0.01f,
            float damageTakenPressure = 0.06f,
            float nonHostileKillInfluence = 1.5f,
            float nonHostileKillPressure = 0.15f,
            float deathPressure = 0.2f)
        {
            BalancedAttackInfluence = balancedAttackInfluence;
            UncontrolledAttackInfluence = uncontrolledAttackInfluence;
            BalancedAttackPressure = balancedAttackPressure;
            RepeatedAttackPressure = repeatedAttackPressure;
            OvercommittedAttackPressure = overcommittedAttackPressure;
            SuccessfulHitInfluence = successfulHitInfluence;
            SuccessfulHitPressure = successfulHitPressure;
            BlockPressureRecovery = blockPressureRecovery;
            BlockMasteryEvidence = blockMasteryEvidence;
            DamageTakenPressure = damageTakenPressure;
            NonHostileKillInfluence = nonHostileKillInfluence;
            NonHostileKillPressure = nonHostileKillPressure;
            DeathPressure = deathPressure;
        }
    }
}
