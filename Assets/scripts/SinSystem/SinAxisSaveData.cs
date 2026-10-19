using System;
using UnityEngine;

namespace DantesInferno.SinSystem
{
    [Serializable]
    public sealed class SinAxisSaveData
    {
        [SerializeField] private SinType sin;
        [SerializeField] private float influenceWeight;
        [SerializeField] private float pressure;
        [SerializeField] private float mastery;

        public SinType Sin => sin;
        public float InfluenceWeight => influenceWeight;
        public float Pressure => pressure;
        public float Mastery => mastery;

        public SinAxisSaveData(SinType sin, float influenceWeight, float pressure, float mastery)
        {
            this.sin = sin;
            this.influenceWeight = influenceWeight;
            this.pressure = pressure;
            this.mastery = mastery;
        }
    }
}
