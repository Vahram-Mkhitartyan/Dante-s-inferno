using System;
using UnityEngine;

namespace DantesInferno.SinSystem
{
    [Serializable]
    public sealed class SinAxisSnapshot
    {
        [SerializeField] private SinType sin;
        [SerializeField] private float influenceWeight;
        [SerializeField] private float composition;
        [SerializeField] private float pressure;
        [SerializeField] private float mastery;

        public SinType Sin => sin;
        public float InfluenceWeight => influenceWeight;
        public float Composition => composition;
        public float Pressure => pressure;
        public float Mastery => mastery;

        public SinAxisSnapshot(
            SinType sin,
            float influenceWeight,
            float composition,
            float pressure,
            float mastery)
        {
            this.sin = sin;
            this.influenceWeight = influenceWeight;
            this.composition = composition;
            this.pressure = pressure;
            this.mastery = mastery;
        }
    }
}
