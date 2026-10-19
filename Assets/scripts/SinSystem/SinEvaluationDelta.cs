namespace DantesInferno.SinSystem
{
    public readonly struct SinEvaluationDelta
    {
        public SinType Sin { get; }
        public float Influence { get; }
        public float Pressure { get; }
        public float Mastery { get; }
        public string ReasonCode { get; }

        public SinEvaluationDelta(
            SinType sin,
            float influence,
            float pressure,
            float mastery,
            string reasonCode)
        {
            Sin = sin;
            Influence = influence;
            Pressure = pressure;
            Mastery = mastery;
            ReasonCode = reasonCode ?? string.Empty;
        }
    }
}
