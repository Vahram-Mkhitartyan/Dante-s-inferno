using System;
using UnityEngine;

namespace DantesInferno.SinSystem
{
    [Serializable]
    public sealed class SinStateSnapshot
    {
        public const int CurrentSchemaVersion = 1;

        [SerializeField] private int schemaVersion;
        [SerializeField] private SinType originSin;
        [SerializeField] private SinType currentDominantSin;
        [SerializeField] private float overallPressure;
        [SerializeField] private SinPressureState overallState;
        [SerializeField] private SinAxisSnapshot[] axes;

        public int SchemaVersion => schemaVersion;
        public SinType OriginSin => originSin;
        public SinType CurrentDominantSin => currentDominantSin;
        public float OverallPressure => overallPressure;
        public SinPressureState OverallState => overallState;
        public int AxisCount => axes?.Length ?? 0;

        public SinStateSnapshot(
            SinType originSin,
            SinType currentDominantSin,
            float overallPressure,
            SinPressureState overallState,
            SinAxisSnapshot[] axes)
        {
            schemaVersion = CurrentSchemaVersion;
            this.originSin = originSin;
            this.currentDominantSin = currentDominantSin;
            this.overallPressure = overallPressure;
            this.overallState = overallState;
            this.axes = axes ?? throw new ArgumentNullException(nameof(axes));
        }

        public SinAxisSnapshot GetAxis(SinType sin)
        {
            int index = (int)sin;
            if (index < 0 || index >= AxisCount)
                throw new ArgumentOutOfRangeException(nameof(sin));

            return axes[index];
        }

        public SinAxisSnapshot[] CopyAxes()
        {
            return (SinAxisSnapshot[])axes.Clone();
        }
    }
}
