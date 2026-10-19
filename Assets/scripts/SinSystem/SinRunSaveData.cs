using System;
using UnityEngine;

namespace DantesInferno.SinSystem
{
    [Serializable]
    public sealed class SinRunSaveData
    {
        public const int CurrentSchemaVersion = 1;
        public const string CurrentRulesVersion = "sin-rules-1";

        [SerializeField] private int schemaVersion;
        [SerializeField] private string rulesVersion;
        [SerializeField] private string runId;
        [SerializeField] private long nextEventSequence;
        [SerializeField] private SinType originSin;
        [SerializeField] private SinType currentDominantSin;
        [SerializeField] private SinAxisSaveData[] axes;
        [SerializeField] private int collapseCount;
        [SerializeField] private float deathCorruption;
        [SerializeField] private DeathCompanionChoice companionChoice;
        [SerializeField] private int seed;
        [SerializeField] private long randomDrawCount;
        [SerializeField] private SinEventId[] processedEventIds;
        [SerializeField] private string[] resolvedTemptationIds;

        public int SchemaVersion => schemaVersion;
        public string RulesVersion => rulesVersion;
        public string RunId => runId;
        public long NextEventSequence => nextEventSequence;
        public SinType OriginSin => originSin;
        public SinType CurrentDominantSin => currentDominantSin;
        public SinAxisSaveData[] Axes => axes;
        public int CollapseCount => collapseCount;
        public float DeathCorruption => deathCorruption;
        public DeathCompanionChoice CompanionChoice => companionChoice;
        public int Seed => seed;
        public long RandomDrawCount => randomDrawCount;
        public SinEventId[] ProcessedEventIds => processedEventIds;
        public string[] ResolvedTemptationIds => resolvedTemptationIds;

        public SinRunSaveData(
            string runId,
            long nextEventSequence,
            SinType originSin,
            SinType currentDominantSin,
            SinAxisSaveData[] axes,
            int collapseCount,
            float deathCorruption,
            DeathCompanionChoice companionChoice,
            int seed,
            long randomDrawCount,
            SinEventId[] processedEventIds,
            string[] resolvedTemptationIds,
            string rulesVersion = CurrentRulesVersion)
        {
            schemaVersion = CurrentSchemaVersion;
            this.rulesVersion = rulesVersion ?? CurrentRulesVersion;
            this.runId = runId ?? string.Empty;
            this.nextEventSequence = Math.Max(1, nextEventSequence);
            this.originSin = originSin;
            this.currentDominantSin = currentDominantSin;
            this.axes = axes ?? Array.Empty<SinAxisSaveData>();
            this.collapseCount = Math.Max(0, collapseCount);
            this.deathCorruption = Math.Max(0f, Math.Min(1f, deathCorruption));
            this.companionChoice = companionChoice;
            this.seed = seed;
            this.randomDrawCount = Math.Max(0, randomDrawCount);
            this.processedEventIds = processedEventIds ?? Array.Empty<SinEventId>();
            this.resolvedTemptationIds = resolvedTemptationIds ?? Array.Empty<string>();
        }

        public void Validate()
        {
            if (schemaVersion != CurrentSchemaVersion)
                throw new InvalidOperationException($"Unsupported save schema {schemaVersion}.");
            if (rulesVersion != CurrentRulesVersion)
                throw new InvalidOperationException($"Unsupported rules version '{rulesVersion}'.");
            if (string.IsNullOrWhiteSpace(runId))
                throw new InvalidOperationException("Save data has no run identifier.");
            if (axes == null || axes.Length != 7)
                throw new InvalidOperationException("Save data must contain all seven sin axes.");
        }
    }
}
