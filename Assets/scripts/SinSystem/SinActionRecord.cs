using System;
using UnityEngine;

namespace DantesInferno.SinSystem
{
    [Serializable]
    public sealed class SinActionRecord
    {
        [SerializeField] private SinEventId eventId;
        [SerializeField] private SinActionKind kind;
        [SerializeField] private double timestamp;
        [SerializeField] private string encounterId;
        [SerializeField] private SinBehaviorPattern behaviorPattern;
        [SerializeField] private bool successful;
        [SerializeField] private float danger;
        [SerializeField] private float availableAlternatives;
        [SerializeField] private float rewardValue;

        public SinEventId EventId => eventId;
        public SinActionKind Kind => kind;
        public double Timestamp => timestamp;
        public string EncounterId => encounterId;
        public SinBehaviorPattern BehaviorPattern => behaviorPattern;
        public bool Successful => successful;
        public float Danger => danger;
        public float AvailableAlternatives => availableAlternatives;
        public float RewardValue => rewardValue;

        public SinActionRecord(
            SinEventId eventId,
            SinActionKind kind,
            double timestamp,
            string encounterId = "",
            SinBehaviorPattern behaviorPattern = SinBehaviorPattern.Balanced,
            bool successful = true,
            float danger = 0f,
            float availableAlternatives = 0f,
            float rewardValue = 0f)
        {
            if (!eventId.IsValid)
                throw new ArgumentException("A valid event identifier is required.", nameof(eventId));

            this.eventId = eventId;
            this.kind = kind;
            this.timestamp = timestamp;
            this.encounterId = encounterId ?? string.Empty;
            this.behaviorPattern = behaviorPattern;
            this.successful = successful;
            this.danger = Clamp01(danger);
            this.availableAlternatives = Math.Max(0f, availableAlternatives);
            this.rewardValue = Math.Max(0f, rewardValue);
        }

        private static float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }
}
