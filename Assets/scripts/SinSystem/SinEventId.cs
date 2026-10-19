using System;
using UnityEngine;

namespace DantesInferno.SinSystem
{
    [Serializable]
    public struct SinEventId : IEquatable<SinEventId>
    {
        [SerializeField] private string runId;
        [SerializeField] private long sequence;

        public string RunId => runId;
        public long Sequence => sequence;
        public bool IsValid => !string.IsNullOrWhiteSpace(runId) && sequence > 0;

        public SinEventId(string runId, long sequence)
        {
            if (string.IsNullOrWhiteSpace(runId))
                throw new ArgumentException("A run identifier is required.", nameof(runId));
            if (sequence <= 0)
                throw new ArgumentOutOfRangeException(nameof(sequence));

            this.runId = runId;
            this.sequence = sequence;
        }

        public bool Equals(SinEventId other)
        {
            return sequence == other.sequence && string.Equals(runId, other.runId, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is SinEventId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((runId != null ? runId.GetHashCode() : 0) * 397) ^ sequence.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{runId}:{sequence}";
        }
    }
}
