namespace DantesInferno.SinSystem
{
    public sealed class WrathActionEvaluator : ISinActionEvaluator
    {
        private readonly WrathEvaluationRules rules;

        public WrathActionEvaluator(WrathEvaluationRules rules = null)
        {
            this.rules = rules ?? WrathEvaluationRules.Default;
        }

        public bool TryEvaluate(
            SinActionRecord action,
            SinStateSnapshot currentState,
            out SinEvaluationDelta delta)
        {
            switch (action.Kind)
            {
                case SinActionKind.AttackStarted:
                    return EvaluateAttack(action.BehaviorPattern, out delta);
                case SinActionKind.AttackHit:
                    delta = new SinEvaluationDelta(
                        SinType.Wrath,
                        rules.SuccessfulHitInfluence,
                        rules.SuccessfulHitPressure,
                        0f,
                        "wrath.attack-hit");
                    return true;
                case SinActionKind.DamageBlocked:
                    delta = new SinEvaluationDelta(
                        SinType.Wrath,
                        0f,
                        -rules.BlockPressureRecovery,
                        rules.BlockMasteryEvidence,
                        "wrath.restraint-block");
                    return true;
                case SinActionKind.DamageTaken:
                    delta = new SinEvaluationDelta(
                        SinType.Wrath,
                        0f,
                        rules.DamageTakenPressure,
                        0f,
                        "wrath.damage-taken");
                    return true;
                case SinActionKind.NonHostileEnemyKilled:
                    delta = new SinEvaluationDelta(
                        SinType.Wrath,
                        rules.NonHostileKillInfluence,
                        rules.NonHostileKillPressure,
                        0f,
                        "wrath.unnecessary-kill");
                    return true;
                case SinActionKind.PlayerDied:
                    delta = new SinEvaluationDelta(
                        SinType.Wrath,
                        0f,
                        rules.DeathPressure,
                        0f,
                        "wrath.combat-death");
                    return true;
                default:
                    delta = default;
                    return false;
            }
        }

        private bool EvaluateAttack(SinBehaviorPattern pattern, out SinEvaluationDelta delta)
        {
            float influence = rules.BalancedAttackInfluence;
            float pressure = rules.BalancedAttackPressure;
            string reason = "wrath.deliberate-attack";

            switch (pattern)
            {
                case SinBehaviorPattern.Repeated:
                case SinBehaviorPattern.Alternating:
                    influence = rules.UncontrolledAttackInfluence;
                    pressure = rules.RepeatedAttackPressure;
                    reason = "wrath.repeated-attack";
                    break;
                case SinBehaviorPattern.Overcommitted:
                    influence = rules.UncontrolledAttackInfluence;
                    pressure = rules.OvercommittedAttackPressure;
                    reason = "wrath.overcommitted-attack";
                    break;
            }

            delta = new SinEvaluationDelta(SinType.Wrath, influence, pressure, 0f, reason);
            return true;
        }
    }
}
