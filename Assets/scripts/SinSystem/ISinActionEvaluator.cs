namespace DantesInferno.SinSystem
{
    public interface ISinActionEvaluator
    {
        bool TryEvaluate(
            SinActionRecord action,
            SinStateSnapshot currentState,
            out SinEvaluationDelta delta);
    }
}
